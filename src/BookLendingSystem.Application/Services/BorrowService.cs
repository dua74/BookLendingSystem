using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums;


namespace BookLendingSystem.Application.Services
{
    public class BorrowService:IBorrowService
    {
        private readonly IRepository<BorrowRecord> _borrowRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper _mapper;

    public BorrowService(IRepository<BorrowRecord> borrowRepository,
                         IRepository<Book> bookRepository,
                         IMapper _mapper)
    {
        _borrowRepository = borrowRepository;
        _bookRepository = bookRepository;
        this._mapper = _mapper;
    }

    public async Task<BorrowRecordDto> BorrowBookAsync(string userId, int bookId)
    {
        
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null) throw new Exception("Book not found.");

        if (book.Status != BookStatus.Available) throw new Exception("Book is currently not available.");

       
        var userActiveBorrows = await _borrowRepository.GetAsync(b => b.UserId == userId && b.ReturnedAt == null);
        if (userActiveBorrows.Any()) throw new Exception("You cannot borrow more than one book at a time.");

      
        var borrowRecord = new BorrowRecord
        {
            BookId = bookId,
            UserId = userId,
            BorrowedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7) 
        };

        book.Status = BookStatus.Borrowed; 
        await _bookRepository.UpdateAsync(book); 
        await _borrowRepository.AddAsync(borrowRecord);

        return _mapper.Map<BorrowRecordDto>(borrowRecord);
    }

    public async Task<bool> ReturnBookAsync(string userId, int borrowId)
    {
        var record = await _borrowRepository.GetByIdAsync(borrowId);

     
        if (record == null || record.UserId != userId) return false;

        if (record.ReturnedAt != null) throw new Exception("Book already returned.");

        
        record.ReturnedAt = DateTime.UtcNow;
        await _borrowRepository.UpdateAsync(record);

        var book = await _bookRepository.GetByIdAsync(record.BookId);
        if (book != null)
        {
            book.Status = BookStatus.Available;
            await _bookRepository.UpdateAsync(book);
        }

        return true;
    }

    public async Task<IEnumerable<BorrowRecordDto>> GetMyBorrowsAsync(string userId)
    {
        var records = await _borrowRepository.GetAsync(b => b.UserId == userId);
        return _mapper.Map<IEnumerable<BorrowRecordDto>>(records);
    }
}
}
