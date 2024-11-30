using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SafeStreet.Models;
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
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public string GoogleMapApiKey { get; private set; }
        public List<Crime> Crimes { get; set; } = new List<Crime>();

        public TestModel(ILogger<TestModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public double SafetyScore { get; private set; } = 100; // Default safety score
        public string SearchNeighborhood { get; private set; } // Property to hold the neighborhood name

        public void OnGet(string neighborhood)
        {
            GoogleMapApiKey = _configuration["GoogleMapApiKey"];

            // Set the neighborhood name if provided
            if (!string.IsNullOrEmpty(neighborhood))
            {
                SearchNeighborhood = neighborhood;
            }
        }


        [HttpGet]
        public async Task<IActionResult> OnGetCrimeStatsNearbyAsync(double latitude, double longitude)
        {
            try
            {
                DateTime oneYearAgo = DateTime.UtcNow.AddYears(-1);

                // Fetch crimes from the API if not already loaded
                if (Crimes == null || !Crimes.Any())
                {
                    Crimes = await FetchCrimesFromApi(oneYearAgo);
                }

                const double SEARCH_RADIUS_KM = 1.0; // 1 km radius
                DateTime now = DateTime.UtcNow;

                // Calculate nearby crimes within the radius
                var nearbyCrimes = Crimes.Where(c =>
                    double.TryParse(c.LatitudeX, out var lat) &&
                    double.TryParse(c.LongitudeX, out var lng) &&
                    CalculateDistance(latitude, longitude, lat, lng) <= SEARCH_RADIUS_KM
                );

                // Summarize crime statistics
                var totalCrimeStats = new
                {
                    Last3Months = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 90),
                    Last6Months = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 180),
                    Last9Months = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 270),
                    Last1Year = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 365)
                };

                // Group crimes by type and calculate counts for each time period
                var crimeTypeStats = nearbyCrimes
                    .GroupBy(c => c.Offense)
                    .Select(g => new
                    {
                        Type = g.Key,
                        Last3Months = g.Count(c => (now - c.DateReported).TotalDays <= 90),
                        Last6Months = g.Count(c => (now - c.DateReported).TotalDays <= 180),
                        Last9Months = g.Count(c => (now - c.DateReported).TotalDays <= 270),
                        Last1Year = g.Count(c => (now - c.DateReported).TotalDays <= 365)
                    })
                    .ToList();

                // Calculate Safety Score
                CalculateSafetyScore(nearbyCrimes);

                // Prepare the response
                var stats = new
                {
                    SafetyScore,
                    TotalCrimeStats = totalCrimeStats,
                    CrimeTypeStats = crimeTypeStats
                };

                _logger.LogInformation($"Stats: {System.Text.Json.JsonSerializer.Serialize(stats)}");
                return new JsonResult(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching or processing nearby crime stats: {ex.Message}");
                return BadRequest("Error processing request.");
            }
        }

        private async Task<List<Crime>> FetchCrimesFromApi(DateTime since)
        {
            var crimes = new List<Crime>();
            int limit = 1000;
            int offset = 0;
            bool moreData = true;

            while (moreData)
            {
                string url = $"https://data.cincinnati-oh.gov/resource/k59e-2pvf.json?$limit={limit}&$offset={offset}&$where=date_reported >= '{since:yyyy-MM-ddT00:00:00}'";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string crimeJson = await response.Content.ReadAsStringAsync();
                    var crimesBatch = Crime.FromJson(crimeJson);

                    if (crimesBatch != null && crimesBatch.Any())
                    {
                        crimes.AddRange(crimesBatch);
                        offset += limit;
                        _logger.LogInformation($"Fetched {crimesBatch.Count} records. Total so far: {crimes.Count}");
                    }
                    else
                    {
                        moreData = false; // No more data available
                    }
                }
                else
                {
                    _logger.LogError($"Failed to fetch crimes: {response.StatusCode}");
                    moreData = false;
                }
            }

            return crimes;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private void CalculateSafetyScore(IEnumerable<Crime> crimes)
        {
            if (crimes == null || !crimes.Any())
            {
                SafetyScore = 100; // No crimes, perfectly safe
                return;
            }

            // Severity categories
            var highSeverityCrimes = new HashSet<string>
    {
        "SEXUAL BATTERY", "AGGRAVATED MENACING", "MURDER", "KIDNAPPING", "AGGRAVATED ROBBERY",
        "AGGRAVATED BURGLARY", "FELONIOUS ASSAULT", "GROSS SEXUAL IMPOSITION",
        "IMPROPERLY DISCHARGING FIREARM AT/INTO HABITATION/SCHOOL", "FAIL COMPLY ORDER/SIGNAL OF PO-ELUDE/FLEE"
    };

            var midSeverityCrimes = new HashSet<string>
    {
        "ASSAULT", "CRIMINAL DAMAGING/ENDANGERING", "THEFT", "BREAKING AND ENTERING",
        "BURGLARY", "SEXUAL IMPOSITION", "ROBBERY", "VANDALISM", "DOMESTIC VIOLENCE",
        "FORGERY", "TAKING THE IDENTITY OF ANOTHER", "UNAUTHORIZED USE OF MOTOR VEHICLE", "MENACING"
    };

            var lowSeverityCrimes = new HashSet<string>
    {
        "MISUSE OF CREDIT CARD", "TELEPHONE HARASSMENT", "TELECOMMUNICATIONS FRAUD", "PASSING BAD CHECKS",
        "CRIMINAL MISCHIEF", "PUBLIC INDECENCY", "MAKING FALSE ALARMS", "EXTORTION",
        "MENACING BY STALKING", "VEHICULAR VANDALISM", "VIOLATE PROTECTION ORDER/CONSENT AGREEMENT",
        "ENDANGERING CHILDREN", "UNAUTHORIZED USE OF PROPERTY"
    };

            // Count crimes by severity
            int highCount = crimes.Count(c => highSeverityCrimes.Contains(c.Offense));
            int midCount = crimes.Count(c => midSeverityCrimes.Contains(c.Offense));
            int lowCount = crimes.Count(c => lowSeverityCrimes.Contains(c.Offense));

            // Calculate Weighted Crime Index
            double weightedCrimeIndex = (highCount * 5) + (midCount * 3) + (lowCount * 1);

            // Calculate Safety Score
            const double crimeThreshold = 6000; // Normalization factor
            SafetyScore = 100 - (int)Math.Floor((weightedCrimeIndex / crimeThreshold) * 100);
            _logger.LogInformation($"SafetyScore: {SafetyScore}, weightedCrimeIndex: {weightedCrimeIndex}");

            // Ensure the score is clamped between 0 and 100
            SafetyScore = Math.Max(0, Math.Min(100, SafetyScore));
        }

    }
}
