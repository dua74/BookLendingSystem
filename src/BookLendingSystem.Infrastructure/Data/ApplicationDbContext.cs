using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookLendingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        // (Fluent API)
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 

            builder.Entity<Book>(entity =>
            {
                // for isbn uniqueness
                entity.HasIndex(b => b.ISBN).IsUnique();

            });

            builder.Entity<BorrowRecord>(entity =>
            {
                
                entity.HasOne(br => br.Book)
                      .WithMany(b => b.BorrowRecords)
                      .HasForeignKey(br => br.BookId)
                      .OnDelete(DeleteBehavior.Restrict); 

                
                entity.HasOne(br => br.User)
                      .WithMany(u => u.BorrowRecords)
                      .HasForeignKey(br => br.UserId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });
        }
    }

}
