using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using MediatR;

namespace BookLendingSystem.Application.Features.Books.Commands.CreateBook
{

    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly IRepository<Book> _repository;
        private readonly IMapper _mapper;

        public CreateBookCommandHandler(IRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            
            var existingBooks = await _repository.GetAsync(b => b.ISBN == request.ISBN);
            if (existingBooks.Any())
                throw new BadRequestException($"A book with ISBN {request.ISBN} already exists.");

           
            var book = _mapper.Map<Book>(request);
            book.CreatedAt = DateTime.UtcNow;
            book.Status = BookStatus.Available;

            
            await _repository.AddAsync(book);

            return _mapper.Map<BookDto>(book);
        }
    }
}
