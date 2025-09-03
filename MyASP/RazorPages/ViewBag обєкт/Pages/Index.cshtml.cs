using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ViewBag_обєкт.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Message"] = "Список пользователей";
            ViewData["People"] = new List<string> { "Tom", "Sam", "Bob" };
        }
    }
}
