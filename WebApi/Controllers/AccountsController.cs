using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Infrastructure;
using WebApi.Models;

namespace WebApi.Controllers {
  [RoutePrefix("api/accounts")]
  public class AccountsController : BaseApiController {

    [Authorize(Roles = "Admin")]
    [Route("users")]
    public IHttpActionResult GetUsers() {
      return Ok(AppUserManager.Users.ToList().Select(u => TheModelFactory.Create(u)));
    }

    [Authorize(Roles = "Admin")]
    [Route("user/{id:guid}", Name = "GetUserById")]
    public async Task<IHttpActionResult> GetUser(string Id) {
      var user = await AppUserManager.FindByIdAsync(Id);
      if (user != null) {
        return Ok(TheModelFactory.Create(user));
      }
      return NotFound();

    }

    [Authorize(Roles = "Admin")]
    [Route("user/{username}")]
    public async Task<IHttpActionResult> GetUserByName(string username) {
      var user = await AppUserManager.FindByNameAsync(username);
      if (user != null) {
        return Ok(TheModelFactory.Create(user));
      }
      return NotFound();
    }

    [AllowAnonymous]
    [Route("create")]
    public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      var user = new ApplicationUser() {
        UserName = createUserModel.Username,
        Email = createUserModel.Email,
        FirstName = createUserModel.FirstName,
        LastName = createUserModel.LastName,
        Level = 3,
        JoinDate = DateTime.Now.Date
      };
      var addUserResult = await AppUserManager.CreateAsync(user, createUserModel.Password);
      if (!addUserResult.Succeeded) {
        return GetErrorResult(addUserResult);
      }
      var code = await AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
      var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));
      await AppUserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
      var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));
      return Created(locationHeader, TheModelFactory.Create(user));
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
    public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "") {
      if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code)) {
        ModelState.AddModelError("", "User ID and Code are required");
        return BadRequest(ModelState);
      }
      var result = await AppUserManager.ConfirmEmailAsync(userId, code);
      if (result.Succeeded) {
        return Ok();
      } else {
        return GetErrorResult(result);
      }
    }

    [Authorize]
    [Route("ChangePassword")]
    public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      var result = await AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
      if (!result.Succeeded) {
        return GetErrorResult(result);
      }
      return Ok();
    }

    [Authorize(Roles = "Admin")]
    [Route("user/{id:guid}")]
    public async Task<IHttpActionResult> DeleteUser(string id) {
      var appUser = await AppUserManager.FindByIdAsync(id);
      if (appUser != null) {
        var result = await AppUserManager.DeleteAsync(appUser);
        if (!result.Succeeded) {
          return GetErrorResult(result);
        }
        return Ok();
      }
      return NotFound();
    }

    [Authorize(Roles = "Admin")]
    [Route("user/{id:guid}/roles")]
    [HttpPut]
    public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign) {
      var appUser = await AppUserManager.FindByIdAsync(id);
      if (appUser == null) {
        return NotFound();
      }
      var currentRoles = await AppUserManager.GetRolesAsync(appUser.Id);
      var rolesNotExists = rolesToAssign.Except(AppRoleManager.Roles.Select(x => x.Name)).ToArray();
      if (rolesNotExists.Count() > 0) {
        ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
        return BadRequest(ModelState);
      }
      var removeResult = await AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());
      if (!removeResult.Succeeded) {
        ModelState.AddModelError("", "Failed to remove user roles");
        return BadRequest(ModelState);
      }
      var addResult = await AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);
      if (!addResult.Succeeded) {
        ModelState.AddModelError("", "Failed to add user roles");
        return BadRequest(ModelState);
      }
      return Ok();
    }

    [Authorize(Roles = "Admin")]
    [Route("user/{id:guid}/assignclaims")]
    [HttpPut]
    public async Task<IHttpActionResult> AssignClaimsToUser([FromUri] string id, [FromBody] List<ClaimBindingModel> claimsToAssign) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      var appUser = await AppUserManager.FindByIdAsync(id);
      if (appUser == null) {
        return NotFound();
      }
      foreach (var claimModel in claimsToAssign) {
        if (appUser.Claims.Any(c => c.ClaimType == claimModel.Type)) {
          await AppUserManager.RemoveClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
        }
        await AppUserManager.AddClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
      }
      return Ok();
    }

    [Authorize(Roles = "Admin")]
    [Route("user/{id:guid}/removeclaims")]
    [HttpPut]
    public async Task<IHttpActionResult> RemoveClaimsFromUser([FromUri] string id, [FromBody] List<ClaimBindingModel> claimsToRemove) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      var appUser = await AppUserManager.FindByIdAsync(id);
      if (appUser == null) {
        return NotFound();
      }
      foreach (var claimModel in claimsToRemove) {
        if (appUser.Claims.Any(c => c.ClaimType == claimModel.Type)) {
          await AppUserManager.RemoveClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
        }
      }
      return Ok();
    }
  }
}