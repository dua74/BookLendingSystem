using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLendingSystem.Domain.Entities
{
    
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BookStatus Status { get; set; } = BookStatus.Available;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}



