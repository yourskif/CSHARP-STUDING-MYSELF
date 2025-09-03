using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace POST_запросы_и_отправка_форм.Pages
{
    //public class IndexModel : PageModel
    //{
    //    public void OnGet()
    //    {
    //    }
    //}

    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        public string Message { get; private set; } = "";
        public void OnGet()
        {
            Message = "Введите свое имя";
        }
        public void OnPost(string username)
        {
            Message = $"Ваше имя: {username}";
        }
    }


}
