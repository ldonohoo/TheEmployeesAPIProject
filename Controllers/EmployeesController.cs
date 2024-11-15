using System;

using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheEmployeeAPI.Abstractions;
using TheEmployeeAPI.Employees;

namespace TheEmployeeAPI.Controllers;

public class EmployeesController : BaseController
{
    // first, import our abstractions:
    //  - our repository
    private readonly  IRepository<Employee> _repository;
    private readonly ILogger<EmployeesController> _logger;

    // - our logger
    // ...you want to inject ILogger in the constructor of the Controller
    // and pass the controller type into it...
    public EmployeesController(IRepository<Employee> repository,
                            ILogger<EmployeesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary> Gets All Employees </summary>
    /// <returns> Returns an array of all employees. </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetEmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetAllEmployees()
    {
    return Ok(_repository.GetAll().Select(employee => EmployeeToGetEmployeeResponse(employee)));

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
    public IActionResult GetEmployeeById(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(EmployeeToGetEmployeeResponse(employee));

    }
    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="employeeRequest">The employee to be created.</param>
    /// <returns>A link to the employee that was created.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetEmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest employeeRequest)
    {
        await Task.CompletedTask;

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

        _repository.Create(newEmployee);
        // location header with access sent back!
        _logger.LogInformation("Employee {EmployeeId} successfully created!", newEmployee.Id);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.Id }, newEmployee);
    }

    /// <summary>
    /// Updates an employee.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="employeeRequest">The employee data to update.</param>
    /// <returns></returns>
    [ProducesResponseType(typeof(GetEmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("{id}")]
    public IActionResult UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest employeeRequest)
    {
        // the placeholder format {EmployeeId} is important to be consistant 
        //   because you will be able to SEARCH on this name in the logs!!!!
        _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);
        var existingEmployee = _repository.GetById(id);
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
            _repository.Update(existingEmployee);
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

    /// <summary>
    /// Gets the benefits for an employee.
    /// </summary>
    /// <param name="employeeId">The ID to get the benefits for.</param>
    /// <returns>The benefits for that employee.</returns>
    [HttpGet("{employeeId}/benefits")]
    [ProducesResponseType(typeof(IEnumerable<GetEmployeeResponseEmployeeBenefit>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetBenefitsForEmployee(int employeeId)
    {
        var employee = _repository.GetById(employeeId);
        if (employee == null)
        {
            return NotFound();
        }
        var responseBenefits = employee.Benefits.Select(BenefitToBenefitResponse);
        // the above code is syntactic sugar for below, and called method group
        // syntax...
        // var responseBenefits = employee.Benefits.Select(benefit => BenefitToBenefitResponse(benefit));
        return Ok(responseBenefits);
    }

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
    private static GetEmployeeResponseEmployeeBenefit BenefitToBenefitResponse(EmployeeBenefit benefit)
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


  