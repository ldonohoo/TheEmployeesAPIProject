using System;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TheEmployeeAPI.Employees;

// when making projects and want to add custom behavior to quickly convert
// one thing to another, USE extension methods!!

// this extension spins through the list of validation results returned
//   by TryValidateObject, and puts it in the format of ProblemDetails,
//   which is the accepted standard for API request problems

public static class Extensions
{
    // model state is an object that says whether a field is valid/invalid/etc.
    // model state you will see used with MVC with views applications (not
    // covered in this course) )
    public static ModelStateDictionary ToModelStateDictionary(this ValidationResult validationResult)
    {
        var modelState = new ModelStateDictionary();

        foreach (var error in validationResult.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return modelState;
    }

}
