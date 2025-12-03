using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Books.Queries.GetAllBooks;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using BookLendingSystem.Infrastructure.Data;
using BookLendingSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLendingSystem.Tests.Features.Books
{
    public class GetAllBooksQueryHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GetAllBooksQueryHandler _handler;
        private readonly IMapper _mapper;

        public GetAllBooksQueryHandlerTests()
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

            _handler = new GetAllBooksQueryHandler(repository, _mapper);
        }

        [Fact] 
        public async Task Handle_ShouldReturnAllBooks_WhenBooksExist()
        {
            // Arrange
            _context.Books.Add(new Book { Title = "Book 1", ISBN = "111", Status = BookStatus.Available });
            _context.Books.Add(new Book { Title = "Book 2", ISBN = "222", Status = BookStatus.Available });
            _context.Books.Add(new Book { Title = "Book 3", ISBN = "333", Status = BookStatus.Borrowed });
            await _context.SaveChangesAsync();

            var query = new GetAllBooksQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3); 
            result.First().Title.Should().Be("Book 1");
        }

        [Fact] 
        public async Task Handle_ShouldReturnEmptyList_WhenNoBooksExist()
        {
            // Arrange
          
            var query = new GetAllBooksQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty(); 
        }
    }
}