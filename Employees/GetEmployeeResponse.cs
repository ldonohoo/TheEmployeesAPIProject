using System;

namespace TheEmployeeAPI.Employees;

// this is the class used when creating a new employee
// firstname, lastname, and SSN are all here and get/settable
public class CreateEmployeeRequest
{
    public required string FirstName { get; set; }   
    public required string LastName { get; set; }  
    public string? SocialSecurityNumber { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

// the respone of the employe, doesn't include SSN
public class GetEmployeeResponse
{
    public required string FirstName { get; set; }   
    public required string LastName { get; set; }  
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

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
