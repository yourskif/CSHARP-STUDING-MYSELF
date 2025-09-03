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
            ViewBag.Author = await _authorRepository.GetByIdAsync(authorId);
            return View(new Book { AuthorId = authorId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookRepository.AddAsync(book);
                return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
            }
            ViewBag.Author = await _authorRepository.GetByIdAsync(book.AuthorId);
            return View(book);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.Author = await _authorRepository.GetByIdAsync(book.AuthorId);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _bookRepository.UpdateAsync(book);
                return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
            }
            ViewBag.Author = await _authorRepository.GetByIdAsync(book.AuthorId);
            return View(book);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book != null)
            {
                await _bookRepository.DeleteAsync(id);
                return RedirectToAction("Edit", "Authors", new { id = book.AuthorId });
            }
            return NotFound();
        }
    }
}