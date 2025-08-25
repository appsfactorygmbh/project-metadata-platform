using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Teams;

/// <inheritdoc />
public class GetLinkedProjectsQueryHandler : IRequestHandler<GetLinkedProjectsQuery, List<string>>
{
    private readonly ITeamRepository _teamRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetTeamQueryHandler" />.
    /// </summary>
    public GetLinkedProjectsQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    /// <inheritdoc/>
    public async Task<List<string>> Handle(
        GetLinkedProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        return ((await _teamRepository.GetTeamWithProjectsAsync(request.Id)).Projects ?? [])
            .Select(proj => proj.Slug)
            .ToList();
    }
}
