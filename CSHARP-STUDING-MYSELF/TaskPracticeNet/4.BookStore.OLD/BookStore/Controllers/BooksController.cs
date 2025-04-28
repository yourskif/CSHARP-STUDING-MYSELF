using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository) => _bookRepository = bookRepository;

        public IActionResult Create(int authorId) => View(new Book { AuthorId = authorId });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookRepository.AddAsync(book);
                return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
            }
            return View(book);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book == null ? NotFound() : View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book != null) await _bookRepository.DeleteAsync(id);
            return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
        }
    }
}