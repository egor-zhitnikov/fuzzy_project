using FuzzyController.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

namespace FuzzyController.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public RestaurantController(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // Метод для отримання списку ресторанів з більш ніж 5 оцінками
        public async Task<IActionResult> Find()
        {
            var popularRestaurants = await _context.RestaurantRatings
                .GroupBy(r => r.RestaurantName)
                .Where(g => g.Count() >= 5) // Фільтруємо тільки ресторани з >= 5 оцінками
                .Select(g => g.Key) // Отримуємо тільки імена ресторанів
                .ToListAsync();

            return View(popularRestaurants);
        }

        // Метод для відправки запиту до API при виборі ресторану
        [HttpGet]
        public async Task<IActionResult> GetRestaurantRating(string restaurantName)
        {
            if (string.IsNullOrWhiteSpace(restaurantName))
            {
                return BadRequest("Restaurant name is required.");
            }

            // Отримуємо всі оцінки для ресторану
            var feedback = await GetRestaurantRatings(restaurantName);

            // Формуємо тіло запиту у вигляді потрібного JSON-об'єкта
            var feedbackData = new
            {
                feedback = feedback // список строк, например ["Good", "Bad", "Good"]
            };

            // Виконання запиту до Python API
            var response = await _httpClient.PostAsJsonAsync("http://127.0.0.3:80/calculate-feedback/", feedbackData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (result.ValueKind == JsonValueKind.Array && result.GetArrayLength() >= 2)
                {
                    string category = result[0].GetString();
                    double mark = result[1].GetDouble();

                    mark = Math.Round(mark, 2); // Округляем до двух знаков после запятой

                    return Json(new { success = true, mark = mark, category = category });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid response format from API." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Failed to get the rating from API." });
            }
        }

        public async Task<List<string>> GetRestaurantRatings(string restaurantName)
        {
            if (string.IsNullOrWhiteSpace(restaurantName))
            {
                return new List<string> { "Restaurant name is required." };
            }

            // Отримати всі оцінки для ресторану
            var ratings = await _context.RestaurantRatings
                .Where(r => r.RestaurantName == restaurantName)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings;
        }
    }
}
