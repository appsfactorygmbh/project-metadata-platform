using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="GetApiTokenDetailsQuery" />.
/// </summary>
public class GetApiTokenDetailsQueryHandler : IRequestHandler<GetApiTokenDetailsQuery, ApiToken>
{
    private readonly IApiTokenRepository _apiTokenRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetApiTokenDetailsQueryHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    public GetApiTokenDetailsQueryHandler(IApiTokenRepository apiTokenRepository)
    {
        _apiTokenRepository = apiTokenRepository;
    }

    /// <summary>
    /// Returns the requested Token.
    /// </summary>
    /// <param name="request">Request for a token.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The Api Token.</returns>
    public Task<ApiToken> Handle(
        GetApiTokenDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        return _apiTokenRepository.GetApiTokenById(request.TokenId);
    }
}
