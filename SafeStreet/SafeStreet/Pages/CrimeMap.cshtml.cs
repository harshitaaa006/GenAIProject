using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SafeStreet.Data;
using SafeStreet.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeStreet.Pages
{
    public class CrimeMapModel : PageModel
    {
        private readonly SafeStreetContext _context;

        public CrimeMapModel(SafeStreetContext context)
        {
            _context = context;
        }

        private static readonly HttpClient client = new HttpClient();

        // Dictionary to hold neighborhood and its safety score
        public Dictionary<string, double> NeighborhoodSafetyScores { get; private set; } = new();

        public async Task OnGetAsync()
        {
            // Fetch data from the API
            HttpResponseMessage response = await client.GetAsync("https://data.cincinnati-oh.gov/resource/k59e-2pvf.json");
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonString);

                // Group crimes by neighborhood
                var groupedCrimes = jsonArray
                    .Where(item => item["cpd_neighborhood"] != null && item["offense"] != null)
                    .GroupBy(
                        item => item["cpd_neighborhood"]?.ToString() ?? "Unknown Neighborhood",
                        item => item["offense"]?.ToString() ?? "Unknown Offense"
                    );
                // Fetch average user ratings per neighborhood from the database
                var userRatings = await _context.UserRating
                    .GroupBy(r => r.Neighborhood)
                    .ToDictionaryAsync(g => g.Key, g => g.Average(r => r.SafetyRating));

                // Calculate safety scores for each neighborhood
                NeighborhoodSafetyScores = groupedCrimes
                    .Select(group =>
                    {
                        var neighborhood = group.Key;
                        var offenses = group.ToList();

                        // Default user rating to 100 if no ratings exist for the neighborhood
                        var averageUserRating = userRatings.ContainsKey(neighborhood) ? userRatings[neighborhood] : 100;

                        var safetyScore = CalculateSafetyScore(offenses, averageUserRating);
                        return new { Neighborhood = neighborhood, SafetyScore = safetyScore };
                    })
                    .OrderByDescending(score => score.SafetyScore)
                    .ToDictionary(x => x.Neighborhood, x => x.SafetyScore);
            }
        }
            // Calculate safety score based on offense weights
            private double CalculateSafetyScore(List<string> offenses, double averageUserRating)
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

                var weights = offenses
                     .Where(offense => offenseWeights.ContainsKey(offense))
                     .Select(offense => offenseWeights[offense]);

                double totalWeight = weights.Sum();
                int offenseCount = weights.Count();

                // Objective offense score
                double offenseScore = offenseCount > 0 ? totalWeight / offenseCount : 100;

                // Weighted average: 80% offense score + 20% user ratings
                return (offenseScore * 0.8) + (averageUserRating * 0.2);
            }
        }
    }


           
     
