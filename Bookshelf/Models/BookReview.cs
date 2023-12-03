﻿using System.ComponentModel.DataAnnotations;

namespace Bookshelf.Models
{
    public class BookReview
    {
        [Key]
        public int ReviewId { get; set; }
        public int ReviewerId { get; set; }
        public int BookId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public User Reviewer { get; set; }
        public Book Book { get; set; }
    }
}
