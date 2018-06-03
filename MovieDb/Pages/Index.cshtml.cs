using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieDb.Data;

namespace MovieDb.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Movie> TopRatingMovies { get; set; }
        public IList<Movie> TopBoxOfficeMovies { get; set; }
        public IList<Movie> TopCommentMovies { get; set; }
        public IList<Movie> RecentMovies { get; set; }

        public int count = 5;

        public void OnGet()
        {
            IEnumerable<Movie> movies = from m in _context.Movie.Include("Comments")
                                        select m;
            movies = movies.OrderByDescending(m => m.Rating);
            TopRatingMovies = movies.Take(count).ToList();
            movies = movies.OrderByDescending(m => m.ReleaseDate);
            RecentMovies = movies.Take(count).ToList();
            movies = movies.OrderByDescending(m => m.BoxOffice);
            TopBoxOfficeMovies = movies.Take(count).ToList();
            movies = movies.OrderByDescending(m => m.CommentsCount);
            TopCommentMovies = movies.Take(count).ToList();
        }
    }
}
