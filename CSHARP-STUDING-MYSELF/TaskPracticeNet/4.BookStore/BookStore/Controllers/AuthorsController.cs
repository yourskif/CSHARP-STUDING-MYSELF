using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();
            return View(authors);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }

        // POST: Authors/SaveBooks
        [HttpPost]
        public async Task<IActionResult> SaveBooks([FromBody] SaveBooksRequest request)
        {
            if (request == null || request.Books == null || !request.Books.Any())
            {
                return BadRequest("Немає книг для збереження");
            }

            var author = await _context.Authors.FindAsync(request.AuthorId);
            if (author == null)
            {
                return NotFound("Автор не знайдений");
            }

            foreach (var book in request.Books)
            {
                book.AuthorId = request.AuthorId;
                _context.Books.Add(book);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class SaveBooksRequest
    {
        public int AuthorId { get; set; }
        public List<Book> Books { get; set; }
    }
}



//using BookStore.Data;
//using BookStore.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace BookStore.Controllers
//{
//    public class AuthorsController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public AuthorsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Authors
//        public async Task<IActionResult> Index()
//        {
//            var authors = await _context.Authors
//                .Include(a => a.Books)
//                .ToListAsync();
//            return View(authors);
//        }

//        // GET: Authors/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var author = await _context.Authors
//                .Include(a => a.Books)
//                .FirstOrDefaultAsync(m => m.Id == id);

//            if (author == null)
//            {
//                return NotFound();
//            }

//            return View(author);
//        }

//        // GET: Authors/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Authors/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Author author)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(author);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(author);
//        }

//        // GET: Authors/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var author = await _context.Authors
//                .Include(a => a.Books)
//                .FirstOrDefaultAsync(a => a.Id == id);

//            if (author == null)
//            {
//                return NotFound();
//            }

//            return View(author);
//        }

//        // POST: Authors/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Author author)
//        {
//            if (id != author.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    // Оновлюємо автора
//                    _context.Update(author);

//                    // Оновлюємо або додаємо книги
//                    foreach (var book in author.Books)
//                    {
//                        if (book.Id == 0) // Нова книга
//                        {
//                            book.AuthorId = author.Id;
//                            _context.Books.Add(book);
//                        }
//                        else // Оновлення існуючої книги
//                        {
//                            _context.Books.Update(book);
//                        }
//                    }

//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!AuthorExists(author.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(author);
//        }

//        // GET: Authors/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var author = await _context.Authors
//                .Include(a => a.Books)
//                .FirstOrDefaultAsync(m => m.Id == id);

//            if (author == null)
//            {
//                return NotFound();
//            }

//            return View(author);
//        }

//        // POST: Authors/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var author = await _context.Authors.FindAsync(id);
//            if (author == null)
//            {
//                return NotFound();
//            }

//            _context.Authors.Remove(author);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool AuthorExists(int id)
//        {
//            return _context.Authors.Any(e => e.Id == id);
//        }

//        // GET: Authors/AddBook
//        public IActionResult AddBook(int authorId)
//        {
//            var book = new Book { AuthorId = authorId };
//            return View(book);
//        }

//        // POST: Authors/AddBook
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> AddBook(Book book)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Books.Add(book);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Details), new { id = book.AuthorId });
//            }
//            return View(book);
//        }

//        // POST: Authors/SaveBooks
//        [HttpPost]
//        public async Task<IActionResult> SaveBooks([FromBody] SaveBooksRequest request)
//        {
//            if (request == null || request.Books == null || !request.Books.Any())
//            {
//                return BadRequest("Немає книг для збереження");
//            }

//            var author = await _context.Authors.FindAsync(request.AuthorId);
//            if (author == null)
//            {
//                return NotFound("Автор не знайдений");
//            }

//            using (var transaction = await _context.Database.BeginTransactionAsync())
//            {
//                try
//                {
//                    foreach (var book in request.Books)
//                    {
//                        book.AuthorId = request.AuthorId;
//                        _context.Books.Add(book);
//                    }

//                    await _context.SaveChangesAsync();
//                    await transaction.CommitAsync();
//                    return Ok();
//                }
//                catch (System.Exception ex)
//                {
//                    await transaction.RollbackAsync();
//                    return StatusCode(500, $"Внутрішня помилка сервера: {ex.Message}");
//                }
//            }
//        }
//    }

//    public class SaveBooksRequest
//    {
//        public int AuthorId { get; set; }
//        public List<Book> Books { get; set; }
//    }
//}




////using BookStore.Data;
////using BookStore.Models;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using System.Linq;
////using System.Threading.Tasks;

