using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDb.Data;

namespace MovieDb.Pages.Comments
{
    public class CreateModel : PageModel
    {
        private readonly MovieDb.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(MovieDb.Data.ApplicationDbContext context,
                           UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Comment Comment { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.SingleOrDefaultAsync(m => m.ID == id);
            var user = await _userManager.GetUserAsync(HttpContext.User);

            Comment.UserID = user.Id;
            Comment.MovieID = movie.ID;
            Comment.Date = DateTime.Now;

            _context.Comment.Add(Comment);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Movies/Details",new {id = Comment.MovieID});
        }
    }
}