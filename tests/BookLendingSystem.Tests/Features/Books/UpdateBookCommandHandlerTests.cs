using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.Features.Books.Commands.UpdateBook;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Infrastructure.Data;
using BookLendingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLendingSystem.Tests.Features.Books
{
    public class UpdateBookCommandHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UpdateBookCommandHandler _handler;
        private readonly IMapper _mapper;

        public UpdateBookCommandHandlerTests()
        {
            // 1. Setup InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            var repository = new Repository<Book>(_context);

            // 2. Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateBookCommand, Book>();
            });
            _mapper = config.CreateMapper();

            _handler = new UpdateBookCommandHandler(repository, _mapper);
        }

        [Fact] 
        public async Task Handle_ShouldUpdateBook_WhenBookExists()
        {
            // Arrange
           
            var book = new Book
            {
                Title = "Old Title",
                Author = "Old Author",
                ISBN = "123"
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var command = new UpdateBookCommand
            {
                Id = book.Id, 
                Title = "New Title",
                Author = "New Author",
                ISBN = "123", 
                Description = "Updated Description"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var updatedBook = await _context.Books.FindAsync(book.Id);
            updatedBook.Title.Should().Be("New Title");
            updatedBook.Author.Should().Be("New Author");

            updatedBook.UpdatedAt.Should().NotBeNull();
        }

        [Fact] 
        public async Task Handle_ShouldThrowNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var command = new UpdateBookCommand { Id = 999, Title = "Ghost Book" };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(command, CancellationToken.None));
        }
    }
}