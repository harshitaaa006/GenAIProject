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

        public List<Crime> Crimes { get; set; } = new List<Crime>();
        public int CrimeCountLastMonth { get; set; }
        public int CrimeCountLastSixMonths { get; set; }
        public int CrimeCountLastYear { get; set; }
        public int CrimeCountLastTwoYears { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                int limit = 1000;
                int offset = 0;
                int totalRecordsToFetch = 60000;
                bool moreData = true;

                while (moreData)
                {
                    string url = $"https://data.cincinnati-oh.gov/resource/k59e-2pvf.json?$limit={limit}&$offset={offset}";
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string crimeJson = await response.Content.ReadAsStringAsync();
                        var crimesBatch = Crime.FromJson(crimeJson);

                        if (crimesBatch != null && crimesBatch.Any())
                        {
                            Crimes.AddRange(crimesBatch);
                            offset += limit;

                            // Log success for each iteration of the loop
                            _logger.LogInformation($"Successfully fetched {crimesBatch.Count} records, current total: {Crimes.Count}, offset: {offset}");

                            // Stop if we have fetched enough records
                            if (Crimes.Count >= totalRecordsToFetch)
                            {
                                _logger.LogInformation("Reached the target of 60,000 records. Stopping data fetch.");
                                moreData = false;
                            }
                        }
                        else
                        {
                            moreData = false; // No more data to fetch
                        }
                    }
                    else
                    {
                        _logger.LogError($"Failed to fetch data: {response.StatusCode}");
                        moreData = false; // Stop if an error occurs
                    }
                }

                // Calculate the counts for different time periods
                DateTime now = DateTime.Now;
                CrimeCountLastMonth = Crimes.Count(crime => crime.DateReported >= now.AddMonths(-1));
                CrimeCountLastSixMonths = Crimes.Count(crime => crime.DateReported >= now.AddMonths(-6));
                CrimeCountLastYear = Crimes.Count(crime => crime.DateReported >= now.AddYears(-1));
                CrimeCountLastTwoYears = Crimes.Count(crime => crime.DateReported >= now.AddYears(-2));

                // Pass crimes to the Razor page through ViewData, if needed
                ViewData["crimes"] = Crimes;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching or processing data: {ex.Message}");
            }
        }
    }
}
