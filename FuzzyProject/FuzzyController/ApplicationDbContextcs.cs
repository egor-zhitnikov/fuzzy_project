using FuzzyController.Models;
using Microsoft.EntityFrameworkCore;

namespace FuzzyController
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<APIrequest> APIrequests { get; set; }
        public DbSet<RestaurantRating> RestaurantRatings { get; set; }
    }
}
