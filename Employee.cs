public class Employee 
{
    public int Id { get; set; }
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
    public List<EmployeeBenefit> Benefits { get; set; } = new List<EmployeeBenefit>();
}

public enum BenefitType
{
    Health,
    Dental,
    Vision
}

public class EmployeeBenefit
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public BenefitType BenefitType { get; set; }
    public decimal Cost { get; set; }

    // Add employee navigation property to our benefits
    //   so if we are getting the benefits we could 'join'
    //   and get properties from the employee table as well
    //                ............
    //
    // And so we add this line:
    // public Employee employee { get; set; }
    //
    // This complains because properties must be NOT NULL
    //    when exiting the constructor.  
    // Really, we just want employee here to be able to 
    //    refer back to the employee object once this
    //    is created, and we may not have access to all
    //   the employee data when this is constructed
    // Three options to fix not-nullable error:
    //    public Employee? employee { get; set; }
    //      - this is bad because EF Core will assume 
    //        EmployeeId is nullable then when it 
    //        is clearly defined as NOT nullable...not what
    //        we want because the id should never be
    //        nullable, confuse compiler even?
    //    public required Employee employee { get; set; }
    //      - this second option to fix compiles just
    //        fine but we are back to the problem that
    //        we might not have all the employee data
    //        when we create this!
    //    public Employee employee { get; set; } = null!;
    //      - this option is the 'lesser of all evils'
    //        This tells the compiler I'm setting this
    //        to a value but as a developer I am 
    //        asserting that it will NOT be null!
    // FYI this would just be a warning usually but with
    //   warnings as errors on this is how you can fix it!
    public Employee Employee { get; set; } = null!;
}