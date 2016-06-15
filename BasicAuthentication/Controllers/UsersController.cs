using BasicAuthentication.Filters;
using System.Web.Http;

namespace BasicAuthentication.Controllers {
  /// <summary>
  /// Example User Controller (./api/users/{id})
  /// The only successful result will be if the id of 1 is provided, otherwise all other results return a 404
  /// </summary>
  [RoutePrefix("api")]
  [BasicAuthenticator(realm: "Basic")]
  public class UsersController : ApiController {
    [Route("users/{id}", Name = "GetUserById")]
    public IHttpActionResult GetUser(string Id) {
      if (Id == "1") {
        return Ok(new { id = Id, name = "Test " + Id.ToString() });
      }
      return NotFound();
    }
  }
}