﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MovieDb.Data.MovieViewModels
{
    public class ReleaseDateGroup
    {
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        public int MovieCount { get; set; }
    }
}
