using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Возвращение_результата_в_Razor_Pages.Pages
{
    public class IndexModel : PageModel
    {
        //public IActionResult OnGet()
        //{
        //    //  return Content("Hello METANIT.COM!");
        //    //retu

        public IActionResult OnGet(string? name)
        {
            if (name != null) return Content($"Hello {name}");
            return Page();
        }
    }
}
