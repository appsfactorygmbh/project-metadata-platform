using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Represents a unit of work that encapsulates one or more operations,
/// ensuring that all changes are committed together as a single transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Commits all pending changes in the current unit of work to the database.
    /// </summary>
    Task CompleteAsync();
}
