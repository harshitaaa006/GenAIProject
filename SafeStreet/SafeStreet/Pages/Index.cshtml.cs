using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using CincinnatiCrime;
using System.Diagnostics.Eventing.Reader;

namespace SafeStreet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        static readonly HttpClient client = new HttpClient();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string query) 
        {
            string brand = "Cincinnati Crime";
            string inBrand = Request.Query["Brand"];
            if (inBrand != null && inBrand.Length > 0)
            {
                brand = inBrand;
            }
            ViewData["Brand"] = brand;

            var task = client.GetAsync("https://data.cincinnati-oh.gov/resource/k59e-2pvf.json");
            HttpResponseMessage result = task.Result;
            List<Crime> crimes = new List<Crime>();
            if (result.IsSuccessStatusCode)
            {
                Task<string> readString = result.Content.ReadAsStringAsync();
                string jsonString = readString.Result;
                JSchema schema = JSchema.Parse(System.IO.File.ReadAllText("crime-schema.json"));
                JArray jsonArray = JArray.Parse(jsonString);
                IList<string> crimeEvents = new List<string>();
                if (jsonArray.IsValid(schema, out crimeEvents))
                {
                    crimes = Crime.FromJson(jsonString);
                }
                else
                {
                    foreach (string evt in crimeEvents)
                    {
                        Console.WriteLine(evt);
                    }
                    {
                        if (result.IsSuccessStatusCode)
                        {
                        

                            var offenseByNeighborhood = jsonArray
                         .Where(item => item["cpd_neighborhood"] != null && item["offense"] != null)
                         .GroupBy(
                            item => item["cpd_neighborhood"]?.ToString() ?? "Unknown Neighborhood",
                            item => item["offense"]?.ToString() ?? "Unknown Offense"
                 )
                           //Chatgpt help:
                          .ToDictionary(
                            group => group.Key,
                            group => group.Distinct().ToList()
                        );
                            if (!string.IsNullOrEmpty(query))
                            {
                                offenseByNeighborhood = offenseByNeighborhood
                                    .Where(kvp =>
                                            kvp.Key.Contains(query, StringComparison.OrdinalIgnoreCase) || // Check neighborhood
                                            kvp.Value.Any(offense => offense.Contains(query, StringComparison.OrdinalIgnoreCase)) // Check offenses
                    )
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                            }
                            //

                            ViewData["OffensesByNeighborhood"] = offenseByNeighborhood;
                        }
                        else
                        {
                            ViewData["Query"] = query;
                        }
                    }
                 

                }
                    
            }
        }
    }
}


