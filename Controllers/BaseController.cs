using System;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace TheEmployeeAPI;

// always use api controller
[ApiController]
// always name route group based off of controller class name
[Route("[controller]")]
// this attribute sets the media type for the api in the metadata!
[Produces("application/json")]
public class BaseController : Controller 
{
    // this Validate Async method will take in a type to validate
    // and an instance of that type
    //
    // if it doesn't find a validator for that type it will throw an error!
    //
    // otherwise, it will create a new validation context and use it to
    // call ValidateAsync to validate the type asynchronously!

    //note: because ValidationResult is a class in multiple libraries (both
    // .NET and third party like fluent) we need to verify we are importing
    // ValidationResult from the right namespace, FluentValidation!
    // protected async Task<ValidationResult> ValidateAsync<T>(T instance)
    // {
    //     var validator = HttpContext.RequestServices.GetService<IValidator<T>>();
    //     if (validator == null)
    //     {
    //         throw new ArgumentException($"No validator found for {typeof(T).Name}");
    //     }
    //     // don't need to explicitly create a validation context, you 
    //     // can instead just pass instance into ValidateAsync...
    //     // var validationContext = new ValidationContext<T>(instance);
    //     // var result = await validator.ValidateAsync(validationContext);

    //     // here instance is an instance of a validator class implementing
    //     // FluentValidation
    //     // this ValidateAsync method will run validation checks on the instance
    //     // passed into ValidateAsync!
    //     var result = await validator.ValidateAsync(instance);

    //     return result;
    // }
}
