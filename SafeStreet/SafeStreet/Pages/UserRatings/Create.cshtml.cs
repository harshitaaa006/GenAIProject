using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SafeStreet.Data;
using SafeStreet.Models;
using Newtonsoft.Json.Linq;


namespace SafeStreet.Pages.UserRatings
{
    public class CreateModel : PageModel
    {
        private readonly SafeStreetContext _context;
        private static readonly HttpClient client = new HttpClient();

        public CreateModel(SafeStreetContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserRating UserRating { get; set; } = default!;

        public List<string> Neighborhoods { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch data from JSON endpoint
            var response = await client.GetAsync("https://data.cincinnati-oh.gov/resource/k59e-2pvf.json");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonString);

                // Extract distinct neighborhoods
                Neighborhoods = jsonArray
           .Where(item => item["cpd_neighborhood"] != null) // Ensure the field exists
           .Select(item => item["cpd_neighborhood"]!.ToString()) // Use null-forgiving operator (!)
           .Where(n => !string.IsNullOrWhiteSpace(n)) // Exclude empty strings
           .Distinct()
           .OrderBy(n => n) // Sort alphabetically
           .ToList();

            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload neighborhoods for dropdown in case of error
                await OnGetAsync();
                return Page();
            }

            _context.UserRating.Add(UserRating);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
