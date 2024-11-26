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
    public class DetailsModel : PageModel
    {
        private readonly SafeStreet.Data.SafeStreetContext _context;

        public DetailsModel(SafeStreet.Data.SafeStreetContext context)
        {
            _context = context;
        }

        public UserRating UserRating { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrating = await _context.UserRating.FirstOrDefaultAsync(m => m.Id == id);
            if (userrating == null)
            {
                return NotFound();
            }
            else
            {
                UserRating = userrating;
            }
            return Page();
        }
    }
}
