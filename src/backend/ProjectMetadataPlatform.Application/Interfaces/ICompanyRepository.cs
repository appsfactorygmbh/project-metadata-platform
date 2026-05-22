using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface ICompanyRepository
{
    Task<bool> CheckIfCompanyExistsAsync(int id);

    Task<bool> CheckIfCompanyNameExistsAsync(string companyName);

    Task<string> RetrieveNameForIdAsync(int id);

    Task<Company> GetCompanyByNameAsync(string companyName);

    Task AddCompanyAsync(Company company);
}
