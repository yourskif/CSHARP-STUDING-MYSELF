using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BookStore.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Прізвище є обов’язковим")]
        [StringLength(50, ErrorMessage = "Максимум 50 символів")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Ім’я є обов’язковим")]
        [StringLength(50)]
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Дата народження є обов’язковою")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
    }
}