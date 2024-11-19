using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheEmployeeAPI.Employees;

namespace TheEmployeeAPI.Controllers;

public class EmployeesController : BaseController
{
    private readonly ILogger<EmployeesController> _logger;
    private readonly AppDbContext _dbContext;

    // - our logger
    // ...you want to inject ILogger in the constructor of the Controller
    // and pass the controller type into it...
    public EmployeesController(ILogger<EmployeesController> logger,
                               AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary> Gets All Employees </summary>
    /// <returns> Returns an array of all employees. </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetEmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllEmployees([FromQuery] GetAllEmployeesRequest request)
    {
        // ?? is null coalescing operator! provide a default if null...
        int page = request?.Page ?? 1;
        int numberOfRecords = request?.RecordsPerPage ?? 100;

        // instead of getting the dbSet, go for the broader IQueryable...
        //    because we are making a query..not just returning the employees
        // skip is used to skip to the appropriate starting record
        //     (if page is 1 we don't skip anything...)
        IQueryable<Employee> query = _dbContext.Employees
            .Include(e => e.Benefits)
            .Skip((page - 1) * numberOfRecords)
            .Take(numberOfRecords);

        if (request != null)
        {
            if (!string.IsNullOrWhiteSpace(request.FirstNameContains))
            {
                query = query.Where(e => e.FirstName.Contains(request.FirstNameContains));
            }
            if (!string.IsNullOrWhiteSpace(request.LastNameContains))
            {
                query = query.Where(e => e.LastName.Contains(request.LastNameContains));
            }
        }
      
        // run the query async and chuck it into an array!!!
        var employees = await query.ToArrayAsync();

        return Ok(employees.Select(EmployeeToGetEmployeeResponse));
    }

    /// <summary> Get a single employee
    ///           by Id number
    /// </summary>
    /// <param name="id"> The id of the employee </param>
    /// <returns>  Returns a single employee record. </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetEmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        // use singleordefault instead of single because this will return a 
        // null if not found and we want to return a not found status if this
        // is the case!
        //  -- there is also a FindAsync method that could be used just as well
        //      you just need to pass it the key!
        //                 _dbContext.Employees.FindAsync(id);
        // note: singleordefault inherits from iQueryable instead of IEnumerable
        //        because it automatically narrows this because dbContext has
        //        an its own extension method using IQueryable
        // note: notice singleordefault here takes an expression, but we are clearly
        // passing a lambda???  This is because of a property of c# called
        // homoiconicity!!  
        // Homoiconicity means syntax you use to define the data structure 
        // of your functions also can just define the functions!!????
        var employee = await _dbContext.Employees.SingleOrDefaultAsync(e => e.Id == id);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(EmployeeToGetEmployeeResponse(employee));
    }

    // /// <summary>
    // /// Creates a new employee.
    // /// </summary>
    // /// <param name="employeeRequest">The employee to be created.</param>
    // /// <returns>A link to the employee that was created.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetEmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest employeeRequest)
    {
        var newEmployee = new Employee
        {
            FirstName = employeeRequest.FirstName!,
            LastName = employeeRequest.LastName!,
            SocialSecurityNumber = employeeRequest.SocialSecurityNumber,
            Address1 = employeeRequest.Address1,
            Address2 = employeeRequest.Address2,
            City = employeeRequest.City,
            State = employeeRequest.State,
            ZipCode = employeeRequest.ZipCode,
            PhoneNumber = employeeRequest.PhoneNumber,
            Email = employeeRequest.Email
        };

        // OR _dbContext.Add(newEmployee)....
        _dbContext.Employees.Add(newEmployee);
        await _dbContext.SaveChangesAsync();
        // location header with access sent back!
        _logger.LogInformation("Employee {EmployeeId} successfully created!", newEmployee.Id);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.Id }, newEmployee);
    }

    // /// <summary>
    // /// Updates an employee.
    // /// </summary>
    // /// <param name="id">The ID of the employee to update.</param>
    // /// <param name="employeeRequest">The employee data to update.</param>
    // /// <returns></returns>
    [ProducesResponseType(typeof(GetEmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] UpdateEmployeeRequest employeeRequest)
    {
        // the placeholder format {EmployeeId} is important to be consistant 
        //   because you will be able to SEARCH on this name in the logs!!!!
        _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

        var existingEmployee = await _dbContext.Employees.FindAsync(id);
        if (existingEmployee == null)
        {
            _logger.LogWarning("Employee with ID: {EmployeeID} not found", id);
            return NotFound();
        }
        _logger.LogDebug("Updating info for employee with ID: {EmployeeId}", id);
        existingEmployee.Address1 = employeeRequest.Address1;
        existingEmployee.Address2 = employeeRequest.Address2;
        existingEmployee.City = employeeRequest.City;
        existingEmployee.State = employeeRequest.State;
        existingEmployee.ZipCode = employeeRequest.ZipCode;
        existingEmployee.PhoneNumber = employeeRequest.PhoneNumber;
        existingEmployee.Email = employeeRequest.Email;

        try
        {
            // need to track this object now that tracking is off!
            _dbContext.Entry(existingEmployee).State = EntityState.Modified;
    
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Employee with ID: {EmployeeId} successfully updated", id);
            return Ok(existingEmployee);
        }
        // added exception handling on the update!!
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
            return StatusCode(500, "An error occurred while updating the employee");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _dbContext.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // /// <summary>
    // /// Gets the benefits for an employee.
    // /// </summary>
    // /// <param name="employeeId">The ID to get the benefits for.</param>
    // /// <returns>The benefits for that employee.</returns>
    // [HttpGet("{employeeId}/benefits")]
    // [ProducesResponseType(typeof(IEnumerable<GetEmployeeResponseEmployeeBenefit>), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // public IActionResult GetBenefitsForEmployee(int employeeId)
    // {
    //     var employee = _repository.GetById(employeeId);
    //     if (employee == null)
    //     {
    //         return NotFound();
    //     }
    //     var responseBenefits = employee.Benefits.Select(BenefitToBenefitResponse);
    //     // the above code is syntactic sugar for below, and called method group
    //     // syntax...
    //     // var responseBenefits = employee.Benefits.Select(benefit => BenefitToBenefitResponse(benefit));
    //     return Ok(responseBenefits);
    // }

    // quick way of mapping an employee to an employee response object for our
    // get routes.
    private static GetEmployeeResponse EmployeeToGetEmployeeResponse(Employee employee)
    {
        return new GetEmployeeResponse
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Address1 = employee.Address1,
            Address2 = employee.Address2,
            City = employee.City,
            State = employee.State,
            ZipCode = employee.ZipCode,
            PhoneNumber = employee.PhoneNumber,
            Email = employee.Email,
            // go through the benefits collection
            //   - for each benefit, create a new employee benefit response object
            //   - slap all of these new objects into a new list!
            Benefits = employee.Benefits.Select(benefit => new GetEmployeeResponseEmployeeBenefit
            {
                Id = benefit.Id,
                EmployeeId = benefit.EmployeeId,
                BenefitType = benefit.BenefitType,
                Cost = benefit.Cost
            }).ToList()
        };
    }

    // this just maps a benefit entity to a benifit response object so we don't 
    // return the entity object!
    private static GetEmployeeResponseEmployeeBenefit BenefitToBenefitResponse(EmployeeBenefits benefit)
{
    return new GetEmployeeResponseEmployeeBenefit
    {
        Id = benefit.Id,
        EmployeeId = benefit.EmployeeId,
        BenefitType = benefit.BenefitType,
        Cost = benefit.Cost
    };
}
}


  