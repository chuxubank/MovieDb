using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieDb.Data;

namespace MovieDb.Pages.Comments
{
    public class IndexModel : PageModel
    {
        private readonly MovieDb.Data.ApplicationDbContext _context;

        public IndexModel(MovieDb.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Comment> Comment { get; set; }

        public async Task OnGetAsync()
        {
            Comment = await _context.Comment
                .Include(c => c.Movie).ToListAsync();
        }
    }
}
