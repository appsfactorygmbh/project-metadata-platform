using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IEnforcerWrapper
{
    IEnumerable<IEnumerable<string>> GetPolicy();

    Task<bool> LoadPolicyAsync();

    Task<bool> EnforceAsync<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4);

    Task<bool> AddPolicyAsync(params string[] values);

    Task<bool> SavePolicyAsync();

    bool LoadPolicy();

    bool RemovePolicies(IEnumerable<IEnumerable<string>> values);

    bool AddPolicies(IEnumerable<IEnumerable<string>> values);

    bool AddPolicy(params string[] values);

    bool SavePolicy();
}
