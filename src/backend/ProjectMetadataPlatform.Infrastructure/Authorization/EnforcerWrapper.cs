using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin;
using Microsoft.Identity.Client;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Infrastructure.Authorization;

/// <summary>
/// Wrapper class for Casbin Enforcer
/// </summary>
public class EnforcerWrapper : IEnforcerWrapper
{
    private readonly IEnforcer _enforcer;

    /// <summary>
    /// initalizes Enforcer Wrapper
    /// </summary>
    /// <param name="enforcer"> Enforcer object</param>
    public EnforcerWrapper(IEnforcer enforcer)
    {
        _enforcer = enforcer;
    }

    ///  <inheritdoc />
    public bool AddPolicies(IEnumerable<IEnumerable<string>> values)
    {
        return _enforcer.AddPolicies(values);
    }

    ///  <inheritdoc />
    public bool AddPolicy(params string[] values)
    {
        return _enforcer.AddPolicy(values);
    }

    ///  <inheritdoc />
    public async Task<bool> AddPolicyAsync(params string[] values)
    {
        return await _enforcer.AddPolicyAsync(values);
    }

    ///  <inheritdoc />
    public async Task<bool> EnforceAsync<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        return await _enforcer.EnforceAsync(value1, value2, value3, value4);
    }

    ///  <inheritdoc />
    public IEnumerable<IEnumerable<string>> GetPolicy()
    {
        return _enforcer.GetPolicy();
    }

    ///  <inheritdoc />
    public bool LoadPolicy()
    {
        return _enforcer.LoadPolicy();
    }

    ///  <inheritdoc />
    public async Task<bool> LoadPolicyAsync()
    {
        return await _enforcer.LoadPolicyAsync();
    }

    ///  <inheritdoc />
    public bool RemovePolicies(IEnumerable<IEnumerable<string>> values)
    {
        return _enforcer.RemovePolicies(values);
    }

    ///  <inheritdoc />
    public bool SavePolicy()
    {
        return _enforcer.SavePolicy();
    }

    ///  <inheritdoc />
    public async Task<bool> SavePolicyAsync()
    {
        return await _enforcer.SavePolicyAsync();
    }
}
