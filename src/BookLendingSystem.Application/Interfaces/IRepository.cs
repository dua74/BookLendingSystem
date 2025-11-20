using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Interfaces
{
     // T here means any class (Book, BorrowRecord, etc.)
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);


        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
    }

}
