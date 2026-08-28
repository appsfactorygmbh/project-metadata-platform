using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Billing;

/// <summary>
/// The repository for billing information that handles the data access.
/// </summary>
public class BillingRepository : RepositoryBase<GlobalBilling>, IBillingRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BillingRepository" /> class.
    /// </summary>
    /// <param name="context">The database context for accessing billing data.</param>
    public BillingRepository(ProjectMetadataPlatformDbContext context)
        : base(context)
    {
        _context = context;
    }

    private readonly ProjectMetadataPlatformDbContext _context;

    /// <inheritdoc />
    public async Task<bool> CheckBillingKindExists(string kind)
    {
        var queryResult = GetIf(billing => billing.BillingKind.ToLower().Equals(kind.ToLower()));
        return await queryResult.AnyAsync();
    }

    /// <inheritdoc />
    public Task<GlobalBilling> StoreBillingInformation(GlobalBilling billing)
    {
        if (billing.Id == 0)
        {
            _ = _context.GlobalBilling.Add(billing);
        }
        else
        {
            Update(billing);
        }

        return Task.FromResult(billing);
    }

    /// <inheritdoc />
    public async Task AddPluginBilling(PluginBilling billing)
    {
        _ = await _context.PluginBillingRelation.AddAsync(billing);
    }

    /// <inheritdoc />
    public async Task UpdatePluginBilling(PluginBilling billing)
    {
        _ = _context.PluginBillingRelation.Update(billing);
    }

    /// <inheritdoc />
    public async Task<GlobalBilling> GetBillingByIdAsync(int id)
    {
        return await GetIf(gb => gb.Id == id).FirstOrDefaultAsync()
            ?? throw new BillingInformationNotFoundException(id);
    }

    /// <inheritdoc />
    public async Task<PluginBilling> GetPluginBillingByIdAsync(int projectId, int pluginId)
    {
        return await _context
                .PluginBillingRelation.Where(pb =>
                    pb.ProjectId == projectId && pb.PluginId == pluginId
                )
                .Include(pb => pb.GlobalBilling)
                .Include(pb => pb.ProjectPlugin!)
                    .ThenInclude(pp => pp.Project)
                .FirstOrDefaultAsync()
            ?? throw new PluginBillingInformationNotFoundException(projectId, pluginId);
    }

    /// <inheritdoc />
    public async Task<GlobalBilling> GetBillingByIdAsNoTrackingAsync(int id)
    {
        return await GetIf(gb => gb.Id == id).AsNoTracking().FirstOrDefaultAsync()
            ?? throw new BillingInformationNotFoundException(id);
    }

    /// <inheritdoc />
    public async Task<IQueryable<GlobalBilling>> GetAllGlobalBillingInformationAsync()
    {
        return GetEverything();
    }

    /// <inheritdoc />
    public async Task DeleteBillingAsync(GlobalBilling billing)
    {
        Delete(billing);
    }

    /// <inheritdoc />
    public async Task DeletePluginBillingAsync(PluginBilling billing)
    {
        _ = _context.PluginBillingRelation.Remove(billing);
    }

    /// <inheritdoc />
    public async Task<bool> CheckPluginBillingExists(int projectId, int pluginId)
    {
        return await _context.PluginBillingRelation.AnyAsync(pb =>
            pb.ProjectId == projectId && pb.PluginId == pluginId
        );
    }
}
