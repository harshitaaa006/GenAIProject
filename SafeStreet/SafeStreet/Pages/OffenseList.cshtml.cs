using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeStreet.Pages
{
    public class OffenseListModel : PageModel
    {
        private static readonly HttpClient client = new HttpClient();

        public List<string> Offenses { get; private set; } = new();

        public async Task OnGetAsync()
        {
            // API call to fetch data
            var task = client.GetAsync("https://data.cincinnati-oh.gov/resource/k59e-2pvf.json");
            HttpResponseMessage result = await task;

            if (result.IsSuccessStatusCode)
            {
                string jsonString = await result.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonString);

                // Extract unique offenses
                Offenses = jsonArray
                    .Where(item => item["offense"] != null)
                    .Select(item => item["offense"].ToString())
                    .Distinct()
                    .OrderBy(offense => offense)
                    .ToList();
            }
        }
    }
}
