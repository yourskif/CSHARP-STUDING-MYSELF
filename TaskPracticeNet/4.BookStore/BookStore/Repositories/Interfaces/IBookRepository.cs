using BookStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Repositories.Interfaces
{
    public interface IBookRepository
    {
        // Отримати книгу за ID
        Task<Book?> GetByIdAsync(int id);

        // Отримати всі книги
        Task<IEnumerable<Book>> GetAllAsync();

        // Додати одну книгу
        Task AddAsync(Book book);

        // Додати кілька книг
        Task AddRangeAsync(IEnumerable<Book> books);

        // Оновити книгу
        Task UpdateAsync(Book book);

        // Видалити книгу за ID
        Task DeleteAsync(int id);

        // Отримати книги за ID автора
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId);
    }
}