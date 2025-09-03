using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ApplicationState.Controllers
{
    public class CookiesSampleController : Controller
    {
        private const string cookieKey = "mydata";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string value, DateTime expirationDate)
        {
            // Створюємо CookieOptions із заданою датою закінчення
            CookieOptions options = new CookieOptions
            {
                Expires = expirationDate
            };

            // Додаємо значення до Cookies
            Response.Cookies.Append(cookieKey, value, options);

            // Перенаправляємо користувача на сторінку перевірки
            return RedirectToAction("Test");
        }

        public IActionResult Test()
        {
            // Зчитуємо значення Cookies
            string value = Request.Cookies[cookieKey];
            return View("Test", value);
        }
    }
}





//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;

//namespace ApplicationState.Controllers
//{
//    public class CookiesSampleController : Controller
//    {
//        private const string cookieKey = "mydata";

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Index(string value)
//        {
//            CookieOptions options = new CookieOptions();
//            options.Expires = DateTime.Now.AddMinutes(1);

//            Response.Cookies.Append(cookieKey, value, options);

//            return View();
//        }

//        public IActionResult Test()
//        {
//            string value = Request.Cookies[cookieKey];
//            return View(value as object);
//        }

//    }
//}