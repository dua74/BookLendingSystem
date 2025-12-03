using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using MediatR;

namespace BookLendingSystem.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
    {
        private readonly IRepository<Book> _repository;
        private readonly IMapper _mapper;

        public UpdateBookCommandHandler(IRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _repository.GetByIdAsync(request.Id);
            if (book == null)
                throw new NotFoundException($"Book with ID {request.Id} not found.");

            _mapper.Map(request, book); 
            book.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(book);
            return true;
        }
    }
}
