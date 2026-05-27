using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.BusinessUnits;

[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class BusinessUnitsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="BusinessUnitsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public BusinessUnitsController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
