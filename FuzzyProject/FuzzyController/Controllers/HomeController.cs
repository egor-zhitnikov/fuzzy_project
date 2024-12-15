using FuzzyController.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FuzzyController.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            // Отримання останніх 5 відгуків з бази даних, незалежно від стану автентифікації
            var reviews = _context.APIrequests
                                  .OrderByDescending(r => r.CreatedAt)
                                  .Take(5)
                                  .ToList();

            ViewData["Reviews"] = reviews;  // Передача списку відгуків у ViewData

            return View();
        }
    }
}
