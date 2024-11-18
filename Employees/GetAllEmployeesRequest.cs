using FluentValidation;

namespace TheEmployeeAPI.Employees;

// query string parameters for getallempoloyees??????
public class GetAllEmployeesRequest
{
    public int? Page { get; set; }
    public int? RecordsPerPage { get; set; }
    public string? FirstNameContains { get; set; }
    public string? LastNameContains { get; set; }
}

public class GetAllEmployeesRequestValidator : AbstractValidator<GetAllEmployeesRequest>
{
        public GetAllEmployeesRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be set to a positive number");
        RuleFor(x => x.RecordsPerPage)
            .GreaterThan(0).WithMessage("YOu must return at least one record")
            .LessThanOrEqualTo(100).WithMessage("You cannot return more than 100 records.");
    }
}
