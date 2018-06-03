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

namespace MovieDb.Pages.Comments
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
        public Comment Comment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comment = await _context.Comment
                                    .Include(c => c.Movie)
                                    .Include(c => c.User)
                                    .SingleOrDefaultAsync(m => m.ID == id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Comment, EntityOperations.Delete);
            var isManager = User.IsInRole(Constants.MovieManagersRole) || User.IsInRole(Constants.MovieAdministratorsRole);
            if (!isAuthorized.Succeeded && !isManager)
            {
                return new ChallengeResult();
            }

            if (Comment == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comment = await _context.Comment.FindAsync(id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Comment, EntityOperations.Delete);
            var isManager = User.IsInRole(Constants.MovieManagersRole) || User.IsInRole(Constants.MovieAdministratorsRole);
            if (!isAuthorized.Succeeded && !isManager)
            {
                return new ChallengeResult();
            }

            if (Comment != null)
            {
                _context.Comment.Remove(Comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
