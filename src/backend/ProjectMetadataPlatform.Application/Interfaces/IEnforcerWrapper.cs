using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Wrapper Interface for Casbin IEnforcer
/// </summary>
public interface IEnforcerWrapper
{
    /// <inheritdoc cref="Casbin.ManagementEnforcerExtension.GetPolicy(IEnforcer)"/>
    IEnumerable<IEnumerable<string>> GetPolicy();

    /// <inheritdoc cref="Casbin.EnforcerExtension.LoadPolicyAsync(IEnforcer)"/>
    Task<bool> LoadPolicyAsync();

    /// <inheritdoc cref="Casbin.EnforcerExtension.EnforceAsync{T1, T2, T3, T4}(IEnforcer, T1, T2, T3, T4)"/>
    Task<bool> EnforceAsync<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4);

    /// <inheritdoc cref="Casbin.ManagementEnforcerExtension.AddPolicyAsync(IEnforcer, string[])"/>
    Task<bool> AddPolicyAsync(params string[] values);

    /// <inheritdoc cref="Casbin.EnforcerExtension.SavePolicyAsync(IEnforcer)"/>
    Task<bool> SavePolicyAsync();

    /// <inheritdoc cref="Casbin.EnforcerExtension.LoadPolicy(IEnforcer)"/>
    bool LoadPolicy();

    /// <inheritdoc cref="Casbin.ManagementEnforcerExtension.RemovePolicies(IEnforcer, IEnumerable{IEnumerable{string}})"/>
    bool RemovePolicies(IEnumerable<IEnumerable<string>> values);

    /// <inheritdoc cref="Casbin.ManagementEnforcerExtension.AddPolicies(IEnforcer, IEnumerable{IEnumerable{string}})"/>
    bool AddPolicies(IEnumerable<IEnumerable<string>> values);

    /// <inheritdoc cref="Casbin.ManagementEnforcerExtension.AddPolicy(IEnforcer, string[])"/>
    bool AddPolicy(params string[] values);

    /// <inheritdoc cref="Casbin.EnforcerExtension.SavePolicy(IEnforcer)"/>
    bool SavePolicy();
}
