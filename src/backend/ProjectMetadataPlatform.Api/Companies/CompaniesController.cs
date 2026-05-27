using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.Companies;

[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="CompaniesController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
