using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface ICompanyRepository
{
    Task<IList<Company>> GetCompaniesAsync();
    Task<Company> GetCompanyAsync(int id);
    Task<bool> CheckIfCompanyExistsAsync(int id);

    Task<bool> CheckIfCompanyNameExistsAsync(string companyName);

    Task<string> RetrieveNameForIdAsync(int id);

    Task<Company> GetCompanyByNameAsync(string companyName);

    Task AddCompanyAsync(Company company);
    Task<Company> UpdateCompanyAsync(Company company);
    Task<Company> DeleteCompanyAsync(Company company);

    Task<Company> GetCompanyWithProjectsAsync(int id);
}
