using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SafeStreet.Data;
using SafeStreet.Models;

namespace SafeStreet.Pages.UserRatings
{
    public class IndexModel : PageModel
    {
        private readonly SafeStreet.Data.SafeStreetContext _context;

        public IndexModel(SafeStreet.Data.SafeStreetContext context)
        {
            _context = context;
        }

        public IList<UserRating> UserRating { get;set; } = default!;

        public async Task OnGetAsync()
        {
            UserRating = await _context.UserRating.ToListAsync();
        }
    }
}
