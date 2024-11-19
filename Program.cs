using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TheEmployeeAPI;


var builder = WebApplication.CreateBuilder(args);

// Add/register services to the container.
// These services are all being added to the root provider


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// the swagger option is for enabling xml comments
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "TheEmployeeAPI.xml"));
});
// service to return STRUCTURED data describing errors from an API
builder.Services.AddProblemDetails();
// this allows us to request an IValidator<CreateEmployeeRequest> from the 
// DI container and get it: scan Program and look at every type
//   to see if need IValidator
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// this will add controllers to the DI pipeline!  Need to use controllers!
builder.Services.AddControllers(options =>
    {
        // this option makes sure that this filter is part of all of 
        //   our controllers.  without this the fluentValidationFilter is
        //   just a class in the project and will not get used!!
        // this is micro middleware 
        // this will run onActionExecution (before route processes) and
        // onActionExecuted (after route processes) methods in our 
        // FluentValidationFilter class
        options.Filters.Add<FluentValidationFilter>();
    });
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseSqlite("Data Source=employee.db");
    // add option to turn off auto change tracking...
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); 
});


builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Once the app is built, we have access to the services from
//      the dependency injection pipeline!
// We use this to seed the database with test data

// create a temporary scope to access the context
// dispose of the scope when done seeding database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.MigrateAndSeed(services);
}

// create a route group for employee
var employeeRoute = app.MapGroup("employees");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// part that will look for a controller that has the current
//   requested route!
app.MapControllers();
app.UseHttpsRedirection();


app.Run();

// this is used to mark the default program class (that is created behind 
//   the scenes) as public (it defaults to internal! to see this you can 
//   convert to a Program.Main style program and look at how it defaults... )
public partial class Program {}