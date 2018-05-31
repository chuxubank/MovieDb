using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

		public DetailsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public Movie Movie { get; set; }
        public PaginatedList<Comment> Comment { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
		{
			if(id == null)
			{
				return NotFound();
			}

			Movie = await _context.Movie.SingleOrDefaultAsync(m => m.ID == id);

			if(Movie == null)
			{
				return NotFound();
			}

            IQueryable<Comment> comments = (from c in _context.Comment select c).Where(c => c.MovieID == id);
            int pageSize = 6;
            Comment = await PaginatedList<Comment>.CreateAsync(comments.AsNoTracking(), pageIndex ?? 1, pageSize);
            foreach(var c in Comment)
            {
                c.User = await _context.Users.SingleOrDefaultAsync(u => u.Id == c.UserID);
            }
			return Page();
		}
	}
}