////namespace BookStore.Controllers
////{
////    public class AuthorsController : Controller
////    {
////        private readonly ApplicationDbContext _context;

////        public AuthorsController(ApplicationDbContext context)
////        {
////            _context = context;
////        }

////        // GET: Authors
////        public async Task<IActionResult> Index()
////        {
////            var authors = await _context.Authors
////                .Include(a => a.Books)
////                .ToListAsync();
////            return View(authors);
////        }

////        // GET: Authors/Details/5
////        public async Task<IActionResult> Details(int? id)
////        {
////            if (id == null)
////            {
////                return NotFound();
////            }

////            var author = await _context.Authors
////                .Include(a => a.Books)
////                .FirstOrDefaultAsync(m => m.Id == id);

////            if (author == null)
////            {
////                return NotFound();
////            }

////            return View(author);
////        }

////        // GET: Authors/Create
////        public IActionResult Create()
////        {
////            return View();
////        }

////        // POST: Authors/Create
////        [HttpPost]
////        [ValidateAntiForgeryToken]
////        public async Task<IActionResult> Create(Author author)
////        {
////            if (ModelState.IsValid)
////            {
////                _context.Add(author);
////                await _context.SaveChangesAsync();
////                return RedirectToAction(nameof(Index));
////            }
////            return View(author);
////        }

////        // GET: Authors/Edit/5
////        public async Task<IActionResult> Edit(int? id)
////        {
////            if (id == null)
////            {
////                return NotFound();
////            }

////            var author = await _context.Authors
////                .Include(a => a.Books)
////                .FirstOrDefaultAsync(a => a.Id == id);

////            if (author == null)
////            {
////                return NotFound();
////            }

////            return View(author);
////        }

////        // POST: Authors/Edit/5
////        [HttpPost]
////        [ValidateAntiForgeryToken]
////        public async Task<IActionResult> Edit(int id, Author author)
////        {
////            if (id != author.Id)
////            {
////                return NotFound();
////            }

////            if (ModelState.IsValid)
////            {
////                try
////                {
////                    // Оновлюємо автора
////                    _context.Update(author);

////                    // Оновлюємо або додаємо книги
////                    foreach (var book in author.Books)
////                    {
////                        if (book.Id == 0) // Нова книга
////                        {
////                            book.AuthorId = author.Id;
////                            _context.Books.Add(book);
////                        }
////                        else // Оновлення існуючої книги
////                        {
////                            _context.Books.Update(book);
////                        }
////                    }

////                    await _context.SaveChangesAsync();
////                }
////                catch (DbUpdateConcurrencyException)
////                {
////                    if (!AuthorExists(author.Id))
////                    {
////                        return NotFound();
////                    }
////                    else
////                    {
////                        throw;
////                    }
////                }
////                return RedirectToAction(nameof(Index));
////            }
////            return View(author);
////        }

////        // GET: Authors/Delete/5
////        public async Task<IActionResult> Delete(int? id)
////        {
////            if (id == null)
////            {
////                return NotFound();
////            }

////            var author = await _context.Authors
////                .Include(a => a.Books)
////                .FirstOrDefaultAsync(m => m.Id == id);

////            if (author == null)
////            {
////                return NotFound();
////            }

////            return View(author);
////        }

////        // POST: Authors/Delete/5
////        [HttpPost, ActionName("Delete")]
////        [ValidateAntiForgeryToken]
////        public async Task<IActionResult> DeleteConfirmed(int id)
////        {
////            var author = await _context.Authors.FindAsync(id);
////            _context.Authors.Remove(author);
////            await _context.SaveChangesAsync();
////            return RedirectToAction(nameof(Index));
////        }

////        private bool AuthorExists(int id)
////        {
////            return _context.Authors.Any(e => e.Id == id);
////        }

////        // GET: Authors/AddBook
////        public IActionResult AddBook(int authorId)
////        {
////            var book = new Book { AuthorId = authorId };
////            return View(book);
////        }

////        // POST: Authors/AddBook
////        [HttpPost]
////        [ValidateAntiForgeryToken]
////        public async Task<IActionResult> AddBook(Book book)
////        {
////            if (ModelState.IsValid)
////            {
////                _context.Books.Add(book);
////                await _context.SaveChangesAsync();
////                return RedirectToAction(nameof(Details), new { id = book.AuthorId });
////            }
////            return View(book);
////        }
////    }
////}



////using BookStore.Data;
////using BookStore.Models;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using System.Linq;
////using System.Threading.Tasks;

////namespace BookStore.Controllers
////{
////    public class AuthorsController : Controller
////    {
////        private readonly ApplicationDbContext _context;

////        public AuthorsController(ApplicationDbContext context)
////        {
////            _context = context;
////        }

