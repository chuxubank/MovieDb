using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDb.Data;

namespace MovieDb.Pages.Movies
{
    [AllowAnonymous]
    public class IndexModel : DI_BasePageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public PaginatedList<Movie> Movie { get; set; }
        public SelectList Genres { get; set; }

        public string TitleSort { get; set; }
        public string DateSort { get; set; }
        public string BoxOfficeSort { get; set; }
        public string RatingSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentMovieGenre { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string currentMovieGenre,
                                     string movieGenre, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            DateSort = sortOrder == "Release Date" ? "date_desc" : "Release Date";
            BoxOfficeSort = sortOrder == "Box Office" ? "box_office_desc" : "Box Office";
            RatingSort = sortOrder == "Rating" ? "rating_desc" : "Rating";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (movieGenre != null)
            {
                pageIndex = 1;
            }
            else
            {
                movieGenre = currentMovieGenre;
            }
            CurrentFilter = searchString;
            CurrentMovieGenre = movieGenre;

            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            //Only IEnumerable in memeory can sort Rating
            IEnumerable<Movie> movies = from m in _context.Movie.Include("Comments")
                                       select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }

            switch (sortOrder)
            {
                case "title_desc":
                    movies = movies.OrderByDescending(m => m.Title);
                    break;

                case "Release Date":
                    movies = movies.OrderBy(m => m.ReleaseDate);
                    break;

                case "date_desc":
                    movies = movies.OrderByDescending(m => m.ReleaseDate);
                    break;

                case "Box Office":
                    movies = movies.OrderBy(m => m.BoxOffice);
                    break;

                case "box_office_desc":
                    movies = movies.OrderByDescending(m => m.BoxOffice);
                    break;

                case "Rating":
                    movies = movies.OrderBy(m => m.Rating);
                    break;

                case "rating_desc":
                    movies = movies.OrderByDescending(m => m.Rating);
                    break;

                default:
                    movies = movies.OrderBy(m => m.Title);
                    break;
            }

            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            int pageSize = 6;
            Movie = PaginatedList<Movie>.Create(movies, pageIndex ?? 1, pageSize);
        }
    }
}
