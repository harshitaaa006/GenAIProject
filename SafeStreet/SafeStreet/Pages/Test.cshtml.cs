using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeStreet.Pages
{
    public class TestModel : PageModel
    {
        private readonly ILogger<TestModel> _logger;
        HttpClient _httpClient = new HttpClient();

        static readonly HttpClient client = new HttpClient();

        public TestModel(ILogger<TestModel> logger)
        {
            _logger = logger;
        }

        public List<Crime> Crimes { get; set; } = new List<Crime>();
        public int CrimeCountLastMonth { get; set; }
        public int CrimeCountLastSixMonths { get; set; }
        public int CrimeCountLastYear { get; set; }
        public int CrimeCountLastTwoYears { get; set; }
        public int NearbyCrimeCountLastMonth { get; set; }
        public int NearbyCrimeCountLastSixMonths { get; set; }
        public int NearbyCrimeCountLastYear { get; set; }
        public int NearbyCrimeCountLastTwoYears { get; set; }

        public async Task OnGetAsync(double? latitude, double? longitude)
        {
            try
            {
                int limit = 1000;
                int offset = 0;
                bool moreData = true;

                DateTime twoYearsAgo = DateTime.Now.AddYears(-2);

                // Fetch crimes from the API within the past 2 years using pagination
                while (moreData)
                {
                    string url = $"https://data.cincinnati-oh.gov/resource/k59e-2pvf.json?$limit={limit}&$offset={offset}&$where=date_reported >= '{twoYearsAgo:yyyy-MM-ddT00:00:00}'";
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
                        }
                        else
                        {
                            moreData = false; // No more data to fetch
                            _logger.LogInformation("No more data to fetch, stopping the loop.");
                        }
                    }
                    else
                    {
                        _logger.LogError($"Failed to fetch data: {response.StatusCode}");
                        moreData = false; // Stop if an error occurs
                    }
                }


                if (latitude.HasValue && longitude.HasValue)
                {
                    const double SEARCH_RADIUS = 1.0;

                    // Filter crimes reported in the past time periods and within the radius
                    DateTime now = DateTime.Now;
                    NearbyCrimeCountLastMonth = Crimes
                        .Where(crime => crime.DateReported >= now.AddMonths(-1) && IsWithinRadius(latitude.Value, longitude.Value, crime.LatitudeX, crime.LongitudeX, SEARCH_RADIUS))
                        .Count();

                    NearbyCrimeCountLastSixMonths = Crimes
                        .Where(crime => crime.DateReported >= now.AddMonths(-6) && IsWithinRadius(latitude.Value, longitude.Value, crime.LatitudeX, crime.LongitudeX, SEARCH_RADIUS))
                        .Count();

                    NearbyCrimeCountLastYear = Crimes
                        .Where(crime => crime.DateReported >= now.AddYears(-1) && IsWithinRadius(latitude.Value, longitude.Value, crime.LatitudeX, crime.LongitudeX, SEARCH_RADIUS))
                        .Count();

                    NearbyCrimeCountLastTwoYears = Crimes
                        .Where(crime => crime.DateReported >= now.AddYears(-2) && IsWithinRadius(latitude.Value, longitude.Value, crime.LatitudeX, crime.LongitudeX, SEARCH_RADIUS))
                        .Count();
                }

                // Calculate the counts for different time periods
                DateTime currentTime = DateTime.Now;
                CrimeCountLastMonth = Crimes.Count(crime => crime.DateReported >= currentTime.AddMonths(-1));
                CrimeCountLastSixMonths = Crimes.Count(crime => crime.DateReported >= currentTime.AddMonths(-6));
                CrimeCountLastYear = Crimes.Count(crime => crime.DateReported >= currentTime.AddYears(-1));
                CrimeCountLastTwoYears = Crimes.Count(crime => crime.DateReported >= currentTime.AddYears(-2));

                // Pass crimes to the Razor page through ViewData, if needed
                ViewData["crimes"] = Crimes;

                // Log the final success of fetching all data
                _logger.LogInformation($"Successfully completed fetching data. Total records fetched: {Crimes.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching or processing data: {ex.Message}");
            }
        }

        // Helper method to determine if a crime is within the specified radius

        private bool IsWithinRadius(double userLat, double userLon, string crimeLatStr, string crimeLonStr, double radiusKm)
        {
            if (double.TryParse(crimeLatStr, out double crimeLat) && double.TryParse(crimeLonStr, out double crimeLon))
            {
                double distance = CalculateDistance(userLat, userLon, crimeLat, crimeLon);
                return distance <= radiusKm;
            }
            return false;
        }

        // Haversine formula to calculate the distance between two coordinates in kilometers

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c; // Distance in km
        }

        private double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
