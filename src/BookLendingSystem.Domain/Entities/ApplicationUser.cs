using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BookLendingSystem.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    
}
}
