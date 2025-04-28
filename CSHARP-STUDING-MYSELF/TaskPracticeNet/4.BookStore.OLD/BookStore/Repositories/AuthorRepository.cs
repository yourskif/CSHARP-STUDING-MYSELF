using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context) : base(context) => _context = context;

        public async Task<IEnumerable<Author>> GetAuthorsWithBooksAsync() =>
            await _context.Authors.Include(a => a.Books).ToListAsync();

        public async Task<bool> Exists(int id) =>
            await _context.Authors.AnyAsync(a => a.Id == id);
    }
}