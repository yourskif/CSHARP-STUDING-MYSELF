using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ViewBag_и_ViewData.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            //            ViewData["Message"] = "Razor Pages on METANIT.COM";
            ViewData["Message"] = "Список пользователей";
            ViewData["People"] = new List<string> { "Tom", "Sam", "Bob" };
        }
    }
}
