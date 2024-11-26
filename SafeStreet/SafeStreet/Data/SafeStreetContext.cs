using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SafeStreet.Models;

namespace SafeStreet.Data
{
    public class SafeStreetContext : DbContext
    {
        public SafeStreetContext (DbContextOptions<SafeStreetContext> options)
            : base(options)
        {
        }

        public DbSet<SafeStreet.Models.UserRating> UserRating { get; set; } = default!;
    }
}
