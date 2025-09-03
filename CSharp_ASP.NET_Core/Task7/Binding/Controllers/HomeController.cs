using Binding.Models;
using Microsoft.AspNetCore.Mvc;

namespace Binding.Models
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new FormData()); // ��������� ������� ��'��� ��� ����������� �����
        }

        [HttpPost]
        public IActionResult Index(FormData formData)
        {
            // ��������� ��������� ������� � ���� Output Visual Studio
            System.Diagnostics.Debug.WriteLine($"First: {formData.First}");
            System.Diagnostics.Debug.WriteLine($"Second: {formData.Second}");
            System.Diagnostics.Debug.WriteLine($"Count: {formData.Count}");

            // ��������� ����� ����� �� ����������� ������
            return View(formData);
        }
    }
}
