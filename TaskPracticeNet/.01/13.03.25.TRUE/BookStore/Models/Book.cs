using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Кількість сторінок обов'язкова")]
        [Range(1, 5000, ErrorMessage = "Некоректна кількість сторінок")]
        public int PageCount { get; set; }

        [Required(ErrorMessage = "Жанр обов'язковий")]
        public GenreEnum Genre { get; set; }

        [Required(ErrorMessage = "Автор обов'язковий")]
        public int AuthorId { get; set; }

        public Author? Author { get; set; }
    }
}