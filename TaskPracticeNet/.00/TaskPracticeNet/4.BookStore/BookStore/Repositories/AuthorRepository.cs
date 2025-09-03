using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookStore.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
            Console.WriteLine("AuthorRepository створено");
        }

        public async Task<IEnumerable<Author>> GetAuthorsWithBooksAsync()
        {
            Console.WriteLine("GetAuthorsWithBooksAsync викликано");
            var authors = await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();
            Console.WriteLine($"GetAuthorsWithBooksAsync повертає {authors.Count} авторів");
            return authors;
        }

        public override async Task<Author?> GetByIdAsync(int id)
        {
            Console.WriteLine($"=== Початок GetByIdAsync ===");
            Console.WriteLine($"Отримано Id: {id} (тип: {id.GetType().Name})");

            // Логуємо всіх авторів перед пошуком
            var allAuthors = await _context.Authors.ToListAsync();
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
            Console.WriteLine($"Усі автори в базі даних: {JsonSerializer.Serialize(allAuthors, options)}");

            // Пошук автора за Id
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                Console.WriteLine($"Автор з Id {id} не знайдений");
            }
            else
            {
                Console.WriteLine($"Знайдено автора: Id={author.Id}, LastName={author.LastName}, FirstName={author.FirstName}");
            }

            Console.WriteLine($"=== Кінець GetByIdAsync ===");
            return author;
        }

        public override async Task<IEnumerable<Author>> GetAllAsync()
        {
            Console.WriteLine("GetAllAsync викликано");
            var authors = await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();
            Console.WriteLine($"GetAllAsync повертає {authors.Count} авторів");
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
            Console.WriteLine($"Усі автори: {JsonSerializer.Serialize(authors, options)}");
            return authors;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Authors.AnyAsync(a => a.Id == id);
        }
    }
}


//using BookStore.Data;
//using BookStore.Models;
//using BookStore.Repositories.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;

//namespace BookStore.Repositories
//{
//    public class AuthorRepository : Repository<Author>, IAuthorRepository
//    {
//        public AuthorRepository(ApplicationDbContext context) : base(context)
//        {
//            Console.WriteLine("AuthorRepository створено");
//        }

//        public async Task<IEnumerable<Author>> GetAuthorsWithBooksAsync()
//        {
//            Console.WriteLine("GetAuthorsWithBooksAsync викликано");
//            var authors = await _context.Authors
//                .Include(a => a.Books)
//                .ToListAsync();
//            Console.WriteLine($"GetAuthorsWithBooksAsync повертає {authors.Count} авторів");
//            return authors;
//        }

//        public override async Task<Author?> GetByIdAsync(int id)
//        {
//            Console.WriteLine($"=== Початок GetByIdAsync ===");
//            Console.WriteLine($"Отримано Id: {id} (тип: {id.GetType().Name})");

//            // Логуємо всіх авторів перед пошуком
//            var allAuthors = await _context.Authors.ToListAsync();
//            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
//            Console.WriteLine($"Усі автори в базі даних: {JsonSerializer.Serialize(allAuthors, options)}");

//            // Пошук автора за Id
//            var author = await _context.Authors.FindAsync(id);
//            if (author == null)
//            {
//                Console.WriteLine($"Автор з Id {id} не знайдений");
//            }
//            else
//            {
//                Console.WriteLine($"Знайдено автора: Id={author.Id}, LastName={author.LastName}, FirstName={author.FirstName}");
//            }

//            Console.WriteLine($"=== Кінець GetByIdAsync ===");
//            return author;
//        }

//        public override async Task<IEnumerable<Author>> GetAllAsync()
//        {
//            Console.WriteLine("GetAllAsync викликано");
//            var authors = await _context.Authors
//                .Include(a => a.Books)
//                .ToListAsync();
//            Console.WriteLine($"GetAllAsync повертає {authors.Count} авторів");
//            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
//            Console.WriteLine($"Усі автори: {JsonSerializer.Serialize(authors, options)}");
//            return authors;
//        }
//    }
//}