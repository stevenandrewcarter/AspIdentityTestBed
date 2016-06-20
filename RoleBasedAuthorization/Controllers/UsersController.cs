﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RoleBasedAuthorization.Infrastructure;
using RoleBasedAuthorization.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RoleBasedAuthorization.Controllers {
  /// <summary>
  /// Controller for the {api/users} end point. This end point is protected by a JWT Authentication and Role-based Authorization.
  /// JWT can be generated by sending a POST request to the {oauth/token} endpoint with the username and password as a x-www-form-urlencoded body
  /// </summary>
  [Authorize(Roles = "Admin")]
  [RoutePrefix("api")]
  public class UsersController : ApiController {

    private ModelFactory ModelFactory {
      get {
        if (_modelFactory == null) {
          _modelFactory = new ModelFactory(Request, AppUserManager);
        }
        return _modelFactory;
      }
    }
    private ModelFactory _modelFactory;

    private ApplicationUserManager AppUserManager {
      get {
        return _AppUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
      }
    }
    private ApplicationUserManager _AppUserManager = null;

    /// <summary>
    /// Route for GET:/api/users.
    /// </summary>
    /// <returns>All of the users in the System</returns>
    [Route("users")]
    public IHttpActionResult GetUsers() {
      return Ok(AppUserManager.Users.ToList().Select(u => ModelFactory.Create(u)));
    }

    /// <summary>
    /// Route for GET:/api/users/id:guid.
    /// </summary>
    /// <param name="Id">Guid of the User</param>
    /// <returns>The User Object for the given Guid</returns>
    [Route("users/{id:guid}", Name = "GetUserById")]
    public async Task<IHttpActionResult> GetUser(string Id) {
      var user = await AppUserManager.FindByIdAsync(Id);
      if (user != null) {
        return Ok(ModelFactory.Create(user));
      }
      return NotFound();
    }

    /// <summary>
    /// Route for POST:/api/users
    /// </summary>
    /// <param name="createUserModel">A new user Binding Model</param>
    /// <returns>Request Created (201) if created, otherwise Bad Request (400). The body will contain a list of error messages</returns>
    [Route("users")]
    [HttpPost]
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
      var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));
      return Created(locationHeader, ModelFactory.Create(user));
    }

    /// <summary>
    /// Helper method for mapping a IdentityResult to a Http Response Code
    /// </summary>
    /// <param name="result">The Identity Result</param>
    /// <returns>A Http Action Response</returns>
    private IHttpActionResult GetErrorResult(IdentityResult result) {
      if (result == null) {
        return InternalServerError();
      }
      if (!result.Succeeded) {
        if (result.Errors != null) {
          foreach (string error in result.Errors) {
            ModelState.AddModelError("", error);
          }
        }
        if (ModelState.IsValid) {
          // No ModelState errors are available to send, so just return an empty BadRequest.
          return BadRequest();
        }
        return BadRequest(ModelState);
      }
      return null;
    }
  }
}