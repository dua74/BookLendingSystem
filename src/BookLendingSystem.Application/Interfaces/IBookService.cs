using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Application.DTOs;

namespace BookLendingSystem.Application.Interfaces
{
    public interface IBookService
    {

        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> AddBookAsync(CreateBookDto bookDto);
        Task<bool> UpdateBookAsync(int id, UpdateBookDto bookDto);
        Task<bool> DeleteBookAsync(int id);
    }
}

