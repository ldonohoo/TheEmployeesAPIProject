using Microsoft.AspNetCore.Mvc;
using TheEmployeeAPI;
using TheEmployeeAPI.Abstractions;
using TheEmployeeAPI.Employees;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

var app = builder.Build();

// create a route group for employee
var employeeRoute = app.MapGroup("employees");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Manual way of getting context out of the request here:
//   *** this is called the request delegate pattern!!!
// app.MapGet("/employees", (HttpContext context) => {
//     context.Request...
//     // once you get the context, you can actually 
//     // read the items and the headers direcly by 
//     // manually by pulling aparth the context object!
//     // we won't be doing it that way however
// });
//      Instead we are just going to use a plain old delegate pattern!!!

// add the repository through dependency injection!
employeeRoute.MapGet(string.Empty, ([FromServices] IRepository<Employee> repo) => {
    // return employees; //this totally works, but you can also
    // return with a status code attached
    // a return with a status code is very explicit
    // USE the Results class for returns!!

    // now, we use the Select to iterate through the employees 
    // and for each employee map the employee fields to a
    // new instanceof GetEmployeeResponse...
    // Select is for projection and it is creating a new object here

    // this route returns a GetEmployeeResponse type object
    // that does not include SSN
    return Results.Ok(repo.GetAll().Select(employee => new GetEmployeeResponse {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    }));
});

// you can constrain id to only be an int! {id:int}

// this route takes in an integer, gets an employee of type
// Employee, and maps it to a new GetEmployeeResponse object
// that doesn't contain the SSN
employeeRoute.MapGet("{id:int}", ([FromServices] IRepository<Employee> repo, 
                                  [FromRoute] int id) => {
    var employee = repo.GetById(id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new GetEmployeeResponse {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    });
});

// this route now takes in the CreateEmployeeRequest type object
// (which has all the fields) and creates a new Employee type object
// object to store in database
employeeRoute.MapPost(string.Empty, ([FromBody] CreateEmployeeRequest employee,
                                     [FromServices] IRepository<Employee> repo) => 
{
    var newEmployee = new Employee {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        SocialSecurityNumber = employee.SocialSecurityNumber,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    };
    repo.Create(newEmployee);
    return Results.Created($"/employees/{newEmployee.Id}", employee);
});

// this route takes an UpdateEmployeeRequest object which doesn't 
// contain firstname, lastname and SSN and maps it to a
// single Employee object so it can update properly
employeeRoute.MapPut("{id:int}", ([FromBody] UpdateEmployeeRequest employee, 
                                  [FromRoute] int id,
                                  [FromServices] IRepository<Employee> repo) => 
{
    var existingEmployee = repo.GetById(id);
    if (existingEmployee == null)
    {
        return Results.NotFound();
    }
    existingEmployee.Address1 = employee.Address1;
    existingEmployee.Address2 = employee.Address2;
    existingEmployee.City = employee.City;
    existingEmployee.State = employee.State;
    existingEmployee.ZipCode = employee.ZipCode;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.Email = employee.Email; 

    repo.Update(existingEmployee);
    return Results.Ok(existingEmployee);
});

app.Run();

// this is used to mark the default program class (that is created behind 
//   the scenes) as public (it defaults to internal! to see this you can 
//   convert to a Program.Main style program and look at how it defaults... )
public partial class Program {}