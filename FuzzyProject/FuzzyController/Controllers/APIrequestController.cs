using FuzzyController.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuzzyController.Controllers
{
    public class APIrequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public APIrequestController(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // GET: /APIrequest/Submit
        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        // POST: /APIrequest/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(APIrequest model)
        {
            // Перевірка наявності користувача у сесії до перевірки валідності моделі
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Завантаження користувача з бази даних
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                // Якщо користувача не знайдено, перенаправити на логін
                return RedirectToAction("Login", "Account");
            }

            // Призначення UserId перед перевіркою на валідність
            model.UserId = user.Id;
            model.User = user;
            model.CreatedAt = DateTime.UtcNow;

            _context.APIrequests.Add(model);
            _context.SaveChanges();

            // Створення даних для API
            var feedbackData = new
            {
                food_quality = model.FoodQuality,
                service_level = model.ServiceLevel,
                atmosphere = model.Atmosphere
            };

            // Виконання запиту до Python API
            var response = await _httpClient.PostAsJsonAsync("http://127.0.0.2:80/calculate-feedback/", feedbackData);

            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                double mark = result.GetProperty("result")[0].GetDouble();
                mark = Math.Round(mark, 2);
                string category = result.GetProperty("result")[1].GetString();

                TempData["Score"] = $"{mark} ({category})"; // Збереження значення у TempData
                var rating = new RestaurantRating
                {
                    RestaurantName = model.Name,
                    Rating = category
                };
                _context.RestaurantRatings.Add(rating);
                _context.SaveChanges();
                return RedirectToAction("Result");
            }
            else
            {
                // Обробка помилки у разі неуспішного запиту
                return RedirectToAction("Error");
            }
        }

        // GET: /APIrequest/Result
        public IActionResult Result(double score)
        {
            ViewData["Score"] = score;  // Показуємо результат
            return View();
        }





    }
}
