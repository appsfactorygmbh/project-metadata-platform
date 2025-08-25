using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Logs;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Logs;

[TestFixture]
public class GetLogsQueryHandlerTest
{
    private GetLogsQueryHandler _handler;
    private Mock<ILogRepository> _mockLogsRepo;

    [SetUp]
    public void Setup()
    {
        _mockLogsRepo = new Mock<ILogRepository>();
        _handler = new GetLogsQueryHandler(_mockLogsRepo.Object);
    }

    [Test]
    public async Task GetLogs_ReturnsAllLogs_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorEmail = "Zitronenfalter",
            ProjectId = 1,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    Property = "Zitrone",
                    OldValue = "Ungefaltet",
                    NewValue = "Gefaltet",
                },
            ],
        };
        var log2 = new Log
        {
            Id = 2,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "2",
            AuthorEmail = "Halbleiter",
            ProjectId = 2,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    Property = "Silizium",
                    OldValue = "rein",
                    NewValue = "dotiert",
                },
            ],
        };

        _mockLogsRepo.Setup(r => r.GetAllLogs()).ReturnsAsync([log, log2]);

        var result = await _handler.Handle(new GetLogsQuery(), CancellationToken.None);
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(logList[0], Is.EqualTo(log));
            Assert.That(logList[1], Is.EqualTo(log2));
        });

        _mockLogsRepo.Verify(r => r.GetAllLogs(), Times.Once);
    }

    [Test]
    public async Task GetLogs_ReturnsAllLogsForProject_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorEmail = "Zitronenfalter",
            ProjectId = 1,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    Property = "Zitrone",
                    OldValue = "Ungefaltet",
                    NewValue = "Gefaltet",
                },
            ],
        };

        _mockLogsRepo.Setup(r => r.GetLogsForProject(1)).ReturnsAsync([log]);

        var result = await _handler.Handle(new GetLogsQuery(1), CancellationToken.None);
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsForProject(1), Times.Once);
    }

    [Test]
    public async Task GetLogs_WithSearch_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorEmail = "Derivative",
            ProjectId = 1,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    Property = "d/dx",
                    OldValue = "exp(x)",
                    NewValue = "exp(x)",
                },
            ],
        };

        _mockLogsRepo.Setup(r => r.GetLogsWithSearch("exp(x)")).ReturnsAsync([log]);

        var result = await _handler.Handle(
            new GetLogsQuery(null, "exp(x)"),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsWithSearch("exp(x)"), Times.Once);
    }

    [Test]
    public async Task GetLogs_ForAffectedUser_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorEmail = "Newton",
            AffectedUserId = "Newton",
            Action = Action.UPDATED_USER,
            Changes =
            [
                new LogChange
                {
                    Property = "flying",
                    OldValue = "yes",
                    NewValue = "no",
                },
            ],
        };

        _mockLogsRepo.Setup(r => r.GetLogsForUser("Newton")).ReturnsAsync([log]);

        var result = await _handler.Handle(
            new GetLogsQuery(null, null, "Newton"),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsForUser("Newton"), Times.Once);
    }

    [Test]
    public async Task GetLogs_ForGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorEmail = "Newton",
            Action = Action.UPDATED_GLOBAL_PLUGIN,
            GlobalPluginId = 42,
            GlobalPluginName = "Gravity",
            Changes =
            [
                new LogChange
                {
                    Property = "discovered",
                    OldValue = "no",
                    NewValue = "yes",
                },
            ],
        };

        _mockLogsRepo.Setup(r => r.GetLogsForGlobalPlugin(42)).ReturnsAsync([log]);

        var result = await _handler.Handle(
            new GetLogsQuery(null, null, null, 42),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsForGlobalPlugin(42), Times.Once);
    }

    [Test]
    public void GetLogs_ThrowsExceptionWhenProjectNotFound_Test()
    {
        _mockLogsRepo
            .Setup(m => m.GetLogsForProject(It.IsAny<int>()))
            .ThrowsAsync(new InvalidOperationException());

        var request = new GetLogsQuery(404);
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(request, It.IsAny<CancellationToken>())
        );

        _mockLogsRepo.Verify(m => m.GetLogsForProject(404), Times.Once);
    }
}
