using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Authorization;
using ProjectMetadataPlatform.Api.Authorization.Models;
using ProjectMetadataPlatform.Application.Authorization;

namespace ProjectMetadataPlatform.Api.Tests.Authorization;

[TestFixture]
public class AuthorizationControllerTest
{
    private AuthorizationController _authorizationController;

    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _authorizationController = new AuthorizationController(_mediator.Object);
    }

    [Test]
    public async Task SuccessfulGetPoliciesTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetPolicyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(["aTestRule", "otherTestRule"]);
        var result = await _authorizationController.Get();
        Assert.That(result.Value, Is.InstanceOf<PolicyResponse>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Value.Rules.Count(), Is.EqualTo(2));
            Assert.That(result.Value.Rules.First(), Is.EqualTo("aTestRule"));
            Assert.That(result.Value.Rules.Last(), Is.EqualTo("otherTestRule"));
        });
    }

    [Test]
    public async Task SuccessfulPutRuleTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<PutRuleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new PutRuleRequest(
            new Domain.Authorization.PolicyRule
            {
                Action = "aAction",
                Effect = Domain.Authorization.Effect.DENY,
            }
        );

        var result = await _authorizationController.Put(request);
        Assert.That(result, Is.InstanceOf<CreatedResult>());
    }

    [Test]
    public async Task BadRequestPutRuleTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<PutRuleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new PutRuleRequest(
            new Domain.Authorization.PolicyRule
            {
                Action = "aAction",
                Effect = Domain.Authorization.Effect.ALLOW,
            }
        );

        var result = await _authorizationController.Put(request);
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }
}
