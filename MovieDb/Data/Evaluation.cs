using System;
using System.ComponentModel.DataAnnotations;

namespace MovieDb.Data
{
    public class Evaluation
    {
        public int ID { get; set; }

        public int MovieID { get; set; }
        public Movie Movie { get; set; }

        public int UserID { get; set; }
        public ApplicationUser User { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0, 10)]
        public double Rating { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
