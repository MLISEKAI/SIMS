using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIMS.Data;
using SIMS.Models;
using System;
using System.Linq;

namespace SIMS.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SIMSContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SIMSContext>>()))
            {
                // Look for any users.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                // Seed initial data
                var adminUser = new Users
                {
                    ID = 1,
                    UserName = "admin",
                    Pass = "1234", 
                    UserRole = "Admin"
                };

                context.Users.Add(adminUser);
                context.SaveChanges();

                context.AdminSystem.Add(new AdminSystem
                {
                    Admin_ID = adminUser.ID,
                    User = adminUser
                });

                context.SaveChanges();
            }
        }

        
    }
}
