var builder = WebApplication.CreateBuilder(args);

//simple list for data storage 
var employees = new List<Employee>
{
    new Employee { Id = 1, FirstName = "John", LastName = "Doe" },
    new Employee { Id = 2, FirstName = "Jane", LastName = "Doe" }
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
    return Results.Ok(employees);
});

// you can constrain id to only be an int! {id:int}
employeeRoute.MapGet("{id:int}", (int id) => {
    var employee = employees.SingleOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(employee);
});

employeeRoute.MapPost(string.Empty, (Employee employee) => 
{
    employee.Id = employees.Max(e => e.Id) + 1;
    employees.Add(employee);
    return Results.Created($"/employees/{employee.Id}", employee);
});

app.Run();

// this is used to mark the default program class (that is created behind 
//   the scenes) as public (it defaults to internal! to see this you can 
//   convert to a Program.Main style program and look at how it defaults... )
public partial class Program {}