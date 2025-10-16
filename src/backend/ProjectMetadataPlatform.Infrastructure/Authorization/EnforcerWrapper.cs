using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin;
using Microsoft.Identity.Client;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Infrastructure.Authorization;

public class EnforcerWrapper : IEnforcerWrapper
{
    private readonly IEnforcer _enforcer;

    public EnforcerWrapper(IEnforcer enforcer)
    {
        _enforcer = enforcer;
    }

    public bool AddPolicies(IEnumerable<IEnumerable<string>> values)
    {
        return _enforcer.AddPolicies(values);
    }

    public bool AddPolicy(params string[] values)
    {
        return _enforcer.AddPolicy(values);
    }

    public async Task<bool> AddPolicyAsync(params string[] values)
    {
        return await _enforcer.AddPolicyAsync(values);
    }

    public async Task<bool> EnforceAsync<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        return await _enforcer.EnforceAsync(value1, value2, value3, value4);
    }

    public IEnumerable<IEnumerable<string>> GetPolicy()
    {
        return _enforcer.GetPolicy();
    }

    public bool LoadPolicy()
    {
        return _enforcer.LoadPolicy();
    }

    public async Task<bool> LoadPolicyAsync()
    {
        return await _enforcer.LoadPolicyAsync();
    }

    public bool RemovePolicies(IEnumerable<IEnumerable<string>> values)
    {
        return _enforcer.RemovePolicies(values);
    }

    public bool SavePolicy()
    {
        return _enforcer.SavePolicy();
    }

    public async Task<bool> SavePolicyAsync()
    {
        return await _enforcer.SavePolicyAsync();
    }
}
