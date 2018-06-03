using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MovieDb.Data
{
    public class Movie
    {
        public int ID { get; set; }
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Box Office")]
        [DataType(DataType.Currency)]
        public decimal BoxOffice { get; set; }

        [Required]
        [StringLength(30)]
        public string Genre { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        public byte[] Poster { get; set; }

        [NotMapped]
        public decimal Rating
        {
            get
            {
                if (Comments!=null && Comments.Count != 0)
                    return (decimal)Comments.Sum(c => c.Rating) / Comments.Count;
                else
                    return -1;
            }
        }

        [NotMapped]
        public int CommentsCount
        {
            get
            {
                if (Comments != null)
                    return Comments.Count;
                else return 0;
            }
        }

        public ICollection<Comment> Comments { get; set; }
    }
}
