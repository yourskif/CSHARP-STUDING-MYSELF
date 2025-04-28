using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using System.Linq;
using System.Collections.Generic;

namespace BookStore.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ApplicationDbContext _context;

        public AuthorsController(IAuthorRepository authorRepository, IBookRepository bookRepository, ApplicationDbContext context)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _context = context;
            Console.WriteLine("AuthorsController створено");
        }

        public async Task<IActionResult> Index(string? authorFilter = null, string? genreFilter = null)
        {
            Console.WriteLine("Index викликано");
            IEnumerable<Author> authors = await _authorRepository.GetAllAsync();
            Console.WriteLine($"Знайдено {authors.Count()} авторів у базі");
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
            Console.WriteLine($"Список авторів: {JsonSerializer.Serialize(authors, options)}");

            if (!string.IsNullOrEmpty(authorFilter))
            {
                authors = authors.Where(a => a.Id == int.Parse(authorFilter));
                Console.WriteLine($"Фільтр за автором: {authorFilter}, знайдено {authors.Count()} авторів");
            }
            if (!string.IsNullOrEmpty(genreFilter))
            {
                authors = authors.Where(a => a.Books.Any(b => b.Genre.ToString() == genreFilter));
                Console.WriteLine($"Фільтр за жанром: {genreFilter}, знайдено {authors.Count()} авторів");
            }

            // Передаємо поточні значення фільтрів у ViewBag
            ViewBag.AuthorFilter = authorFilter;
            ViewBag.GenreFilter = genreFilter;

            return View(authors);
        }

        public async Task<IActionResult> Details(int id)
        {
            Console.WriteLine($"Запит Details для id: {id}");
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                Console.WriteLine($"Автор із id: {id} не знайдений у Details");
                return NotFound();
            }

            author.Books = (await _bookRepository.GetBooksByAuthorIdAsync(id)).ToList();
            Console.WriteLine($"Знайдено автора: {author.Id}, {author.LastName}, кількість книг: {author.Books.Count}");
            return View(author);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine($"Запит Delete для id: {id}");
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                Console.WriteLine($"Автор із id: {id} не знайдений у Delete");
                return NotFound();
            }

            author.Books = (await _bookRepository.GetBooksByAuthorIdAsync(id)).ToList();
            Console.WriteLine($"Знайдено автора: {author.Id}, {author.LastName}, кількість книг: {author.Books.Count}");
            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine($"Запит DeleteConfirmed для id: {id}");
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                Console.WriteLine($"Автор із id: {id} не знайдений у DeleteConfirmed");
                return NotFound();
            }

            var books = await _bookRepository.GetBooksByAuthorIdAsync(id);
            if (books.Any())
            {
                _context.Books.RemoveRange(books);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Видалено {books.Count()} книг автора {author.LastName}");
            }

            await _authorRepository.DeleteAsync(id);
            Console.WriteLine($"Автор {author.LastName} успішно видалений");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            Console.WriteLine($"Запит Edit для id: {id}");
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                Console.WriteLine($"Автор із id: {id} не знайдений у Edit");
                return NotFound();
            }

            author.Books = (await _bookRepository.GetBooksByAuthorIdAsync(id)).ToList();
            Console.WriteLine($"Знайдено автора: {author.Id}, {author.LastName}, кількість книг: {author.Books.Count}");
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            Console.WriteLine($"Запит Edit (POST) для id: {id}");
            if (id != author.Id)
            {
                Console.WriteLine($"Невірний ID автора: {id} != {author.Id}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _authorRepository.UpdateAsync(author);

                    foreach (var book in author.Books)
                    {
                        if (book.Id == 0)
                        {
                            await _bookRepository.AddAsync(book);
                        }
                        else
                        {
                            await _bookRepository.UpdateAsync(book);
                        }
                    }

                    Console.WriteLine($"Автор {author.LastName} успішно оновлений");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _authorRepository.Exists(author.Id))
                    {
                        Console.WriteLine($"Автор із id: {author.Id} не знайдений");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            Console.WriteLine("Помилка валідації при редагуванні автора");
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBooks([FromBody] SaveBooksRequest request)
        {
            if (request == null)
            {
                Console.WriteLine("Отримано null замість об'єкта SaveBooksRequest");
                return Json(new { success = false, message = "Невірний формат даних" });
            }

            Console.WriteLine($"=== Початок SaveBooks ===");
            Console.WriteLine($"Отримано authorId: {request.AuthorId}, тип: {request.AuthorId.GetType().Name}");
            Console.WriteLine($"Отримано книги: {JsonSerializer.Serialize(request.Books ?? new List<Book>())}");

            var author = await _authorRepository.GetByIdAsync(request.AuthorId);
            if (author == null)
            {
                Console.WriteLine($"Автор із ID {request.AuthorId} не знайдений");
                return Json(new { success = false, message = "Автор не знайдений" });
            }

            if (request.Books == null || !request.Books.Any())
            {
                Console.WriteLine("Список книг порожній");
                return Json(new { success = false, message = "Список книг порожній" });
            }

            foreach (var book in request.Books)
            {
                Console.WriteLine($"Обробка книги: {book.Title}, Genre: {book.Genre}, PageCount: {book.PageCount}");

                if (book.Genre == 0)
                {
                    book.Genre = GenreEnum.Fiction;
                }

                ModelState.Remove("Author");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    Console.WriteLine("Помилка валідації: " + string.Join(", ", errors));
                    return Json(new { success = false, message = "Помилка валідації: " + string.Join(", ", errors) });
                }

                book.AuthorId = request.AuthorId;
                await _bookRepository.AddAsync(book);
            }

            Console.WriteLine("Книги успішно збережено");
            return Json(new { success = true, message = "Книги успішно збережено" });
        }

        public class SaveBooksRequest
        {
            public int AuthorId { get; set; }
            public List<Book> Books { get; set; }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                await _authorRepository.AddAsync(author);
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }
    }
}