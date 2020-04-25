using Microsoft.EntityFrameworkCore;

namespace massage.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions options) : base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<PAvailTime> PAvailTimes { get; set; }
        public DbSet<PInsurance> PInsurances { get; set; }
        public DbSet<PSchedule> PSchedules { get; set; }
        public DbSet<PService> PServices { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomService> RoomServices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Timeslot> Timeslots { get; set; }
        
    }
}