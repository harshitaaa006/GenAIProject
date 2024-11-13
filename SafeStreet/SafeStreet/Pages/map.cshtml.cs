using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeStreet.Pages
{
    public class mapModel : PageModel
    {
        public string ApiKey { get; private set; }

        public void OnGet()
        {
            // Retrieve the API key from environment variables
            ApiKey = Environment.GetEnvironmentVariable("MAPBOX_API_KEY");
        }
    }
}
