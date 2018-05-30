﻿using System;
using System.ComponentModel.DataAnnotations;

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

        [Range(0, 10)]
        public double Rating { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        public byte[] Poster { get; set; }
    }
}