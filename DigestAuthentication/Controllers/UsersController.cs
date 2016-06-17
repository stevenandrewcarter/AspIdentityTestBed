using DigestAuthentication.Filters;
using System.Web.Http;

namespace DigestAuthentication.Controllers {
  [RoutePrefix("api")]
  public class UsersController : ApiController {
    [Route("users/{id}", Name = "GetUserById")]
    [Authentication(realm: "Basic", opaque: "Test Opaque")]
    public IHttpActionResult GetUser(string Id) {
      if (Id == "1") {
        return Ok(new { id = Id, name = "Test " + Id.ToString() });
      }
      return NotFound();
    }
  }
}