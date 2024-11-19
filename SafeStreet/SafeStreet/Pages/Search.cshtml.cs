using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeStreet.Pages
{
    public class SearchModel : PageModel
    {
        //I Updated the model (SearchModel) to include a property for the query string, This binds the input value directly to the Query property, reducing manual handling.
        [BindProperty(SupportsGet = true)]
        public string Query { get; set; }
    }
}
