using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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



            }
        }
    }
}

