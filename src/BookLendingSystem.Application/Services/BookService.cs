using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Domain.Enums; 

namespace BookLendingSystem.Application.Services
{
    
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _repository;
        private readonly IMapper _mapper;

        public BookService(IRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return null;

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> AddBookAsync(CreateBookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);

         
            book.CreatedAt = DateTime.UtcNow;
            book.Status = BookStatus.Available;

            await _repository.AddAsync(book);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto bookDto)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return false;

            _mapper.Map(bookDto, book);
            book.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(book);
            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return false;

            await _repository.DeleteAsync(book);
            return true;
        }
    }
}