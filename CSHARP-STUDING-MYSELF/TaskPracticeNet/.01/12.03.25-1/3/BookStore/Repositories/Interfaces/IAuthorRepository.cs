using BookStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByIdAsync(int id);
        Task<IEnumerable<Author>> GetAllAsync();
        Task AddAsync(Author author);
        Task UpdateAsync(Author author);
        Task DeleteAsync(int id);
        Task<IEnumerable<Author>> GetAuthorsWithBooksAsync();

        // Додаємо цей метод
        Task<bool> Exists(int id);
    }
}



//using BookStore.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace BookStore.Repositories.Interfaces
//{
//    public interface IAuthorRepository
//    {
//        Task<Author?> GetByIdAsync(int id);
//        Task<IEnumerable<Author>> GetAllAsync();
//        Task AddAsync(Author author);
//        Task UpdateAsync(Author author);
//        Task DeleteAsync(int id);
//        Task<IEnumerable<Author>> GetAuthorsWithBooksAsync();
//    }
//}