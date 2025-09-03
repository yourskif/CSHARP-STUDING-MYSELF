using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class InfoController : Controller
    {
        // Метод для отримання інформації про IP-адресу та браузер
        public IActionResult Index()
        {
            var userInfo = new UserInfoModel
            {
                IPAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            return View(userInfo);
        }
    }

    // Модель для передачі інформації
    public class UserInfoModel
    {
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
