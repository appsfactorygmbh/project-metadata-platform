using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Common;

/// <summary>
/// Internal Object for looking up the DisplayValue of a CompanyState Enum.
/// </summary>
public class CompanyStateLookup
{
    /// <summary>
    /// Enum Key.
    /// </summary>
    public CompanyState Id { get; set; }

    /// <summary>
    /// Enum Display Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Internal Object for looking up the DisplayValue of a SecurityLevel Enum.
/// </summary>
public class SecurityLevelLookup
{
    /// <summary>
    /// Enum Key
    /// </summary>
    public SecurityLevel Id { get; set; }

    /// <summary>
    /// Enum Display Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Internal Object for looking up the DisplayValue of a TimeFrame Enum.
/// </summary>
public class TimeFrameLookup
{
    /// <summary>
    /// Enum Key
    /// </summary>
    public TimeFrame Id { get; set; }

    /// <summary>
    /// Enum Display Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Internal Object for looking up the DisplayValue of a Currencies Enum.
/// </summary>
public class CurrencyLookup
{
    /// <summary>
    /// Enum Key
    /// </summary>
    public Currencies Id { get; set; }

    /// <summary>
    /// Enum Display Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
