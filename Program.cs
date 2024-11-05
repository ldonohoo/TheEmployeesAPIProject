using Microsoft.AspNetCore.Mvc;
using TheEmployeeAPI.Employees;

var builder = WebApplication.CreateBuilder(args);

//simple list for data storage 
var employees = new List<Employee>
{
    new Employee { Id = 1, FirstName = "John", LastName = "Doe", SocialSecurityNumber = "123-45-3445" },
    new Employee { Id = 2, FirstName = "Jane", LastName = "Doe", SocialSecurityNumber = "123-45-3446"  }
};
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
employeeRoute.MapGet(string.Empty, () => {
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
    return Results.Ok(employees.Select(employee => new GetEmployeeResponse {
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
employeeRoute.MapGet("{id:int}", (int id) => {
    var employee = employees.SingleOrDefault(e => e.Id == id);
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
employeeRoute.MapPost(string.Empty, ([FromBody] CreateEmployeeRequest employee) => 
{
    var newEmployee = new Employee {
        Id = employees.Max(e => e.Id) + 1,
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
    employees.Add(newEmployee);
    return Results.Created($"/employees/{newEmployee.Id}", employee);
});

// this route takes an UpdateEmployeeRequest object which doesn't 
// contain firstname, lastname and SSN and maps it to a
// single Employee object so it can update properly
employeeRoute.MapPut("{id:int}", (UpdateEmployeeRequest employee, int id) => 
{
    var existingEmployee = employees.SingleOrDefault(e => e.Id == id);
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
    return Results.Ok(existingEmployee);
});

app.Run();

// this is used to mark the default program class (that is created behind 
//   the scenes) as public (it defaults to internal! to see this you can 
//   convert to a Program.Main style program and look at how it defaults... )
public partial class Program {}