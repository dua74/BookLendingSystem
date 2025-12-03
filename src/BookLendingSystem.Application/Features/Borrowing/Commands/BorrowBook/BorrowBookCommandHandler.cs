using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using MediatR;

namespace BookLendingSystem.Application.Features.Borrowing.Commands.BorrowBook
{
    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowRecordDto>
    {
        private readonly IRepository<BorrowRecord> _borrowRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
      

        public BorrowBookCommandHandler(
            IRepository<BorrowRecord> borrowRepository,
            IRepository<Book> bookRepository,
            IMapper mapper)
        {
            _borrowRepository = borrowRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<BorrowRecordDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
           
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var book = await _bookRepository.GetByIdAsync(request.BookId);
                if (book == null)
                    throw new NotFoundException($"Book with ID {request.BookId} not found.");

              
                if (book.Status != BookStatus.Available)
                    throw new BadRequestException("Book is not available for borrowing.");

                var userActiveBorrows = await _borrowRepository.GetAsync(
                    b => b.UserId == request.UserId && b.ReturnedAt == null
                );

                if (userActiveBorrows.Any())
                    throw new BadRequestException("You cannot borrow more than one book at a time.");

                var borrowRecord = new BorrowRecord
                {
                    BookId = request.BookId,
                    UserId = request.UserId,
                    BorrowedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7)
                };

            
                book.Status = BookStatus.Borrowed;
                await _bookRepository.UpdateAsync(book);

             
                await _borrowRepository.AddAsync(borrowRecord);

                scope.Complete();

           
                var resultDto = _mapper.Map<BorrowRecordDto>(borrowRecord);
                resultDto.BookTitle = book.Title;

                return resultDto;
            }



        }
    }
}