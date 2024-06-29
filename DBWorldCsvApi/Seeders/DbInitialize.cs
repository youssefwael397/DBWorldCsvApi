using DBWorldCsvApi.Data;
using DBWorldCsvApi.Models;
using DBWorldCsvApi.Services;
using Microsoft.EntityFrameworkCore;

namespace DBWorldCsvApi.Seeders
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                var hashingService = serviceProvider.GetRequiredService<IHashingService>();

                // Look for any users.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                context.Users.AddRange(
                    new User
                    {
                        Name = "Admin",
                        Email = "admin@example.com",
                        Password = hashingService.Hash("admin123")
                    },
                    new User
                    {
                        Name = "Youssef Wael",
                        Email = "youssef@gmail.com",
                        Password = hashingService.Hash("123456789")
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
