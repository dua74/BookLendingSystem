using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Application.DTOs;

namespace BookLendingSystem.Application.Interfaces
{
    public interface IBorrowService
    {
        Task<BorrowRecordDto> BorrowBookAsync(string userId, int bookId);
        Task<bool> ReturnBookAsync(string userId, int borrowId);
        Task<IEnumerable<BorrowRecordDto>> GetMyBorrowsAsync(string userId);
    }
}
