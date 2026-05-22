using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IBusinessUnitRepository
{
    Task<bool> CheckIfBusinessUnitExistsAsync(int id);

    Task<bool> CheckIfBusinessUnitNameExistsAsync(string buName);

    Task<string> RetrieveNameForIdAsync(int id);

    Task<BusinessUnit> GetBusinessUnitByNameAsync(string buName);
    Task AddBusinessUnitAsync(BusinessUnit bu);
}
