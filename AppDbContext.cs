using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Internal;
using TheEmployeeAPI.Migrations;

namespace TheEmployeeAPI;

// inherit from DbContext from Entity Framework!!!
public class AppDbContext : DbContext
{

    private readonly ISystemClock _systemClock;

    // constructor for your DbContext!
    // this will call the constructor for the BASE class (which is DbContext)
    // with the options passed in.
    //   - calling the constructor on the base class DbContext with the given
    //     options witll make sure the connection string for the database is 
    //     set up, etc.
    public AppDbContext(
                 DbContextOptions<AppDbContext> options,
                 ISystemClock systemClock) : base(options) 
    {
        _systemClock = systemClock;
    }
    // declare a table with DbSet!
    // collection of Employee entities mapped to employees table in the database!
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Benefit> Benefits { get; set; } = null!;
    public DbSet<EmployeeBenefit> EmployeeBenefits { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //create a unique constraint on EmployeeId and BenefitId
        // this creates a unique index for EmployeeBenefit 
        //    ( the join table between Employee and Benefit )
        //   consisting of an object with both benefit & employee ids
        //   and telling the model it is a unique index
        //
        // ADVANTAGES to doing this way::
        // - this combined index will prevent duplicate entries and
        // maintain the integrity of the many to many relationship
        // - also can use to speed up query performance, locate pair
        //   efficiently
        // - super quick validation to check if pair already exists!
        //   prevent duplicate inserts due to race conditions
        // - locate pair quickly for update and delete too
        // USE A UNIQUE COMBINED INDEX if:
        //   - pair of ids much always be unique
        //   - you frequently query using both columns
        //   - you want to enforce data integrity at database level
        // WARNING:
        //  - possible insert/update overhead enforcing the index
        //  - make sure join table doesn't have OTHER fields that
        //    need separate uniqueness constraints??
        modelBuilder.Entity<EmployeeBenefit>()
            .HasIndex(eb => new {eb.BenefitId, eb.EmployeeId})
            .IsUnique();
    }
        public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = "TheCreateUser";
                // this will use system clock to pull in the datetime
                //   so it can be overridden during testing and pull
                //   in a specific datetime
                entry.Entity.CreatedOn = _systemClock.UtcNow.UtcDateTime;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedBy = "TheUpdateUser";
                entry.Entity.LastModifiedOn = _systemClock.UtcNow.UtcDateTime;
            }
        }
    }
}
