using BookStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(int id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task AddAsync(Book entity);
        Task UpdateAsync(Book entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId);
        Task AddRangeAsync(IEnumerable<Book> books);

        Task<bool> Exists(int id);
    }
}