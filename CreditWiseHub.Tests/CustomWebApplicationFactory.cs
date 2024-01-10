using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Repository.Contexts;
using CreditWiseHub.Tests.Seeds;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            TestServerOptions.TestServer = true;

            builder.ConfigureServices(async services =>
            {
                var descriptor = services.Single(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<AppDbContext>));

                services.Remove(descriptor);
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });


                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<UserApp>>();
                    var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
                    db.Database.EnsureCreated();
                  
                    SeedService seedService = new SeedService(db, userManager, roleManager);
                    await seedService.SeedAsync();
                }
            });
        }      
    }
}
