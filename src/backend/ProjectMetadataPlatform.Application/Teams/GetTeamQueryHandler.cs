using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <inheritdoc />
public class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, Team>
{
    private readonly ITeamRepository _teamRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetTeamQueryHandler" />.
    /// </summary>
    public GetTeamQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    /// <inheritdoc/>
    public Task<Team> Handle(GetTeamQuery request, CancellationToken cancellationToken)
    {
        return _teamRepository.GetTeamAsync(request.Id);
    }
}
