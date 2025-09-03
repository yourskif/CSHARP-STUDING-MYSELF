using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetByIdAsync(int id)
            => await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);

        public async Task<IEnumerable<Book>> GetAllAsync()
            => await _context.Books.Include(b => b.Author).ToListAsync();

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
            => await _context.Books.Where(b => b.AuthorId == authorId).ToListAsync();

        public async Task AddRangeAsync(IEnumerable<Book> books)
        {
            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();
        }
    }
}