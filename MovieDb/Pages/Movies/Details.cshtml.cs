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

            CurrentUser = await _userManager.GetUserAsync(HttpContext.User);

            CurrentUserComment = await _context.Comment.SingleAsync(c => c.MovieID == Movie.ID && c.UserID == CurrentUser.Id);

            if (Movie == null)
            {
                return NotFound();
            }

            IQueryable<Comment> comments = (from c in _context.Comment select c).Where(c => c.MovieID == id);
            int pageSize = 6;
            Comment = await PaginatedList<Comment>.CreateAsync(comments.AsNoTracking(), pageIndex ?? 1, pageSize);
            foreach (var c in Comment)
            {
                c.User = await _context.Users.SingleOrDefaultAsync(u => u.Id == c.UserID);
            }
            return Page();
        }

        public async Task<bool> HasCommentAsync()
        {
            if (!Comment.Any())
                return false;
            return (await _context.Comment.SingleAsync(c => c.MovieID == Movie.ID && c.UserID == CurrentUser.Id) != null);
        }
    }
}
