using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IOfficeLocationRepository
{
    Task<IList<OfficeLocation>> GetOfficeLocationsAsync();
    Task<OfficeLocation> GetOfficeLocationAsync(int id);
    Task<bool> CheckIfOfficeLocationNameExistsAsync(string officeLocationName);

    Task<bool> CheckIfOfficeLocationExistsAsync(int id);

    Task<OfficeLocation> GetOfficeLocationByNameAsync(string officeLocationName);

    Task AddOfficeLocationAsync(OfficeLocation officeLocation);
    Task<OfficeLocation> UpdateOfficeLocationAsync(OfficeLocation location);
    Task<OfficeLocation> DeleteOfficeLocationAsync(OfficeLocation location);
}
