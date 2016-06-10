namespace WebApi.Migrations {
  using Infrastructure;
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.EntityFramework;
  using System;
  using System.Data.Entity.Migrations;

  internal sealed class Configuration : DbMigrationsConfiguration<WebApi.Infrastructure.ApplicationDbContext> {
    public Configuration() {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(WebApi.Infrastructure.ApplicationDbContext context) {
      var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
      var user = new ApplicationUser() {
        UserName = "SuperPowerUser",
        Email = "test@test.com",
        EmailConfirmed = true,
        FirstName = "Test",
        LastName = "Testser",
        Level = 1,
        JoinDate = DateTime.Now.AddYears(-3)
      };
      manager.Create(user, "MySuperP@ssword!");
    }
  }
}
