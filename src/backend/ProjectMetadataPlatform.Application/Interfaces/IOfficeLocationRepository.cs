using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IOfficeLocationRepository
{
    
    Task<bool> CheckIfOfficeLocationNameExistsAsync(string officeLocationName);

    Task<OfficeLocation> GetOfficeLocationByNameAsync(string officeLocationName);

    Task AddOfficeLocationAsync(OfficeLocation officeLocation);
}

