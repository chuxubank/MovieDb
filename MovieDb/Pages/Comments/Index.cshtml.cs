using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDb.Authorization;
using MovieDb.Data;

namespace MovieDb.Pages.Comments
{
    public class IndexModel : PageModel
    {
        private readonly MovieDb.Data.ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(MovieDb.Data.ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public PaginatedList<Comment> Comment { get; set; }
        public SelectList Movies { get; set; }
        public SelectList Users { get; set; }

        public string DateSort { get; set; }
        public string RatingSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int? CurrentMovieID { get; set; }
        public string CurrentUserID { get; set; }

        public async Task OnGetAsync(string sortOrder,
                                     string currentFilter,
                                     string searchString,
                                     int? currentMovieID,
                                     int? movieID,
                                     string currentUserID,
                                     string userID,
                                     int? pageIndex)
        {
            CurrentSort = sortOrder;
            DateSort = sortOrder == "Comment Date" ? "date_desc" : "Comment Date";
            RatingSort = sortOrder == "Rating" ? "rating_desc" : "Rating";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (movieID != null)
            {
                pageIndex = 1;
            }
            else
            {
                movieID = currentMovieID;
            }
            if (userID != null)
            {
                pageIndex = 1;
            }
            else
            {
                userID = currentUserID;
            }
            CurrentFilter = searchString;
            CurrentUserID = userID;
            CurrentMovieID = movieID;

            IQueryable<Movie> movies = from m in _context.Movie
                                       select m;

            IQueryable<ApplicationUser> users = from u in _context.Users
                                                select u;

            IQueryable<Comment> comments = from c in _context.Comment.Include("Movie")
                                             .Include("User")
                                           select c;
            
            if (!(User.IsInRole(Constants.MovieAdministratorsRole) || User.IsInRole(Constants.MovieManagersRole)))
            {
                comments = comments.Where(c => c.UserID.Equals(_userManager.GetUserId(User)));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                comments = comments.Where(c => c.Content.Contains(searchString));
            }

            if (movieID != null && movieID != 0)
            {
                comments = comments.Where(c => c.MovieID.Equals(movieID));
            }

            if (!String.IsNullOrEmpty(userID))
            {
                comments = comments.Where(c => c.UserID.Equals(userID));
            }

            switch (sortOrder)
            {
                case "Comment Date":
                    comments = comments.OrderBy(c => c.Date);
                    break;
                case "date_desc":
                    comments = comments.OrderByDescending(c => c.Date);
                    break;
                case "Rating":
                    comments = comments.OrderBy(c => c.Rating);
                    break;
                case "rating_desc":
                    comments = comments.OrderByDescending(c => c.Rating);
                    break;
                default:
                    comments = comments.OrderByDescending(c => c.Date);
                    break;
            }
            Movies = new SelectList(movies, "ID", "Title");

            Users = new SelectList(users, "Id", "UserName");

            int pageSize = 6;
            Comment = await PaginatedList<Comment>.CreateAsync(comments, pageIndex ?? 1, pageSize);
        }
    }
}
