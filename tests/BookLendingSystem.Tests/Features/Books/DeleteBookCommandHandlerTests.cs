using System;
using System.Threading;
using System.Threading.Tasks;
using BookLendingSystem.Application.Features.Books.Commands.DeleteBook;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Infrastructure.Data;
using BookLendingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLendingSystem.Tests.Features.Books
{
    public class DeleteBookCommandHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly DeleteBookCommandHandler _handler;

        public DeleteBookCommandHandlerTests()
        {
            // 1. Setup InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            var repository = new Repository<Book>(_context);

            _handler = new DeleteBookCommandHandler(repository);
        }

        [Fact] 
        public async Task Handle_ShouldDeleteBook_WhenBookExists()
        {
            // Arrange
            var book = new Book { Title = "Book To Delete", ISBN = "123" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var command = new DeleteBookCommand(book.Id); 

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue(); 

            var dbBook = await _context.Books.FindAsync(book.Id);
            dbBook.Should().BeNull();
        }

        [Fact] 
        public async Task Handle_ShouldThrowNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var command = new DeleteBookCommand(999); 

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }
    }
}