////        // GET: Authors/Details/5
////        public async Task<IActionResult> Details(int? id)
////        {
////            if (id == null)
////            {
////                return NotFound();
////            }

////            var author = await _context.Authors
////                .Include(a => a.Books)
////                .FirstOrDefaultAsync(m => m.Id == id);

////            if (author == null)
////            {
////                return NotFound();
////            }

////            return View(author);
////        }

////        // GET: Authors/AddBook
////        public IActionResult AddBook(int authorId)
////        {
////            var book = new Book { AuthorId = authorId };
////            return View(book);
////        }

////        // POST: Authors/AddBook
////        [HttpPost]
////        [ValidateAntiForgeryToken]
////        public async Task<IActionResult> AddBook(Book book)
////        {
////            if (ModelState.IsValid)
////            {
////                _context.Books.Add(book);
////                await _context.SaveChangesAsync();
////                return RedirectToAction(nameof(Details), new { id = book.AuthorId });
////            }
////            return View(book);
////        }
////    }
////}


//////using BookStore.Data;
//////using BookStore.Models;
//////using Microsoft.AspNetCore.Mvc;
//////using Microsoft.EntityFrameworkCore;
//////using System.Linq;
//////using System.Threading.Tasks;

//////namespace BookStore.Controllers
//////{
//////    public class AuthorsController : Controller
//////    {
//////        private readonly ApplicationDbContext _context;

//////        public AuthorsController(ApplicationDbContext context)
//////        {
//////            _context = context;
//////        }

//////        // GET: Authors
//////        public async Task<IActionResult> Index()
//////        {
//////            var authors = await _context.Authors
//////                .Include(a => a.Books)
//////                .ToListAsync();
//////            return View(authors);
//////        }

//////        // GET: Authors/Details/5
//////        public async Task<IActionResult> Details(int? id)
//////        {
//////            if (id == null)
//////            {
//////                return NotFound();
//////            }

//////            var author = await _context.Authors
//////                .Include(a => a.Books)
//////                .FirstOrDefaultAsync(m => m.Id == id);

//////            if (author == null)
//////            {
//////                return NotFound();
//////            }

//////            return View(author);
//////        }

//////        // GET: Authors/Create
//////        public IActionResult Create()
//////        {
//////            return View();
//////        }

//////        // POST: Authors/Create
//////        [HttpPost]
//////        [ValidateAntiForgeryToken]
//////        public async Task<IActionResult> Create(Author author)
//////        {
//////            if (ModelState.IsValid)
//////            {
//////                _context.Add(author);
//////                await _context.SaveChangesAsync();
//////                return RedirectToAction(nameof(Index));
//////            }
//////            return View(author);
//////        }

//////        // GET: Authors/Edit/5
//////        public async Task<IActionResult> Edit(int? id)
//////        {
//////            if (id == null)
//////            {
//////                return NotFound();
//////            }

//////            var author = await _context.Authors
//////                .Include(a => a.Books)
//////                .FirstOrDefaultAsync(a => a.Id == id);

//////            if (author == null)
//////            {
//////                return NotFound();
//////            }

//////            return View(author);
//////        }

//////        // POST: Authors/Edit/5
//////        [HttpPost]
//////        [ValidateAntiForgeryToken]
//////        public async Task<IActionResult> Edit(int id, Author author)
//////        {
//////            if (id != author.Id)
//////            {
//////                return NotFound();
//////            }

//////            if (ModelState.IsValid)
//////            {
//////                try
//////                {
//////                    // Оновлюємо автора
//////                    _context.Update(author);

//////                    // Оновлюємо або додаємо книги
//////                    foreach (var book in author.Books)
//////                    {
//////                        if (book.Id == 0) // Нова книга
//////                        {
//////                            book.AuthorId = author.Id;
//////                            _context.Books.Add(book);
//////                        }
//////                        else // Оновлення існуючої книги
//////                        {
//////                            _context.Books.Update(book);
//////                        }
//////                    }

//////                    await _context.SaveChangesAsync();
//////                }
//////                catch (DbUpdateConcurrencyException)
//////                {
//////                    if (!AuthorExists(author.Id))
//////                    {
//////                        return NotFound();
//////                    }
//////                    else
//////                    {
//////                        throw;
//////                    }
//////                }
//////                return RedirectToAction(nameof(Index));
//////            }
//////            return View(author);
//////        }

//////        // GET: Authors/Delete/5
//////        public async Task<IActionResult> Delete(int? id)
//////        {
//////            if (id == null)
//////            {
//////                return NotFound();
//////            }

