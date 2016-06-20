namespace ClaimsBasedAuthorization.Migrations {
  using Infrastructure;
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.EntityFramework;
  using System;
  using System.Data.Entity.Migrations;

  internal sealed class Configuration : DbMigrationsConfiguration<ClaimsBasedAuthorization.Infrastructure.ApplicationDbContext> {
    public Configuration() {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(ApplicationDbContext context) {
      //  This method will be called after migrating to the latest version.
      var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
      var user = new ApplicationUser() {
        UserName = "SuperPowerUser",
        Email = "superpoweruser@test.com",
        EmailConfirmed = true,
        FirstName = "Test",
        LastName = "Power User",
        Level = 1,
        JoinDate = DateTime.Now.AddYears(-3)
      };
      manager.Create(user, "MySuperP@ssword!");
    }
  }
}
