using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieDb.Authorization;
using MovieDb.Data;

namespace MovieDb.Pages.Movies
{
	public class CreateModel : DI_BasePageModel
	{
		private readonly ApplicationDbContext _context;

		public CreateModel(
			ApplicationDbContext context,
			IAuthorizationService authorizationService,
			UserManager<ApplicationUser> userManager)
			: base(context, authorizationService, userManager)
		{
			_context = context;
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		[BindProperty]
		public Movie Movie { get; set; }

		public async Task<IActionResult> OnPostAsync(IFormFile poster)
		{
			if(!ModelState.IsValid)
			{
				return Page();
			}

			var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Movie, MoiveOperations.Create);

			if(!isAuthorized.Succeeded)
			{
				return new ChallengeResult();
			}

			using(var memoryStream = new MemoryStream())
			{
				await poster.CopyToAsync(memoryStream);
				Movie.Poster = memoryStream.ToArray();
			}

			_context.Movie.Add(Movie);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}