using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers {
  [Authorize(Roles = "Admin")]
  [RoutePrefix("api/roles")]
  public class RolesController : BaseApiController {
    [Route("{id:guid}", Name = "GetRoleById")]
    public async Task<IHttpActionResult> GetRole(string id) {
      var role = await AppRoleManager.FindByIdAsync(id);
      if (role != null) {
        return Ok(TheModelFactory.Create(role));
      }
      return NotFound();
    }

    [Route("", Name = "GetAllRoles")]
    public IHttpActionResult GetAllRoles() {
      return Ok(AppRoleManager.Roles);
    }

    [Route("create")]
    public async Task<IHttpActionResult> Create(CreateRoleBindingModel model) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      var role = new IdentityRole { Name = model.Name };
      var result = await AppRoleManager.CreateAsync(role);
      if (!result.Succeeded) {
        return GetErrorResult(result);
      }
      var locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));
      return Created(locationHeader, TheModelFactory.Create(role));
    }

    [Route("{id:guid}")]
    public async Task<IHttpActionResult> DeleteRole(string id) {
      var role = await AppRoleManager.FindByIdAsync(id);
      if (role != null) {
        var result = await AppRoleManager.DeleteAsync(role);
        if (!result.Succeeded) {
          return GetErrorResult(result);
        }
        return Ok();
      }
      return NotFound();
    }

    [Route("ManageUsersInRole")]
    public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model) {
      var role = await AppRoleManager.FindByIdAsync(model.Id);
      if (role == null) {
        ModelState.AddModelError("", "Role does not exist");
        return BadRequest(ModelState);
      }
      foreach (string user in model.EnrolledUsers) {
        var appUser = await AppUserManager.FindByIdAsync(user);
        if (appUser == null) {
          ModelState.AddModelError("", string.Format("User: {0} does not exists", user));
          continue;
        }
        if (!AppUserManager.IsInRole(user, role.Name)) {
          var result = await AppUserManager.AddToRoleAsync(user, role.Name);
          if (!result.Succeeded) {
            ModelState.AddModelError("", string.Format("User: {0} could not be added to role", user));
          }
        }
      }
      foreach (string user in model.RemovedUsers) {
        var appUser = await AppUserManager.FindByIdAsync(user);
        if (appUser == null) {
          ModelState.AddModelError("", string.Format("User: {0} does not exists", user));
          continue;
        }
        var result = await AppUserManager.RemoveFromRoleAsync(user, role.Name);
        if (!result.Succeeded) {
          ModelState.AddModelError("", string.Format("User: {0} could not be removed from role", user));
        }
      }
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      return Ok();
    }
  }
}