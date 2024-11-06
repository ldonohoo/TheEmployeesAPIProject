using System;
using System.ComponentModel.DataAnnotations;

namespace TheEmployeeAPI.Employees;


// this is the class used when creating a new employee
// firstname, lastname, and SSN are all here and get/settable
public class CreateEmployeeRequest
{
    [Required(AllowEmptyStrings = false)]
    // we are going to remove required right in front of 
    // string and instead make string
    // a nullable string!!!  why??????
    // we want our API caller to:
    //   - pass in an empty string
    //   - have our validator handle it 
    // If these aren't nullable, you can't have 
    // validation logic returning a nice error message
    // to the caller saying you can't have it null.
    // Need to get past this point so you can handle
    // the null error.

    // so, the incoming create employee request
    // will accept nulls in firstname lastname
    // just long enough to give the user requestor
    // better data back!
    public string? FirstName { get; set; }   
    [Required(AllowEmptyStrings = false)]
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

