using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Validators {
  public class MyCustomPasswordValidator : PasswordValidator {
    public override async Task<IdentityResult> ValidateAsync(string password) {
      var result = await base.ValidateAsync(password);
      if (password.Contains("abcdef") || password.Contains("123456")) {
        var errors = result.Errors.ToList();
        errors.Add("Password cannot contain sequence of characters");
        result = new IdentityResult(errors);
      }
      return result;
    }
  }
}