using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeStreet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SafetyScoreController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();

        [HttpGet("scores")]
        public async Task<IActionResult> GetSafetyScores()
        {
            // Fetch data from the API
            HttpResponseMessage response = await client.GetAsync("https://data.cincinnati-oh.gov/resource/k59e-2pvf.json");
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonString);

                // Group crimes by neighborhood
                var groupedCrimes = jsonArray
                    .Where(item => item["community_council_neighborhood"] != null && item["offense"] != null)
                    .GroupBy(
                        item => item["community_council_neighborhood"]?.ToString() ?? "Unknown Neighborhood",
                        item => item["offense"]?.ToString() ?? "Unknown Offense"
                    );

                // Calculate safety scores for each neighborhood
                var neighborhoodSafetyScores = groupedCrimes
                    .Select(group =>
                    {
                        var neighborhood = group.Key; // Neighborhood name
                        var offenses = group.ToList(); // List of offenses for the neighborhood
                        var safetyScore = CalculateSafetyScore(offenses); // Calculate safety score
                        return new { Neighborhood = neighborhood, SafetyScore = safetyScore };
                    })
                    .OrderByDescending(score => score.SafetyScore) // Sort by safety score (highest to lowest)
                    .ToDictionary(x => x.Neighborhood, x => x.SafetyScore);

                return Ok(neighborhoodSafetyScores);
            }
            return StatusCode((int)response.StatusCode, "Failed to fetch crime data.");
        }

        // Calculate safety score based on offense weights
        private double CalculateSafetyScore(List<string> offenses)
        {
            var offenseWeights = new Dictionary<string, int>
            {
                { "MURDER", 1 },
                { "RAPE", 5 },
                { "ABDUCTION", 8 },
                { "AGGRAVATED ROBBERY", 10 },
                { "FELONIOUS ASSAULT", 10 },
                { "AGGRAVATED BURGLARY", 15 },
                { "DOMESTIC VIOLENCE", 15 },
                { "ENDANGERING CHILDREN", 15 },
                { "IMPROPERLY DISCHARGING FIREARM AT/INTO HABITATION/SCHOOL", 10 },
                { "ROBBERY", 20 },
                { "ASSAULT", 30 },
                { "BURGLARY", 30 },
                { "BREAKING AND ENTERING", 65 },
                { "CRIMINAL DAMAGING/ENDANGERING", 60 },
                { "VANDALISM", 70 },
                { "MENACING BY STALKING", 55 },
                { "THEFT", 45 },
                { "AGGRAVATED MENACING", 50 },
                { "DISSEMINATE MATTER HARMFUL TO JUVENILES", 50 },
                { "FAIL COMPLY ORDER/SIGNAL OF PO-ELUDE/FLEE", 50 },
                { "MENACING", 50 },
                { "UNAUTHORIZED USE OF MOTOR VEHICLE", 70 },
                { "TAKING THE IDENTITY OF ANOTHER", 60 },
                { "MISUSE OF CREDIT CARD", 85 },
                { "TELECOMMUNICATIONS FRAUD", 85 },
                { "TELEPHONE HARASSMENT", 80 },
                { "MAKING FALSE ALARMS", 80 },
                { "UNLAWFUL RESTRAINT", 80 },
                { "VIOLATE PROTECTION ORDER/CONSENT AGREEMENT", 80 },
                { "INTERFERENCE WITH CUSTODY", 80 },
                { "INDUCING PANIC", 85 },
                { "PUBLIC INDECENCY", 95 },
                { "CRIMINAL MISCHIEF", 90 },
            };

            // Calculate weights for given offenses
            var weights = offenses
                .Where(offense => offenseWeights.ContainsKey(offense)) // Filter offenses that exist in the dictionary
                .Select(offense => offenseWeights[offense]); // Map offenses to their weights

            // Calculate total weight and offense count
            double totalWeight = weights.Sum();
            int offenseCount = weights.Count();

            // Return average score (100 if no offenses reported)
            return offenseCount > 0 ? totalWeight / offenseCount : 100;
        }
    }
}
