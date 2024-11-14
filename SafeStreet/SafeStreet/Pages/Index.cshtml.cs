using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Net.Http;

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

        public int CrimeCountLastMonth { get; set; }
        public int CrimeCountLastSixMonths { get; set; }
        public int CrimeCountLastYear { get; set; }
        public int CrimeCountLastTwoYears { get; set; }

        public void OnGet()
        {
            // Grab specimen data.
            Task<HttpResponseMessage> task = _httpClient.GetAsync("https://data.cincinnati-oh.gov/resource/k59e-2pvf.json");
            HttpResponseMessage result = task.Result;




            List<Crime> Crimes = new List<Crime>();
            if (result.IsSuccessStatusCode)
            {
                Task<string> readString = result.Content.ReadAsStringAsync();
                string crimeJSON = readString.Result;



                // validate incoming JSON
                // read our schema file.
                //JSchema jSchema = JSchema.Parse(System.IO.File.ReadAllText("crime-schema.json"));
                //JArray specimenArray = JArray.Parse(crimeJSON);
                // create a collection to hold errors.
                //IList<string> validationEvents = new List<String>();


                //if (specimenArray.IsValid(jSchema, out validationEvents))
                //{
                    Crimes = Crime.FromJson(crimeJSON);
                //}
                //else
                //{
                //    foreach (string evt in validationEvents)
                //    {
                //        Console.WriteLine(evt);
                //    }
                //}

                // Calculate the counts for different time periods
                DateTime now = DateTime.Now;
                CrimeCountLastMonth = Crimes.Count(crime => crime.DateReported >= now.AddMonths(-1));
                CrimeCountLastSixMonths = Crimes.Count(crime => crime.DateReported >= now.AddMonths(-6));
                CrimeCountLastYear = Crimes.Count(crime => crime.DateReported >= now.AddYears(-1));
                CrimeCountLastTwoYears = Crimes.Count(crime => crime.DateReported >= now.AddYears(-2));

                ViewData["crimes"] = Crimes;




            }


        }
    }
}
