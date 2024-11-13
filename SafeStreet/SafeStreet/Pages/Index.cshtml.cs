using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using QuickType;

namespace SafeStreet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        HttpClient _httpClient = new HttpClient();


        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

            // Grab specimen data.
            Task<HttpResponseMessage> task = _httpClient.GetAsync("https://data.cincinnati-oh.gov/resource/k59e-2pvf.json");
            HttpResponseMessage result = task.Result;


            List<Crime> crimes = new List<Crime>();
            if (result.IsSuccessStatusCode)
            {
                Task<string> readString = result.Content.ReadAsStringAsync();
                string crimeJSON = readString.Result;

                // validate incoming JSON
                // read our schema file.
                JSchema jSchema = JSchema.Parse(System.IO.File.ReadAllText("crime-schema.json"));
                JArray specimenArray = JArray.Parse(crimeJSON);
                // create a collection to hold errors.
                IList<string> validationEvents = new List<String>();

                if (specimenArray.IsValid(jSchema, out validationEvents))
                {
                    crimes = Crime.FromJson(crimeJSON);
                }
                else
                {
                    foreach (string evt in validationEvents)
                    {
                        Console.WriteLine(evt);
                    }
                }


            }

        }
        }
    }

