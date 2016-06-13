using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using WebApi.Validators;

namespace WebApi.Infrastructure {
  public class ApplicationUserManager : UserManager<ApplicationUser> {
    public ApplicationUserManager(IUserStore<ApplicationUser> store)
      : base(store) {

    }

    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) {
      var appDbContext = context.Get<ApplicationDbContext>();
      var appUserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(appDbContext));
      appUserManager.EmailService = new Services.EmailService();
      var dataProtectionProvider = options.DataProtectionProvider;
      if (dataProtectionProvider != null) {
        appUserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity")) {
          TokenLifespan = TimeSpan.FromHours(6)
        };
      }
      appUserManager.UserValidator = new MyCustomUserValidator(appUserManager) {
        AllowOnlyAlphanumericUserNames = true,
        RequireUniqueEmail = true
      };
      appUserManager.PasswordValidator = new MyCustomPasswordValidator {
        RequiredLength = 6,
        RequireNonLetterOrDigit = true,
        RequireDigit = false,
        RequireLowercase = true,
        RequireUppercase = true
      };
      return appUserManager;
    }
  }
}