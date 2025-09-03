using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using System.Threading.Tasks;

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
    }
}