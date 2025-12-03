using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Borrowing.Queries.GetMyBorrows;
using BookLendingSystem.Domain.Entities; 
using BookLendingSystem.Domain.Enums;
using BookLendingSystem.Infrastructure.Data;
using BookLendingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLendingSystem.Tests.Features.Borrowing
{
    public class GetMyBorrowsQueryHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GetMyBorrowsQueryHandler _handler;
        private readonly IMapper _mapper;

        public GetMyBorrowsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            var borrowRepo = new Repository<BorrowRecord>(_context);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BorrowRecord, BorrowRecordDto>()
                   .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            });

            _mapper = config.CreateMapper();

            _handler = new GetMyBorrowsQueryHandler(borrowRepo, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyUserRecords_WhenUserHasHistory()
        {
            // Arrange
            
            var user1 = new ApplicationUser { Id = "user1", UserName = "Dua", Email = "dua@test.com" };
            _context.Users.Add(user1);

            var user2 = new ApplicationUser { Id = "user2", UserName = "Ahmed", Email = "ahmed@test.com" };
            _context.Users.Add(user2);

          
            var book1 = new Book { Title = "User1 Book", Status = BookStatus.Borrowed };
            _context.Books.Add(book1);

            var book2 = new Book { Title = "User2 Book", Status = BookStatus.Borrowed };
            _context.Books.Add(book2);

            var record1 = new BorrowRecord
            {
                Book = book1,
                User = user1,
                UserId = "user1",
                BorrowedAt = DateTime.UtcNow
            };
            _context.BorrowRecords.Add(record1);

            var record2 = new BorrowRecord
            {
                Book = book2,
                User = user2,
                UserId = "user2",
                BorrowedAt = DateTime.UtcNow
            };
            _context.BorrowRecords.Add(record2);

            await _context.SaveChangesAsync();

            var query = new GetMyBorrowsQuery("user1");

          
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().BookTitle.Should().Be("User1 Book");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenUserHasNoHistory()
        {
            var query = new GetMyBorrowsQuery("GhostUser");
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().BeEmpty();
        }
    }
}