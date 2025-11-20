using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Domain.Entities;


namespace BookLendingSystem.Domain.Entities
{
    public class BorrowRecord
    {
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }

        [NotMapped]
        public bool IsOverdue => !ReturnedAt.HasValue && DateTime.UtcNow > DueDate;

        [NotMapped]
        public bool IsReturned => ReturnedAt.HasValue;
    }
}



