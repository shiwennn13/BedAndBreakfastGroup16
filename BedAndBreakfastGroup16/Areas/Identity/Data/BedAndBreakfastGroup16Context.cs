using BedAndBreakfastGroup16.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BedAndBreakfastGroup16.Data;

public class BedAndBreakfastGroup16Context : IdentityDbContext<BedAndBreakfastGroup16User>
{
    public BedAndBreakfastGroup16Context(DbContextOptions<BedAndBreakfastGroup16Context> options)
        : base(options)
    {
    }

    public DbSet<BedAndBreakfastGroup16.Models.Rooms> RoomsTable { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
