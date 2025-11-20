using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Application.Services;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;
using System.Linq.Expressions;

namespace BookLendingSystem.Tests.Services
{
    public class BorrowServiceTests
    {
        private readonly IRepository<BorrowRecord> _mockBorrowRepo;
        private readonly IRepository<Book> _mockBookRepo;
        private readonly IMapper _mockMapper;
        private readonly BorrowService _borrowService;

        public BorrowServiceTests()
        {
            _mockBorrowRepo = Substitute.For<IRepository<BorrowRecord>>();
            _mockBookRepo = Substitute.For<IRepository<Book>>();
            _mockMapper = Substitute.For<IMapper>();

            _borrowService = new BorrowService(_mockBorrowRepo, _mockBookRepo, _mockMapper);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldSucceed_WhenBookIsAvailable()
        {
            // Arrange
            var userId = "user-123";
            var bookId = 1;
            var book = new Book { Id = bookId, Title = "Clean Code", Status = BookStatus.Available };

            _mockBookRepo.GetByIdAsync(bookId).Returns(Task.FromResult<Book?>(book));

            _mockBorrowRepo.GetAsync(Arg.Any<Expression<Func<BorrowRecord, bool>>>())
                           .Returns(Task.FromResult<IReadOnlyList<BorrowRecord>>(new List<BorrowRecord>()));

            var expectedDto = new BorrowRecordDto { BookTitle = "Clean Code", DueDate = DateTime.UtcNow.AddDays(7) };
            _mockMapper.Map<BorrowRecordDto>(Arg.Any<BorrowRecord>()).Returns(expectedDto);

            // Act
            var result = await _borrowService.BorrowBookAsync(userId, bookId);

            // Assert
            result.Should().NotBeNull();
            book.Status.Should().Be(BookStatus.Borrowed);
            await _mockBookRepo.Received(1).UpdateAsync(book);
            await _mockBorrowRepo.Received(1).AddAsync(Arg.Is<BorrowRecord>(r => r.BookId == bookId && r.UserId == userId));
        }
    }
}