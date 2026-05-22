using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

/// <summary>
/// Exception thrown when a company is not found.
/// </summary>
public class CompanyNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyNotFoundException"/> class with a specified company ID.
    /// </summary>
    /// <param name="companyId">The ID of the Company that was not found.</param>
    public CompanyNotFoundException(int companyId)
        : base("The Company with id " + companyId + " was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref=" CompanyNotFoundException"/> class with a specified company name.
    /// </summary>
    /// <param name="company">The name of the Company that was not found.</param>
    public CompanyNotFoundException(string company)
        : base("The Company with name " + company + " was not found.") { }
}
