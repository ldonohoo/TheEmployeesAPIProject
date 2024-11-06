using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TheEmployeeAPI.Employees;

// when making projects and want to add custom behavior to quickly convert
// one thing to another, USE extension methods!!

// this extension spins through the list of validation results returned
//   by TryValidateObject, and puts it in the format of ProblemDetails,
//   which is the accepted standard for API request problems

public static class Extensions
{
    public static ValidationProblemDetails ToValidationProblemDetails(this List<ValidationResult> validationResults)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ValidationProblemDetails();

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                if (problemDetails.Errors.ContainsKey(memberName))
                {
                    problemDetails.Errors[memberName] = problemDetails.Errors[memberName].Concat([validationResult.ErrorMessage]).ToArray()!;
                }
                else
                {
                    problemDetails.Errors[memberName] = new List<string> { validationResult.ErrorMessage! }.ToArray();
                }
            }
        }

        return problemDetails;
    }
}