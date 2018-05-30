using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieDb.Authorization;

namespace MovieDb.Data
{
    public static class SeedData
    {
        public static void SeedDB(ApplicationDbContext context)
        {
            // Look for any movies.
            if (context.Movie.Any())
            {
                return;   // DB has been seeded
            }

            context.Movie.AddRange(
                new Movie
                {
                    Title = "When Harry Met Sally",
                    ReleaseDate = DateTime.Parse("1989-2-12"),
                    Genre = "Romantic Comedy",
                    BoxOffice = 7.99M,
                    Rating = 8.2
                },

                new Movie
                {
                    Title = "Ghostbusters",
                    ReleaseDate = DateTime.Parse("1984-3-13"),
                    Genre = "Comedy",
                    BoxOffice = 8.99M,
                    Rating = 9.4
                },

                new Movie
                {
                    Title = "Ghostbusters 2",
                    ReleaseDate = DateTime.Parse("1986-2-23"),
                    Genre = "Comedy",
                    BoxOffice = 9.99M,
                    Rating = 9.1
                },

                new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.Parse("1959-4-15"),
                    Genre = "Western",
                    BoxOffice = 3.99M,
                    Rating = 8.0
                }
            );
            context.SaveChanges();
        }

        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                SeedDB(context);
                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@moviedb.com");
                await EnsureRole(serviceProvider, adminID, Constants.MovieAdministratorsRole);

                // allowed user can create and edit contacts that they create
                var uid = await EnsureUser(serviceProvider, testUserPw, "manager@moviedb.com");
                await EnsureRole(serviceProvider, uid, Constants.MovieManagersRole);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                     string testUserPw, 
                                                     string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                             string uid, 
                                                             string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }
}
