using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieDb.Data;

namespace MovieDb.Pages.Movies
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Movie Movie { get; set; }
        public PaginatedList<Comment> Comment { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public Comment CurrentUserComment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie = await _context.Movie.SingleOrDefaultAsync(m => m.ID == id);

            await _context.Entry(Movie).Collection(m => m.Comments).LoadAsync();

            CurrentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (CurrentUser != null)
                CurrentUserComment = Movie.Comments.SingleOrDefault(c => c.UserID == CurrentUser.Id);

            if (Movie == null)
            {
                return NotFound();
            }

            IQueryable<Comment> comments = (from c in _context.Comment select c).Where(c => c.MovieID == id);
            int pageSize = 6;
            Comment = await PaginatedList<Comment>.CreateAsync(comments, pageIndex ?? 1, pageSize);
            foreach (var c in Comment)
            {
                c.User = await _context.Users.SingleOrDefaultAsync(u => u.Id == c.UserID);
            }

            return Page();
        }

        public bool HasCurrentUserComment()
        {
            if (!Comment.Any() || CurrentUser == null)
                return false;
            return CurrentUserComment != null;
        }
    }
}
