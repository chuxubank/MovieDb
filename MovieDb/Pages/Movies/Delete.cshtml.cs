using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieDb.Authorization;
using MovieDb.Data;

namespace MovieDb.Pages.Movies
{
	public class DeleteModel : DI_BasePageModel
	{
		private readonly ApplicationDbContext _context;

		public DeleteModel(
			ApplicationDbContext context,
			IAuthorizationService authorizationService,
			UserManager<ApplicationUser> userManager)
			: base(context, authorizationService, userManager)
		{
			_context = context;
		}

		[BindProperty]
		public Movie Movie { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if(id == null)
			{
				return NotFound();
			}

            Movie = await _context.Movie.SingleOrDefaultAsync(m => m.ID == id);

            await _context.Entry(Movie).Collection(m => m.Comments).LoadAsync();

			var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Movie, EntityOperations.Delete);
			if(!isAuthorized.Succeeded)
			{
				return new ChallengeResult();
			}

			if(Movie == null)
			{
				return NotFound();
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int? id)
		{
			if(id == null)
			{
				return NotFound();
			}

			var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Movie, EntityOperations.Delete);

			if(!isAuthorized.Succeeded)
			{
				return new ChallengeResult();
			}

			Movie = await _context.Movie.FindAsync(id);

			if(Movie != null)
			{
				_context.Movie.Remove(Movie);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}
