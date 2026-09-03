using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for managing billing information.
/// </summary>
public interface IBillingRepository
{
    /// <summary>
    /// Checks wether billing information of the specified kind already exists.
    /// </summary>
    /// <param name="kind">Billing kind</param>
    /// <returns>True if billing info exists.</returns>
    Task<bool> CheckBillingKindExists(string kind);

    /// <summary>
    /// Checks if a specified plugin has billing information.
    /// </summary>
    /// <param name="projectId">Id of the project of plugin.</param>
    /// <param name="pluginId">Id of the plugin.</param>
    /// <returns>True if billing information exists on plugin.</returns>
    Task<bool> CheckPluginBillingExists(int projectId, int pluginId);

    /// <summary>
    /// Adds or Updates global Billing information.
    /// </summary>
    /// <param name="billing">Billing Information to be stored.</param>
    /// <returns>Created / Updated Billing Object.</returns>
    Task<GlobalBilling> StoreBillingInformation(GlobalBilling billing);

    /// <summary>
    /// Adds a Plugin Billing object to the database.
    /// </summary>
    /// <param name="billing">Billing object to be added.</param>
    /// <returns></returns>
    Task AddPluginBilling(Domain.Billing.PluginBilling billing);

    /// <summary>
    /// Updates a plugin billing object.
    /// </summary>
    /// <param name="billing">Updated Billing Object.</param>
    /// <returns>Updated Billing Object.</returns>
    Task UpdatePluginBilling(Domain.Billing.PluginBilling billing);

    /// <summary>
    /// Returns a global billing object.
    /// </summary>
    /// <param name="id">Id of the global billing</param>
    /// <returns>The billing information if it exists.</returns>
    Task<GlobalBilling> GetBillingByIdAsync(int id);

    /// <summary>
    /// Returns the billing information of a specified project plugin.
    /// </summary>
    /// <param name="projectId">ProjectId of the plugin.</param>
    /// <param name="pluginId">Id of the plugin.</param>
    /// <returns>Plugin billing information if it exists.</returns>
    Task<Domain.Billing.PluginBilling> GetPluginBillingByIdAsync(int projectId, int pluginId);

    /// <summary>
    /// Returns a global billing object without tracking changes.
    /// </summary>
    /// <param name="id">Id of the global billing</param>
    /// <returns>The billing information if it exists.</returns>
    Task<GlobalBilling> GetBillingByIdAsNoTrackingAsync(int id);

    /// <summary>
    /// Returns queryable containing all global billing objects.
    /// </summary>
    /// <returns>All Global Billing Information. </returns>
    Task<IQueryable<GlobalBilling>> GetAllGlobalBillingInformationAsync();

    /// <summary>
    /// Deletes global billing Information.
    /// </summary>
    /// <param name="billing">Billing object to be deleted.</param>
    /// <returns></returns>
    Task DeleteBillingAsync(GlobalBilling billing);

    /// <summary>
    /// Deletes plugin billing information.
    /// </summary>
    /// <param name="billing">Billing object to be deleted.</param>
    /// <returns></returns>
    Task DeletePluginBillingAsync(Domain.Billing.PluginBilling billing);
}
