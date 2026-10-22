using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class GetAllTeamsQueryHandlerTest
{
    private GetAllTeamsQueryHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockTeamRepository = new Mock<ITeamRepository>();
        _handler = new GetAllTeamsQueryHandler(
            teamRepository: _mockTeamRepository.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }
}
