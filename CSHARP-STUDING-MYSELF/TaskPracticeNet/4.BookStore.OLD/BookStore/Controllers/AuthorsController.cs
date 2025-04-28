using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public AuthorsController(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var author = await _authorRepository.GetByIdAsync(id.Value);
            if (author == null) return NotFound();
            author.Books = (await _bookRepository.GetBooksByAuthorIdAsync(id.Value)).ToList();
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,MiddleName,BirthDate,Books")] Author author)
        {
            if (id != author.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    await _authorRepository.UpdateAsync(author);
                    foreach (var book in author.Books)
                    {
                        if (book.Id == 0)
                        {
                            book.AuthorId = author.Id;
                            await _bookRepository.AddAsync(book);
                        }
                        else await _bookRepository.UpdateAsync(book);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AuthorExists(author.Id)) return NotFound();
                    else throw;
                }
            }
            return View(author);
        }

        private async Task<bool> AuthorExists(int id) => await _authorRepository.Exists(id);
    }
}