//////            var author = await _context.Authors
//////                .Include(a => a.Books)
//////                .FirstOrDefaultAsync(m => m.Id == id);

//////            if (author == null)
//////            {
//////                return NotFound();
//////            }

//////            return View(author);
//////        }

//////        // POST: Authors/Delete/5
//////        [HttpPost, ActionName("Delete")]
//////        [ValidateAntiForgeryToken]
//////        public async Task<IActionResult> DeleteConfirmed(int id)
//////        {
//////            var author = await _context.Authors.FindAsync(id);
//////            _context.Authors.Remove(author);
//////            await _context.SaveChangesAsync();
//////            return RedirectToAction(nameof(Index));
//////        }

//////        private bool AuthorExists(int id)
//////        {
//////            return _context.Authors.Any(e => e.Id == id);
//////        }
//////    }
//////}



////////using BookStore.Data;
////////using BookStore.Models;
////////using Microsoft.AspNetCore.Mvc;
////////using Microsoft.EntityFrameworkCore;
////////using System.Linq;
////////using System.Threading.Tasks;

////////namespace BookStore.Controllers
////////{
////////    public class AuthorsController : Controller
////////    {
////////        private readonly ApplicationDbContext _context;

////////        public AuthorsController(ApplicationDbContext context)
////////        {
////////            _context = context;
////////        }

////////        // GET: Authors
////////        public async Task<IActionResult> Index()
////////        {
////////            var authors = await _context.Authors
////////                .Include(a => a.Books)
////////                .ToListAsync();
////////            return View(authors);
////////        }

////////        // GET: Authors/Details/5
////////        public async Task<IActionResult> Details(int? id)
////////        {
////////            if (id == null)
////////            {
////////                return NotFound();
////////            }

////////            var author = await _context.Authors
////////                .Include(a => a.Books)
////////                .FirstOrDefaultAsync(m => m.Id == id);

////////            if (author == null)
////////            {
////////                return NotFound();
////////            }

////////            return View(author);
////////        }

////////        // GET: Authors/Create
////////        public IActionResult Create()
////////        {
////////            return View();
////////        }

////////        // POST: Authors/Create
////////        [HttpPost]
////////        [ValidateAntiForgeryToken]
////////        public async Task<IActionResult> Create(Author author)
////////        {
////////            if (ModelState.IsValid)
////////            {
////////                _context.Add(author);
////////                await _context.SaveChangesAsync();
////////                return RedirectToAction(nameof(Index));
////////            }
////////            return View(author);
////////        }

////////        // GET: Authors/Edit/5
////////        public async Task<IActionResult> Edit(int? id)
////////        {
////////            if (id == null)
////////            {
////////                return NotFound();
////////            }

////////            var author = await _context.Authors.FindAsync(id);
////////            if (author == null)
////////            {
////////                return NotFound();
////////            }
////////            return View(author);
////////        }

////////        // POST: Authors/Edit/5
////////        [HttpPost]
////////        [ValidateAntiForgeryToken]
////////        public async Task<IActionResult> Edit(int id, Author author)
////////        {
////////            if (id != author.Id)
////////            {
////////                return NotFound();
////////            }

////////            if (ModelState.IsValid)
////////            {
////////                try
////////                {
////////                    _context.Update(author);
////////                    await _context.SaveChangesAsync();
////////                }
////////                catch (DbUpdateConcurrencyException)
////////                {
////////                    if (!AuthorExists(author.Id))
////////                    {
////////                        return NotFound();
////////                    }
////////                    else
////////                    {
////////                        throw;
////////                    }
////////                }
////////                return RedirectToAction(nameof(Index));
////////            }
////////            return View(author);
////////        }

////////        // GET: Authors/Delete/5
////////        public async Task<IActionResult> Delete(int? id)
////////        {
////////            if (id == null)
////////            {
////////                return NotFound();
////////            }

////////            var author = await _context.Authors
////////                .Include(a => a.Books)
////////                .FirstOrDefaultAsync(m => m.Id == id);

////////            if (author == null)
////////            {
////////                return NotFound();
////////            }

////////            return View(author);
////////        }

////////        // POST: Authors/Delete/5
////////        [HttpPost, ActionName("Delete")]
////////        [ValidateAntiForgeryToken]
////////        public async Task<IActionResult> DeleteConfirmed(int id)
////////        {
////////            var author = await _context.Authors.FindAsync(id);
////////            _context.Authors.Remove(author);
////////            await _context.SaveChangesAsync();
////////            return RedirectToAction(nameof(Index));
////////        }

////////        private bool AuthorExists(int id)
////////        {
////////            return _context.Authors.Any(e => e.Id == id);
////////        }
////////    }
////////}