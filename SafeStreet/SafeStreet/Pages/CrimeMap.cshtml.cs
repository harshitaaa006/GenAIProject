using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using SafeStreet.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeStreet.Pages
{
    public class CrimeMapModel : PageModel
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<TestModel> _logger;

        public CrimeMapModel(ILogger<TestModel> logger)
        {
            _logger = logger;
        }

        // Dictionary to hold neighborhood and its safety score
        public Dictionary<string, (double Lat, double Lng, double SafetyScore)> NeighborhoodSafetyScores { get; private set; } = new();

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

                // Calculate safety scores for each neighborhood
                NeighborhoodSafetyScores = groupedCrimes
                    .Select(group =>
                    {
                        var neighborhood = group.Key;
                        var offenses = group.ToList();
                        var safetyScore = CalculateSafetyScore(offenses);
                        _logger.LogInformation($"Processing neighborhood: {neighborhood}");

                        var defaultCoordinates = neighborhood switch
                        {
                            "WEST PRICE HILL" => (39.1122194, -84.6295593),
                            "EAST PRICE HILL" => (39.1061415, -84.5837513),
                            "WESTWOOD" => (39.1479188, -84.6370096),
                            "EVANSTON" => (39.1440305, -84.4901171),
                            "MILLVALE" => (39.1442257, -84.5619227),
                            "NORTH AVONDALE" => (39.1576, -84.4869),
                            "QUEENSGATE" => (39.1012, -84.5308),
                            "OVER-THE-RHINE" => (39.1106, -84.5156),
                            "COLLEGE  HILL" => (39.1912, -84.5477),
                            "MADISONVILLE" => (39.1573, -84.3850),
                            "FAY APARTMENTS" => (39.1663, -84.5586),
                            "LOWER PRICE  HILL" => (39.1014, -84.5453),
                            "C. B. D. / RIVERFRONT" => (39.0968, -84.5133),
                            "OAKLEY" => (39.1526, -84.4299),
                            "WEST  END" => (39.1100, -84.5280),
                            "WINTON HILLS" => (39.1862, -84.5250),
                            "MOUNT AIRY" => (39.1868, -84.5583),
                            "SOUTH  FAIRMOUNT" => (39.1231, -84.5475),
                            "CLIFTON" => (39.1426, -84.5197),
                            "MOUNT  AUBURN" => (39.1236, -84.5113),
                            "COLUMBIA / TUSCULUM" => (39.1192, -84.4316),
                            "CORRYVILLE" => (39.1290, -84.5073),
                            "ENGLISH  WOODS" => (39.1342, -84.5481),
                            "EAST WALNUT HILLS" => (39.1270, -84.4800),
                            "BONDHILL" => (39.1762, -84.4805),
                            "SPRING GROVE VILLAGE" => (39.1620, -84.5300),
                            "ROSELAWN" => (39.1886, -84.4563),
                            "AVONDALE" => (39.1478, -84.4949),
                            "NORTHSIDE" => (39.1664, -84.5427),
                            "PENDLETON" => (39.1100, -84.5070),
                            "S.. CUMMINSVILLE" => (39.1535547, -84.5618757),
                            "RIVERSIDE" => (39.0931, -84.5938),
                            "WALNUT HILLS" => (39.1270, -84.4800),
                            "MT.  LOOKOUT" => (39.1279198, -84.4297732),
                            "CAMP  WASHINGTON" => (39.1236, -84.5361),
                            "FAIRVIEW" => (39.1270, -84.5300),
                            "EAST  WESTWOOD" => (39.1550, -84.5760),
                            "PLEASANT RIDGE" => (39.1840, -84.4290),
                            "HARTWELL" => (39.2150, -84.4740),
                            "MT.  WASHINGTON" => (39.0870, -84.3830),
                            "PADDOCK  HILLS" => (39.1760, -84.4800),
                            "CLIFTON/UNIVERSITY HEIGHTS" => (39.1426, -84.5197),
                            "CARTHAGE" => (39.1880, -84.4800),
                            "NORTH FAIRMOUNT" => (39.1340, -84.5450),
                            "HYDE PARK" => (39.1370, -84.4470),
                            "KENNEDY  HEIGHTS" => (39.1910, -84.4200),
                            "SEDAMSVILLE" => (39.1040, -84.5580),
                            "EAST  END" => (39.1190, -84.4310),
                            "SAYLER  PARK" => (39.0950, -84.6600),
                            "CALIFORNIA" => (39.0780, -84.3880),
                            "LINWOOD" => (39.1190, -84.4310),
                            "MOUNT  ADAMS" => (39.1070, -84.4970)
                        };

                        return new
                        {
                            Neighborhood = neighborhood,
                            Data = (Lat: defaultCoordinates.Item1, Lng: defaultCoordinates.Item2, SafetyScore: safetyScore)
                        };
                    })
                    .OrderByDescending(entry => entry.Data.SafetyScore) // Sort by SafetyScore (highest to lowest)
                    .ToDictionary(entry => entry.Neighborhood, entry => entry.Data);

                    ViewData["NeighborhoodSafetyScores"] = NeighborhoodSafetyScores;

            }
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


