using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IBusinessUnitRepository
{
    Task<IList<BusinessUnit>> GetBusinessUnitsAsync();
    Task<BusinessUnit> GetBusinessUnitAsync(int id);
    Task<bool> CheckIfBusinessUnitExistsAsync(int id);

    Task<bool> CheckIfBusinessUnitNameExistsAsync(string buName);

    Task<string> RetrieveNameForIdAsync(int id);

    Task<BusinessUnit> GetBusinessUnitByNameAsync(string buName);
    Task AddBusinessUnitAsync(BusinessUnit bu);
    Task<BusinessUnit> UpdateBusinessUnitAsync(BusinessUnit businessUnit);
    Task<BusinessUnit> DeleteBusinessUnitAsync(BusinessUnit businessUnit);

    Task<BusinessUnit> GetBusinessUnitWithTeamsAsync(int id);
}
