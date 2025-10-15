using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Authorization;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class GetPolicyQueryHandlerTest
{
    private GetPolicyQueryHandler _handler;

    private Mock<IEnforcer> _enforcer;

    [SetUp]
    public void Setup()
    {
        _enforcer = new Mock<IEnforcer>();
        _handler = new GetPolicyQueryHandler(_enforcer.Object);
    }

    [Test]
    public async Task HandleGetPolicyQuerySuccesful()
    {
        _enforcer
            .Setup(m => m.GetPolicy())
            .Returns(
                [
                    ["aCondition", "anotherCondition", "anotherCondition", "anAction", "aEffect"],
                    ["aCondition2", "anotherCondition2", "anAction2", "aEffect"],
                ]
            );

        var request = new GetPolicyQuery();
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(
                result.First(),
                Is.EqualTo(
                    "aCondition && anotherCondition && anotherCondition && anAction && aEffect"
                )
            );
            Assert.That(
                result.Last(),
                Is.EqualTo("aCondition2 && anotherCondition2 && anAction2 && aEffect2")
            );
        });
    }
}
