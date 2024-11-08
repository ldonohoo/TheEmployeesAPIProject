using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TheEmployeeAPI;
using TheEmployeeAPI.Abstractions;
using TheEmployeeAPI.Employees;

var builder = WebApplication.CreateBuilder(args);

// Add/register services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// we are adding as a singleton here because it's just an in-memory
// database but this will def change and it's not how to do in real
// production systems (because we would have real database!)
// this allows are EmployeeRepository service to be successfully
// injected!!

// so because we may want to test this separately, we typically 
// don't use the specific type EmployeeRepository, instead we
// use the interface type of IRepository<Employee>
// then below in the routes we change the EmployeeRepository types to 
// IRepository<Employee> as well
//builder.Services.AddSingleton<EmployeeRepository>();

// so use interface type not concrete type!!!
// very common to do this:
builder.Services.AddSingleton<IRepository<Employee>, EmployeeRepository>();
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

var app = builder.Build();

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