using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SafeStreet.Data;
using SafeStreet.Models;

namespace SafeStreet.Pages.UserRatings
{
    public class EditModel : PageModel
    {
        private readonly SafeStreet.Data.SafeStreetContext _context;

        public EditModel(SafeStreet.Data.SafeStreetContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserRating UserRating { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrating =  await _context.UserRating.FirstOrDefaultAsync(m => m.Id == id);
            if (userrating == null)
            {
                return NotFound();
            }
            UserRating = userrating;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(UserRating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRatingExists(UserRating.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserRatingExists(int id)
        {
            return _context.UserRating.Any(e => e.Id == id);
        }
    }
}
