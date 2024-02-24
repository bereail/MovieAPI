using Microsoft.EntityFrameworkCore;
using MOVIE_API.Models;

namespace movie_api.Data
{
    public class movieDbContext : DbContext
    {
        public movieDbContext(DbContextOptions<movieDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<BookingDetail> BookingDetails { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }

      

    }
}