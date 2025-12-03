using System;
using System.Threading;
using System.Threading.Tasks;
using BookLendingSystem.Application.Features.Borrowing.Commands.ReturnBook;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using BookLendingSystem.Infrastructure.Data;
using BookLendingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLendingSystem.Tests.Features.Borrowing
{
    public class ReturnBookCommandHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ReturnBookCommandHandler _handler;

        public ReturnBookCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var bookRepo = new Repository<Book>(_context);
            var borrowRepo = new Repository<BorrowRecord>(_context);

            _handler = new ReturnBookCommandHandler(borrowRepo, bookRepo);
        }

        [Fact]
        public async Task Handle_ShouldReturnBook_WhenDataIsValid()
        {
            // Arrange
            var book = new Book { Title = "Test Book", Status = BookStatus.Borrowed };
            _context.Books.Add(book);

            var borrowRecord = new BorrowRecord
            {
                BookId = book.Id,
                UserId = "user1",
                BorrowedAt = DateTime.UtcNow,
                ReturnedAt = null
            };
            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            var command = new ReturnBookCommand("user1", borrowRecord.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var dbBook = await _context.Books.FindAsync(book.Id);
            dbBook.Status.Should().Be(BookStatus.Available);

            var dbRecord = await _context.BorrowRecords.FindAsync(borrowRecord.Id);
            dbRecord.ReturnedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenBorrowRecordDoesNotExist()
        {
            var command = new ReturnBookCommand("user1", 999);

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenUserIsNotTheBorrower()
        {
            // Arrange
            var borrowRecord = new BorrowRecord { UserId = "user1", BorrowedAt = DateTime.UtcNow };
            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            var command = new ReturnBookCommand("user2", borrowRecord.Id);

            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenBookIsAlreadyReturned()
        {
            // Arrange
            var borrowRecord = new BorrowRecord
            {
                UserId = "user1",
                ReturnedAt = DateTime.UtcNow
            };
            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            var command = new ReturnBookCommand("user1", borrowRecord.Id);

            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }
    }
}