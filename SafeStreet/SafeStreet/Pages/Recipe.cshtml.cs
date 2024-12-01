using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafeStreet.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using System.Globalization;
using SafeStreet.Data.Migration;

namespace SafeStreet.Pages
{
    public class RecipeModel : PageModel
    {
        static readonly HttpClient client = new HttpClient();
        private readonly ILogger<IndexModel> _logger;
    

        // Property to store the fetched meals
        public List<Meal> Meals { get; private set; } = new List<Meal>();

        

       

        public async Task OnGetAsync(string culture = "en")
        {
            // Make the HTTP GET request to fetch meals
            var task = client.GetAsync("https://www.themealdb.com/api/json/v1/1/search.php?s");
            HttpResponseMessage result = task.Result;

            if (result.IsSuccessStatusCode)
            {
                // Read and parse the response JSON
                Task<string> readString = result.Content.ReadAsStringAsync();
                string jsonString = readString.Result;

                JObject jsonResponse = JObject.Parse(jsonString);

                if (jsonResponse["meals"] != null && jsonResponse["meals"].Type == JTokenType.Array)
                {
                    // Deserialize if valid
                    Meals mealsResponse = JsonConvert.DeserializeObject<Meals>(jsonString);

                    if (mealsResponse != null && mealsResponse.MealList != null)
                    {
                        Meals = mealsResponse.MealList;
                    }
                    else
                    {
                        _logger.LogWarning("The 'meals' array is null or could not be deserialized.");
                    }
                }
                else
                {
                    _logger.LogError("Invalid JSON structure: 'meals' array is missing or not valid.");
                }
            }
            else
            {
                _logger.LogError($"Failed to fetch data. Status Code: {result.StatusCode}");
            }
        }
    }
}
