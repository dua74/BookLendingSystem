using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Books.Commands.CreateBook;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using BookLendingSystem.Infrastructure.Data;
using BookLendingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLendingSystem.Tests.Features.Books
{
    public class CreateBookCommandHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CreateBookCommandHandler _handler;
        private readonly IMapper _mapper;

        public CreateBookCommandHandlerTests()
        {
            
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            var repository = new Repository<Book>(_context);

           
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateBookCommand, Book>();
                cfg.CreateMap<Book, BookDto>();
            });
            _mapper = config.CreateMapper();

            _handler = new CreateBookCommandHandler(repository, _mapper);
        }

        [Fact] 
        public async Task Handle_ShouldCreateBook_WhenISBNIsUnique()
        {
            // Arrange
            var command = new CreateBookCommand
            {
                Title = "Clean Code",
                Author = "Robert Martin",
                ISBN = "978-0132350884",
                Description = "A book about clean code"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.ISBN.Should().Be("978-0132350884");
            result.Status.Should().Be(BookStatus.Available.ToString()); 

            var count = await _context.Books.CountAsync();
            count.Should().Be(1);
        }

        [Fact] 
        public async Task Handle_ShouldThrowBadRequest_WhenISBNAlreadyExists()
        {
            // Arrange
           
            var existingBook = new Book
            {
                Title = "Old Version",
                ISBN = "111-222",
                Status = BookStatus.Available
            };
            _context.Books.Add(existingBook);
            await _context.SaveChangesAsync();

            var command = new CreateBookCommand
            {
                Title = "New Version",
                ISBN = "111-222", 
                Author = "Dua"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }
    }
}