using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookLendingSystem.Application.Features.Borrowing.Commands.ReturnBook
{
   
    public class ReturnBookCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public int BorrowId { get; set; }

        public ReturnBookCommand(string userId, int borrowId)
        {
            UserId = userId;
            BorrowId = borrowId;
        }
    }
}
