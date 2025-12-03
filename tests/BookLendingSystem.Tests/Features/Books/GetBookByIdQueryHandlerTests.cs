using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Books.Queries.GetBookById;
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
    public class GetBookByIdQueryHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GetBookByIdQueryHandler _handler;
        private readonly IMapper _mapper;

        public GetBookByIdQueryHandlerTests()
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
                cfg.CreateMap<Book, BookDto>();
            });
            _mapper = config.CreateMapper();

            _handler = new GetBookByIdQueryHandler(repository, _mapper);
        }

        [Fact] 
        public async Task Handle_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var book = new Book { Title = "Specific Book", ISBN = "999", Status = BookStatus.Available };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var query = new GetBookByIdQuery(book.Id); 

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Specific Book");
            result.ISBN.Should().Be("999");
        }

        [Fact] 
        public async Task Handle_ShouldThrowNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var query = new GetBookByIdQuery(9999); 

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(query, CancellationToken.None));
        }
    }
}