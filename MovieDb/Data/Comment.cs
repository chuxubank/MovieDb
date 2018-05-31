using System;
using System.ComponentModel.DataAnnotations;

namespace MovieDb.Data
{
    public class Comment
    {
        public int ID { get; set; }

        public int MovieID { get; set; }
        public Movie Movie { get; set; }

        public string UserID { get; set; }
        public ApplicationUser User { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0, 10)]
        public int Rating { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
