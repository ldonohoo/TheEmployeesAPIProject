using System;
using FluentValidation;
using TheEmployeeAPI.Abstractions;

namespace TheEmployeeAPI.Employees;

// the class to update an employee, notice you can no longer change
// first and last names!
public class UpdateEmployeeRequest
{
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

// create a validator method for UpdateEmployeeRequest
// implements AbstractValidator from Fluent validation, passing the Retrieval 
// object UpdateEmployeeRequest
public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    private readonly HttpContext _httpContext;
    private readonly IRepository<Employee> _repository;

    // the constructor pulls in:
    // - httpContextAccesor passed in so we can pull context info from the Http
    //      request like id to update!!  context accessor is part of DI system
    //          - you need to register this in the DI container!!!
    // - the repository is passed in so we have access to the database to do 
    //          specific validation on the existence of an object!
    public UpdateEmployeeRequestValidator(IHttpContextAccessor httpContextAccessor, IRepository<Employee> repository)
    {
        this._httpContext = httpContextAccessor.HttpContext!;
        this._repository = repository;

        // check to see if address is already set for employee.  if it is,
        // don't let them blank it out (only let them change it!)
        //  - uses the MustAsync method which runs a task and returns true/false
        //       MustAsync looks like it passes the address???
        // - if true it returns message with method WithMessage???
        RuleFor(x => x.Address1)
            .MustAsync(NotBeEmptyIfItIsSetOnEmployeeAlreadyAsync)
            .WithMessage("Address1 must not be empty as an address was already set for the employee.");
    }

    private async Task<bool> NotBeEmptyIfItIsSetOnEmployeeAlreadyAsync(string? address, CancellationToken token)
    {
        await Task.CompletedTask;   //again, we'll not make this async for now!

        // access the id from the current request context!!
        var id = Convert.ToInt32(_httpContext.Request.RouteValues["id"]);
        var employee = _repository.GetById(id);
        if (employee == null)
            {
                return false;
            }
        if (employee!.Address1 != null && string.IsNullOrWhiteSpace(address))
        {
            return false;
        }

        return true;
    }
}