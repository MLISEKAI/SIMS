using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;
using System;

namespace SIMS.Models
{
    public class SeedData : Controller
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

                // Hash the password


                context.Users.AddRange(
                    new Users
                    {
                        UserName = "admin",
                        Pass = "1234",
                        Email = "admin@example.com",
                        UserRole = "Admin"
                    }
                );
                context.SaveChanges();
            }
        }

    }
}
