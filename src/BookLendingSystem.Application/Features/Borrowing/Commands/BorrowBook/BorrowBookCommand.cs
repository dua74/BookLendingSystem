using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Application.DTOs;
using MediatR;

namespace BookLendingSystem.Application.Features.Borrowing.Commands.BorrowBook
{
    public class BorrowBookCommand : IRequest<BorrowRecordDto>
    {
        public string UserId { get; set; }
        public int BookId { get; set; }

        public BorrowBookCommand(string userId, int bookId)
        {
            UserId = userId;
            BookId = bookId;
        }
    }
}
