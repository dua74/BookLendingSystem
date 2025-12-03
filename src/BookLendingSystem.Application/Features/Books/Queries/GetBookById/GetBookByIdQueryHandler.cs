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
using MediatR;

namespace BookLendingSystem.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
    {
        private readonly IRepository<Book> _repository;
        private readonly IMapper _mapper;

        public GetBookByIdQueryHandler(IRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _repository.GetByIdAsync(request.Id);
            if (book == null)
                throw new NotFoundException($"Book with ID {request.Id} not found.");

            return _mapper.Map<BookDto>(book);
        }
    }

}
