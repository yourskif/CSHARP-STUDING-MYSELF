using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ApplicationDbContext _context;

        public AuthorsController(
            IAuthorRepository authorRepository,
            IBookRepository bookRepository,
            ApplicationDbContext context)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _context = context;
        }

        // Сторінка зі списком авторів
        public async Task<IActionResult> Index(string? authorFilter, string? genreFilter)
        {
            var authors = await _authorRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(authorFilter))
                authors = authors.Where(a => a.Id == int.Parse(authorFilter));

            if (!string.IsNullOrEmpty(genreFilter))
                authors = authors.Where(a => a.Books.Any(b => b.Genre.ToString() == genreFilter));

            ViewBag.AuthorFilter = authorFilter;
            ViewBag.GenreFilter = genreFilter;

            return View(authors);
        }

        // Деталі автора
        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            author.Books = (await _bookRepository.GetBooksByAuthorIdAsync(id)).ToList();
            return View(author);
        }

        // Видалення автора (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            author.Books = (await _bookRepository.GetBooksByAuthorIdAsync(id)).ToList();
            return View(author);
        }

        // Видалення автора (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            // Видаляємо книги автора
            var books = await _bookRepository.GetBooksByAuthorIdAsync(id);
            if (books.Any())
            {
                _context.Books.RemoveRange(books);
                await _context.SaveChangesAsync();
            }

            await _authorRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Редагування автора (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            author.Books = (await _bookRepository.GetBooksByAuthorIdAsync(id)).ToList();
            return View(author);
        }

        // Редагування автора (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _authorRepository.UpdateAsync(author);

                    // Оновлюємо або додаємо книги
                    foreach (var book in author.Books)
                    {
                        if (book.Id == 0)
                            await _bookRepository.AddAsync(book);
                        else
                            await _bookRepository.UpdateAsync(book);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _authorRepository.Exists(author.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(author);
        }

        // Збереження книг через AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBooks([FromBody] SaveBooksRequest request)
        {
            if (request == null)
            {
                Console.Error.WriteLine("Помилка: запит на збереження книг порожній.");
                return Json(new { success = false, message = "Invalid request" });
            }

            var author = await _authorRepository.GetByIdAsync(request.AuthorId);
            if (author == null)
            {
                Console.Error.WriteLine($"Помилка: автор з ID {request.AuthorId} не знайдений.");
                return Json(new { success = false, message = "Author not found" });
            }

            if (request.Books == null || !request.Books.Any())
            {
                Console.Error.WriteLine("Помилка: список книг для збереження порожній.");
                return Json(new { success = false, message = "No books to save" });
            }

            try
            {
                await _bookRepository.AddRangeAsync(request.Books);
                Console.WriteLine($"Книги для автора з ID {request.AuthorId} успішно збережено.");
                return Json(new { success = true, message = "Books saved successfully" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Помилка під час збереження книг: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        // Допоміжний клас для AJAX-запиту
        public class SaveBooksRequest
        {
            public int AuthorId { get; set; }
            public List<Book> Books { get; set; }
        }
    }
}