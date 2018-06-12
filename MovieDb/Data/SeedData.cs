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
                    Title = "The Shawshank Redemption",
                    ReleaseDate = DateTime.Parse("1994-10-14"),
                    Genre = "Crime",
                    BoxOffice = 58500000M,
                    Summary = "The Shawshank Redemption is a highly-acclaimed movie starring Tim Robbins and Morgan Freeman. " +
                    "Andy Dufresne is convicted of the murder of his wife and her lover, " +
                    "and sentenced to life imprisonment at Shawshank prison. " +
                    "Life seems to have taken a turn for the worse, " +
                    "but fortunately Andy befriends some of the other inmates, " +
                    "in particular a character known only as Red. " +
                    "Over time Andy finds ways to live out life with relative ease as one can in a prison, " +
                    "leaving a message for all that while the body may be locked away in a cell, " +
                    "the spirit can never be truly imprisoned."
                },

                new Movie
                {
                    Title = "The Godfather",
                    ReleaseDate = DateTime.Parse("1972-3-24"),
                    Genre = "Crime",
                    BoxOffice = 245066411M,
                    Summary = "When the aging head of a famous crime family decides to transfer his position to one of his subalterns, a series of unfortunate events start happening to the family, and a war begins between all the well-known families leading to insolence, deportation, murder and revenge, and ends with the favorable successor being finally chosen. "
                },

                new Movie
                {
                    Title = " The Godfather: Part II",
                    ReleaseDate = DateTime.Parse("1974-12-20"),
                    Genre = "Crime",
                    BoxOffice = 114600000M,
                    Summary = "The continuing saga of the Corleone crime family tells the story of a young Vito Corleone growing up in Sicily and in 1910s New York; and follows Michael Corleone in the 1950s as he attempts to expand the family business into Las Vegas, Hollywood and Cuba."
                },

                new Movie
                {
                    Title = "The Dark Knight",
                    ReleaseDate = DateTime.Parse("2008-7-18"),
                    Genre = "Action",
                    BoxOffice = 1004558444M,
                    Summary = "Set within a year after the events of Batman Begins, Batman, Lieutenant James Gordon, and new district attorney Harvey Dent successfully begin to round up the criminals that plague Gotham City until a mysterious and sadistic criminal mastermind known only as the Joker appears in Gotham, creating a new wave of chaos. Batman's struggle against the Joker becomes deeply personal, forcing him to 'confront everything he believes' and improve his technology to stop him. A love triangle develops between Bruce Wayne, Dent and Rachel Dawes."
                },

                new Movie
                {
                    Title = "Inception",
                    ReleaseDate = DateTime.Parse("2010-7-16"),
                    Genre = "Action",
                    BoxOffice = 825532764M,
                    Summary = "Dom Cobb is a skilled thief, the absolute best in the dangerous art of extraction, stealing valuable secrets from deep within the subconscious during the dream state, when the mind is at its most vulnerable. Cobb's rare ability has made him a coveted player in this treacherous new world of corporate espionage, but it has also made him an international fugitive and cost him everything he has ever loved. Now Cobb is being offered a chance at redemption. One last job could give him his life back but only if he can accomplish the impossible - inception. Instead of the perfect heist, Cobb and his team of specialists have to pull off the reverse: their task is not to steal an idea but to plant one. If they succeed, it could be the perfect crime. But no amount of careful planning or expertise can prepare the team for the dangerous enemy that seems to predict their every move. An enemy that only Cobb could have seen coming."
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

                // allowed user can create and edit movies
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
