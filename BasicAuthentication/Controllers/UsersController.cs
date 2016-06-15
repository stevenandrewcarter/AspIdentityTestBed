using System.Web.Http;

namespace BasicAuthentication.Controllers {
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