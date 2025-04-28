using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;

        public BooksController(IBookRepository bookRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
        }

        // Відкриття форми додавання книги (GET)
        public async Task<IActionResult> Create(int authorId)
        {
            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound("Автор не знайдений.");
            }

            ViewBag.Author = author;
            return View(new Book { AuthorId = authorId });
        }

        // Додавання книги (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Author = await _authorRepository.GetByIdAsync(book.AuthorId);
                return View(book);
            }

            var author = await _authorRepository.GetByIdAsync(book.AuthorId);
            if (author == null)
            {
                ModelState.AddModelError("", "Автор не знайдений.");
                ViewBag.Author = null;
                return View(book);
            }

            await _bookRepository.AddAsync(book);
            return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
        }

        // Видалення книги (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound("Книгу не знайдено.");
            }

            return View(book);
        }

        // Видалення книги (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound("Книгу не знайдено.");
            }

            try
            {
                await _bookRepository.DeleteAsync(id);
                Console.WriteLine($"Книга з ID {id} успішно видалена.");
                return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Помилка під час видалення книги з ID {id}: {ex.Message}");
                return StatusCode(500, "Сталася помилка під час видалення книги.");
            }
        }

        // Редагування книги (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound("Книгу не знайдено.");
            }

            return View(book);
        }

        // Редагування книги (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            // Перевірка, чи ID у запиті співпадає з ID книги
            if (id != book.Id)
            {
                return NotFound("Невірний ідентифікатор книги.");
            }

            // Перевірка валідності моделі
            if (!ModelState.IsValid)
            {
                return View(book);
            }

            // Перевірка, чи книга існує в базі даних
            if (!await _bookRepository.Exists(book.Id))
            {
                return NotFound("Книгу не знайдено.");
            }

            try
            {
                // Оновлення книги
                await _bookRepository.UpdateAsync(book);
                return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
            }
            catch (DbUpdateConcurrencyException)
            {
                // Якщо книга була видалена іншим користувачем під час редагування
                if (!await _bookRepository.Exists(book.Id))
                {
                    return NotFound("Книгу не знайдено.");
                }
                else
                {
                    // Якщо сталася інша помилка конкурентного доступу
                    ModelState.AddModelError("", "Не вдалося оновити книгу. Спробуйте ще раз.");
                    return View(book);
                }
            }
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Book book)
        //{
        //    if (id != book.Id)
        //    {
        //        return NotFound("Невірний ідентифікатор книги.");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _bookRepository.UpdateAsync(book);
        //            return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!await _bookRepository.Exists(book.Id))
        //            {
        //                return NotFound("Книгу не знайдено.");
        //            }
        //            throw;
        //        }
        //    }
        //    return View(book);
        //}




    }
}