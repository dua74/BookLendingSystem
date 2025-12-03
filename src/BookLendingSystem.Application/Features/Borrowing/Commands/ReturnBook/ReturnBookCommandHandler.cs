using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BookLendingSystem.Application.Exceptions;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;
using MediatR;

namespace BookLendingSystem.Application.Features.Borrowing.Commands.ReturnBook
{
    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, bool>
    {
        private readonly IRepository<BorrowRecord> _borrowRepository;
        private readonly IRepository<Book> _bookRepository;

        public ReturnBookCommandHandler(IRepository<BorrowRecord> borrowRepository, IRepository<Book> bookRepository)
        {
            _borrowRepository = borrowRepository;
            _bookRepository = bookRepository;
        }

        public async Task<bool> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var record = await _borrowRepository.GetByIdAsync(request.BorrowId);

                if (record == null) throw new NotFoundException($"Borrow record {request.BorrowId} not found.");
                if (record.UserId != request.UserId) throw new BadRequestException("You are not authorized to return this book.");
                if (record.ReturnedAt != null) throw new BadRequestException("This book is already returned.");

          
                record.ReturnedAt = DateTime.UtcNow;
                await _borrowRepository.UpdateAsync(record);

             
                var book = await _bookRepository.GetByIdAsync(record.BookId);
                if (book != null)
                {
                    book.Status = BookStatus.Available;
                    await _bookRepository.UpdateAsync(book);
                }

                scope.Complete(); // Commit Transaction
                return true;
            }
        }
    }
}
