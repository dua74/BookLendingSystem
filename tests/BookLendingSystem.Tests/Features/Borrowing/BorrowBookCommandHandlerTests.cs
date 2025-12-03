using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Borrowing.Commands.BorrowBook;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Application.Mappings;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using BookLendingSystem.Infrastructure.Data;
using BookLendingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLendingSystem.Tests.Features.Borrowing
{
    public class BorrowBookCommandHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly BorrowBookCommandHandler _handler;
        private readonly IMapper _mapper;

        public BorrowBookCommandHandlerTests()
        {
            
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var bookRepo = new Repository<Book>(_context);
            var borrowRepo = new Repository<BorrowRecord>(_context);

           
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

          
            _handler = new BorrowBookCommandHandler(borrowRepo, bookRepo, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldBorrowBook_WhenBookIsAvailable()
        {
           
            var book = new Book { Title = "Test Book", Author = "Dua", ISBN = "123", Status = BookStatus.Available };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var command = new BorrowBookCommand("user1", book.Id);

            var result = await _handler.Handle(command, CancellationToken.None);

            
            result.Should().NotBeNull();
            result.BookTitle.Should().Be("Test Book");

          
            var dbBook = await _context.Books.FindAsync(book.Id);
            dbBook.Status.Should().Be(BookStatus.Borrowed);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenBookDoesNotExist()
        {
           
            var command = new BorrowBookCommand("user1", 99);

            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }


        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenBookIsNotAvailable()
        {
           
            var book = new Book { Title = "Busy Book", Status = BookStatus.Borrowed };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var command = new BorrowBookCommand("user1", book.Id);

            
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenUserHasActiveBorrow()
        {
            
            var bookToBorrow = new Book { Title = "New Book", Status = BookStatus.Available };
            _context.Add(bookToBorrow);

            var existingBorrow = new BorrowRecord
            {
                UserId = "user1",
                BookId = 999, 
                BorrowedAt = DateTime.UtcNow,
                ReturnedAt = null 
            };
            _context.Add(existingBorrow);

            await _context.SaveChangesAsync();

         
            var command = new BorrowBookCommand("user1", bookToBorrow.Id);

            
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }
    }
}