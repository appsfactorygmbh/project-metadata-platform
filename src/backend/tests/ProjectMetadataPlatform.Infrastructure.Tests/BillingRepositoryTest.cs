using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.Billing;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class BillingRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;

    private BillingRepository _billingRepository;

    [SetUp]
    public async Task SetupAsync()
    {
        _context = DbContext();
        _billingRepository = new BillingRepository(_context);
        ClearData(_context);
        _context.Projects.Add(
            new Project
            {
                Id = 1,
                ProjectName = "A",
                Slug = "a",
                ClientName = "1",
                CompanyId = 1,
            }
        );
        _context.Plugins.Add(
            new Plugin
            {
                PluginName = "Warp-Drive",
                ProjectPlugins = [],
                Id = 1,
            }
        );
        var plugin = new ProjectPlugin
        {
            Id = 1,
            ProjectId = 1,
            PluginId = 1,
            DisplayName = "Gitlab",
            Url = "gitlab.de",
        };
        _ = _context.ProjectPluginsRelation.Add(plugin);

        _ = await _context.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task CheckBillingKindExists_TrueTest()
    {
        var billing = new GlobalBilling { BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();

        var result = await _billingRepository.CheckBillingKindExists(billing.BillingKind);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckBillingKindExists_FalseTest()
    {
        var result = await _billingRepository.CheckBillingKindExists("Abc");
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task StoreBillingInformation_StoresBillingTest()
    {
        var billing = new GlobalBilling { BillingKind = "Abc" };

        var result = await _billingRepository.StoreBillingInformation(billing);
        await _context.SaveChangesAsync();
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.BillingKind, Is.EqualTo(billing.BillingKind));
    }

    [Test]
    public async Task StoreBillingInformation_UpdatesBillingTest()
    {
        var billing = new GlobalBilling { BillingKind = "Abc" };

        var result = await _billingRepository.StoreBillingInformation(billing);
        await _context.SaveChangesAsync();
        Assert.That(result.Id, Is.EqualTo(1));
        billing.BillingKind = "CBB";
        var resultUpdate = await _billingRepository.StoreBillingInformation(billing);
        await _context.SaveChangesAsync();
        Assert.That(resultUpdate.Id, Is.EqualTo(billing.Id));
        Assert.That(resultUpdate.BillingKind, Is.EqualTo(billing.BillingKind));
    }

    [Test]
    public async Task AddPluginBillingTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();
        Assert.That(_context.PluginBillingRelation.Any(), Is.False);
        var pluginBilling = new PluginBilling
        {
            ProjectId = 1,
            PluginId = 1,
            BillingId = billing.Id,
            DisplayName = "Name",
            Currency = "",
            BudgetLimit = 0,
            HostingFee = 0,
            TargetMargin = 0,
            TimeFrame = TimeFrame.NEVER,
        };
        await _billingRepository.AddPluginBilling(pluginBilling);
        await _context.SaveChangesAsync();
        var result = _context.PluginBillingRelation.First();
        Assert.That(result.DisplayName, Is.EqualTo(pluginBilling.DisplayName));
    }

    [Test]
    public async Task UpdatePluginBillingTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();
        Assert.That(_context.PluginBillingRelation.Any(), Is.False);
        var pluginBilling = new PluginBilling
        {
            ProjectId = 1,
            PluginId = 1,
            BillingId = billing.Id,
            DisplayName = "Name",
            Currency = "",
            BudgetLimit = 0,
            HostingFee = 0,
            TargetMargin = 0,
            TimeFrame = TimeFrame.NEVER,
        };
        await _context.PluginBillingRelation.AddAsync(pluginBilling);
        await _context.SaveChangesAsync();
        Assert.That(_context.PluginBillingRelation.Any(), Is.True);
        pluginBilling.DisplayName = "Other Name";
        await _billingRepository.UpdatePluginBilling(pluginBilling);
        await _context.SaveChangesAsync();
        var result = _context.PluginBillingRelation.First();
        Assert.That(result.DisplayName, Is.EqualTo(pluginBilling.DisplayName));
    }

    [Test]
    public async Task GetBillingByIdAsync_ReturnsBillingTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();

        var result = await _billingRepository.GetBillingByIdAsync(1);

        Assert.That(result, Is.EqualTo(billing));
    }

    [Test]
    public async Task GetBillingByIdAsync_ThrowsIfNotFoundTest()
    {
        Assert.ThrowsAsync<BillingInformationNotFoundException>(() =>
            _billingRepository.GetBillingByIdAsync(1)
        );
    }

    [Test]
    public async Task GetBillingByIdAsNoTrackingAsync_ReturnsBillingTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();

        var result = await _billingRepository.GetBillingByIdAsNoTrackingAsync(1);

        Assert.That(result.BillingKind, Is.EqualTo(billing.BillingKind));
        Assert.That(result.Id, Is.EqualTo(billing.Id));
    }

    [Test]
    public async Task GetBillingByIdAsNoTrackingAsync_ThrowsIfNotFoundTest()
    {
        Assert.ThrowsAsync<BillingInformationNotFoundException>(() =>
            _billingRepository.GetBillingByIdAsNoTrackingAsync(1)
        );
    }

    [Test]
    public async Task GetPluginBillingByIdAsync_ReturnsPluginBillingTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();
        var pluginBilling = new PluginBilling
        {
            ProjectId = 1,
            PluginId = 1,
            BillingId = billing.Id,
            DisplayName = "Name",
            Currency = "",
            BudgetLimit = 0,
            HostingFee = 0,
            TargetMargin = 0,
            TimeFrame = TimeFrame.NEVER,
        };
        await _context.PluginBillingRelation.AddAsync(pluginBilling);
        await _context.SaveChangesAsync();
        var result = await _billingRepository.GetPluginBillingByIdAsync(1, 1);

        Assert.That(result, Is.EqualTo(pluginBilling));
    }

    [Test]
    public async Task GetPluginBillingByIdAsync_ThrowsIfNotFoundTest()
    {
        Assert.ThrowsAsync<PluginBillingInformationNotFoundException>(() =>
            _billingRepository.GetPluginBillingByIdAsync(1, 1)
        );
    }

    [Test]
    public async Task GetAllGlobalBillingInformationAsync_ReturnsEmptyTest()
    {
        var result = await (
            await _billingRepository.GetAllGlobalBillingInformationAsync()
        ).ToListAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllGlobalBillingInformationAsync_ReturnsAllBillingTest()
    {
        var billing1 = new GlobalBilling { Id = 1, BillingKind = "Abc" };
        var billing2 = new GlobalBilling { Id = 3, BillingKind = "Abcd" };

        await _context.GlobalBilling.AddRangeAsync(billing1, billing2);
        await _context.SaveChangesAsync();
        var result = await (
            await _billingRepository.GetAllGlobalBillingInformationAsync()
        ).ToListAsync();

        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.First().BillingKind, Is.EqualTo(billing1.BillingKind));
        Assert.That(result.Last().BillingKind, Is.EqualTo(billing2.BillingKind));
    }

    [Test]
    public async Task DeleteBillingAsyncTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();
        var pluginBilling = new PluginBilling
        {
            ProjectId = 1,
            PluginId = 1,
            BillingId = billing.Id,
            DisplayName = "Name",
            Currency = "",
            BudgetLimit = 0,
            HostingFee = 0,
            TargetMargin = 0,
            TimeFrame = TimeFrame.NEVER,
        };
        await _context.PluginBillingRelation.AddAsync(pluginBilling);
        await _context.SaveChangesAsync();
        Assert.That(_context.GlobalBilling.Any(), Is.True);
        Assert.That(_context.PluginBillingRelation.Any(), Is.True);
        await _billingRepository.DeleteBillingAsync(billing);
        await _context.SaveChangesAsync();
        Assert.That(_context.GlobalBilling.Any(), Is.False);
        Assert.That(_context.PluginBillingRelation.Any(), Is.False);
    }

    [Test]
    public async Task DeletePluginBillingAsyncTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();
        var pluginBilling = new PluginBilling
        {
            ProjectId = 1,
            PluginId = 1,
            BillingId = billing.Id,
            DisplayName = "Name",
            Currency = "",
            BudgetLimit = 0,
            HostingFee = 0,
            TargetMargin = 0,
            TimeFrame = TimeFrame.NEVER,
        };
        await _context.PluginBillingRelation.AddAsync(pluginBilling);
        await _context.SaveChangesAsync();
        Assert.That(_context.PluginBillingRelation.Any(), Is.True);
        await _billingRepository.DeletePluginBillingAsync(pluginBilling);
        await _context.SaveChangesAsync();
        Assert.That(_context.PluginBillingRelation.Any(), Is.False);
    }

    [Test]
    public async Task CheckPluginBillingExists_TrueTest()
    {
        var billing = new GlobalBilling { Id = 1, BillingKind = "Abc" };

        await _context.GlobalBilling.AddAsync(billing);
        await _context.SaveChangesAsync();
        var pluginBilling = new PluginBilling
        {
            ProjectId = 1,
            PluginId = 1,
            BillingId = billing.Id,
            DisplayName = "Name",
            Currency = "",
            BudgetLimit = 0,
            HostingFee = 0,
            TargetMargin = 0,
            TimeFrame = TimeFrame.NEVER,
        };
        await _context.PluginBillingRelation.AddAsync(pluginBilling);
        await _context.SaveChangesAsync();

        var result = await _billingRepository.CheckPluginBillingExists(1, 1);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckPluginBillingExists_FalseTest()
    {
        var result = await _billingRepository.CheckPluginBillingExists(1, 1);
        Assert.That(result, Is.False);
    }
}
