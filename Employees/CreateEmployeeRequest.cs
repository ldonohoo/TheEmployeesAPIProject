using System;
using FluentValidation;

namespace TheEmployeeAPI.Employees;


// this is the class used when creating a new employee
// firstname, lastname, and SSN are all here and get/settable
public class CreateEmployeeRequest
{
    public string? FirstName { get; set; }   
    public string? LastName { get; set; }  
    public string? SocialSecurityNumber { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

// this validator inherits from AbstractValidator from FluentValidation!
//    give AbtractValidator the object we are validating:
//      the CreateEmployeeRequest object
public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
        public CreateEmployeeRequestValidator()
    {
        // the rulefor Firstname is that it should not be empty (or null)
        // expression is to select that property
        RuleFor(x => x.FirstName)
            .NotEmpty();
            // can chain stuff here!!
            // .WithMessage("First name is required.");
        // the rulefor Lastname is that it should not be empty (or null)
        RuleFor(x => x.LastName).NotEmpty();
    }
}
