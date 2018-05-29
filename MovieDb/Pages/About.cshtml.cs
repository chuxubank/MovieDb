using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieDb.Data;
using MovieDb.Models.MovieViewModels;

namespace MovieDb.Pages
{
    [AllowAnonymous]
    public class AboutModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AboutModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ReleaseDateGroup> Movie { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<ReleaseDateGroup> data =
                from m in _context.Movie
                group m by m.ReleaseDate into dateGroup
                select new ReleaseDateGroup()
                {
                    ReleaseDate = dateGroup.Key,
                    MovieCount = dateGroup.Count()
                };
            Movie = await data.AsNoTracking().ToListAsync();
        }
    }
}
