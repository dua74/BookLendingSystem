using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Application.DTOs;
using MediatR;

namespace BookLendingSystem.Application.Features.Borrowing.Queries.GetMyBorrows
{
 
    public class GetMyBorrowsQuery : IRequest<IEnumerable<BorrowRecordDto>>
    {
        public string UserId { get; set; }

        public GetMyBorrowsQuery(string userId)
        {
            UserId = userId;
        }
    }
}
