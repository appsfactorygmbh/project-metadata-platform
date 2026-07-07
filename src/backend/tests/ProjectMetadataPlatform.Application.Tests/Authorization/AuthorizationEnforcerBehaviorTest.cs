using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class AuthorizationEnforcerBehaviorTest
{
    private Mock<IAuthorizationTracker> _authorizationTrackerMock;

    private AuthorizationEnforcerBehavior<
        It.IsAnyType,
        It.IsAnyType
    > _authorizationEnforcerBehavior;

    private Mock<RequestHandlerDelegate<It.IsAnyType>> _next;

    [SetUp]
    public void SetUp()
    {
        _authorizationTrackerMock = new Mock<IAuthorizationTracker>();
        _next = new Mock<RequestHandlerDelegate<It.IsAnyType>>();
        _authorizationEnforcerBehavior = new AuthorizationEnforcerBehavior<
            It.IsAnyType,
            It.IsAnyType
        >(_authorizationTrackerMock.Object);
    }

    [Test]
    public async Task HandleRequest_WasChecked()
    {
        _authorizationTrackerMock.Setup(a => a.WasChecked).Returns(true);
        await _authorizationEnforcerBehavior.Handle(
            It.IsAny<It.IsAnyType>(),
            _next.Object,
            new CancellationToken()
        );

        _next.Verify(next => next(), Times.Once);
        _authorizationTrackerMock.Verify(a => a.WasChecked, Times.Once);
        _authorizationTrackerMock.Verify(a => a.RevertCheck(), Times.Once);
    }

    [Test]
    public async Task HandleRequest_WasNotChecked_Throws()
    {
        _authorizationTrackerMock.Setup(a => a.WasChecked).Returns(false);

        Assert.ThrowsAsync<UnauthorizedException>(async () =>
            await _authorizationEnforcerBehavior.Handle(
                It.IsAny<It.IsAnyType>(),
                _next.Object,
                new CancellationToken()
            )
        );

        _next.Verify(next => next(), Times.Once);
        _authorizationTrackerMock.Verify(a => a.WasChecked, Times.Once);
        _authorizationTrackerMock.Verify(a => a.RevertCheck(), Times.Never);
    }
}
