using Casbin.Persist.Adapter.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// Db Context for Casbin ef core adapter. Persists authorization policy
/// </summary>
/// <param name="options">Options for Db Context.</param>
public class CasbinDbContext(DbContextOptions<CasbinDbContext> options)
    : CasbinDbContext<int>(options) { }
