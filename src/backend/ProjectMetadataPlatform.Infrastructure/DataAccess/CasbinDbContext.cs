using Casbin.Persist.Adapter.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class CasbinDbContext(DbContextOptions<CasbinDbContext> options)
    : CasbinDbContext<int>(options) { }
