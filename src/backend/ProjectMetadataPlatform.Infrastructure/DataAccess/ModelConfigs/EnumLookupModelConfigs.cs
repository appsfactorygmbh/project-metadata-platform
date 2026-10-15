using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Common;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for CompanyState Lookup.
/// </summary>
public class CompanyStateLookupConfig : IEntityTypeConfiguration<CompanyStateLookup>
{
    /// <summary>
    /// Configures the CompanyStateLookup entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<CompanyStateLookup> builder)
    {
        _ = builder.HasKey(e => e.Id);
        _ = builder.Property(e => e.Id).ValueGeneratedNever();
        _ = builder.Property(e => e.Name).IsRequired();

        _ = builder.HasData(
            Enum.GetValues<CompanyState>()
                .Select(e => new CompanyStateLookup
                {
                    Id = e,
                    Name = e.ToString().ToLower().Replace("_", " "),
                })
        );
    }
}

/// <summary>
/// Data Base Configuration for SecurityLevel Lookup.
/// </summary>
public class SecurityLevelLookupConfig : IEntityTypeConfiguration<SecurityLevelLookup>
{
    /// <summary>
    /// Configures the SecurityLevelLookup entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<SecurityLevelLookup> builder)
    {
        _ = builder.HasKey(e => e.Id);
        _ = builder.Property(e => e.Id).ValueGeneratedNever();
        _ = builder.Property(e => e.Name).IsRequired();

        _ = builder.HasData(
            Enum.GetValues<SecurityLevel>()
                .Select(e => new SecurityLevelLookup
                {
                    Id = e,
                    Name = e.ToString().ToLower().Replace("_", " "),
                })
        );
    }
}

/// <summary>
/// Data Base Configuration for TimeFrame Lookup.
/// </summary>
public class TimeFrameLookupConfig : IEntityTypeConfiguration<TimeFrameLookup>
{
    /// <summary>
    /// Configures the TimeFrameLookup entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<TimeFrameLookup> builder)
    {
        _ = builder.HasKey(e => e.Id);
        _ = builder.Property(e => e.Id).ValueGeneratedNever();
        _ = builder.Property(e => e.Name).IsRequired();

        _ = builder.HasData(
            Enum.GetValues<TimeFrame>()
                .Select(e => new TimeFrameLookup
                {
                    Id = e,
                    Name = e.ToString().ToLower().Replace("_", " "),
                })
        );
    }
}

/// <summary>
/// Data Base Configuration for Currency Lookup.
/// </summary>
public class CurrencyLookupConfig : IEntityTypeConfiguration<CurrencyLookup>
{
    /// <summary>
    /// Configures the CurrencyLookup entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<CurrencyLookup> builder)
    {
        _ = builder.HasKey(e => e.Id);
        _ = builder.Property(e => e.Id).ValueGeneratedNever();
        _ = builder.Property(e => e.Name).IsRequired();

        _ = builder.HasData(
            Enum.GetValues<Currencies>()
                .Select(e => new CurrencyLookup
                {
                    Id = e,
                    Name = e.ToString().ToLower().Replace("_", " "),
                })
        );
    }
}
