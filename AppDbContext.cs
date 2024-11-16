using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TheEmployeeAPI;

// inherit from DbContext from Entity Framework!!!
public class AppDbContext : DbContext
{
    // constructor for your DbContext!
    // this will call the constructor for the BASE class (which is DbContext)
    // with the options passed in.
    //   - calling the constructor on the base class DbContext with the given
    //     options witll make sure the connection string for the database is 
    //     set up, etc.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    {

    }
    // declare a table with DbSet!
    // collection of Employee entities mapped to employees table in the database!
    public DbSet<Employee> Employees { get; set; }
}