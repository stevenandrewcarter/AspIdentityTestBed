using Microsoft.AspNet.Identity.EntityFramework;

namespace RoleBasedAuthorization.Infrastructure {
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false) {
      Configuration.ProxyCreationEnabled = false;
      Configuration.LazyLoadingEnabled = false;
    }

    public static ApplicationDbContext Create() {
      return new ApplicationDbContext();
    }

  }
}