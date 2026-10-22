using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Logs;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Logs;

[TestFixture]
public class GetLogsQueryHandlerTest
{
    private GetLogsQueryHandler _handler;
    private Mock<ILogRepository> _mockLogsRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockLogsRepo = new Mock<ILogRepository>();
        _handler = new GetLogsQueryHandler(_mockLogsRepo.Object, _paginationHelperMock.Object);
    }

    [Test]
    public async Task GetLogs_ReturnsAllLogs_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorName = "Zitronenfalter",
            AuthorTokenId = null,
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
            AuthorTokenId = 2,
            AuthorId = null,
            AuthorName = "Halbleiter",
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Log>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Log> query, int? limit) => (query, null));
        _ = _mockLogsRepo
            .Setup(r => r.GetAllLogs(null))
            .ReturnsAsync(new List<Log> { log, log2 }.BuildMock());

        var (result, cursor) = await _handler.Handle(
            new GetLogsQuery(null, null, null, null),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(logList[0], Is.EqualTo(log));
            Assert.That(logList[1], Is.EqualTo(log2));
        });

        _mockLogsRepo.Verify(r => r.GetAllLogs(null), Times.Once);
    }

    [Test]
    public async Task GetLogs_ReturnsAllFilteredLogs_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorName = "Zitronenfalter",
            AuthorTokenId = null,
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
            AuthorTokenId = 2,
            AuthorId = null,
            AuthorName = "Halbleiter",
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Log>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Log> query, int? limit) => (query, null));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Log>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockLogsRepo
            .Setup(r => r.GetAllLogs(null))
            .ReturnsAsync(new List<Log> { log, log2 }.BuildMock());

        var (result, cursor) = await _handler.Handle(
            new GetLogsQuery(null, null, null, null),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(logList[0], Is.EqualTo(log));
            Assert.That(logList[1], Is.EqualTo(log2));
        });

        _mockLogsRepo.Verify(r => r.GetAllLogs(null), Times.Once);
    }

    [Test]
    public async Task GetLogs_ReturnsAllLogsForProject_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorTokenId = null,
            AuthorName = "Zitronenfalter",
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Log>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Log> query, int? limit) => (query, null));
        _ = _mockLogsRepo
            .Setup(r => r.GetLogsForProject(1, null))
            .ReturnsAsync(new List<Log> { log }.BuildMock());

        var (result, cursor) = await _handler.Handle(
            new GetLogsQuery(null, null, null, null, 1),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsForProject(1, null), Times.Once);
    }

    [Test]
    public async Task GetLogs_WithSearch_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorTokenId = null,
            AuthorName = "Derivative",
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Log>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Log> query, int? limit) => (query, null));
        _ = _mockLogsRepo
            .Setup(r => r.GetLogsWithSearch("exp(x)", null, null, null))
            .ReturnsAsync(new List<Log> { log }.BuildMock());

        var (result, cursor) = await _handler.Handle(
            new GetLogsQuery(null, null, null, null, null, "exp(x)"),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsWithSearch("exp(x)", null, null, null), Times.Once);
    }

    [Test]
    public async Task GetLogs_ForAffectedUser_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorTokenId = 1,
            AuthorId = null,
            AuthorName = "Newton",
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Log>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Log> query, int? limit) => (query, null));
        _ = _mockLogsRepo
            .Setup(r => r.GetLogsForUser("Newton", null))
            .ReturnsAsync(new List<Log> { log }.BuildMock());

        var (result, cursor) = await _handler.Handle(
            new GetLogsQuery(null, null, null, null, null, null, "Newton"),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsForUser("Newton", null), Times.Once);
    }

    [Test]
    public async Task GetLogs_ForGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "1",
            AuthorTokenId = null,
            AuthorName = "Newton",
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Log>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Log> query, int? limit) => (query, null));
        _ = _mockLogsRepo
            .Setup(r => r.GetLogsForGlobalPlugin(42, null))
            .ReturnsAsync(new List<Log> { log }.BuildMock());

        var (result, cursor) = await _handler.Handle(
            new GetLogsQuery(null, null, null, null, null, null, null, 42),
            CancellationToken.None
        );
        var logList = result.ToList();

        Assert.That(logList, Has.Count.EqualTo(1));
        Assert.That(logList[0], Is.EqualTo(log));

        _mockLogsRepo.Verify(r => r.GetLogsForGlobalPlugin(42, null), Times.Once);
    }

    [Test]
    public void GetLogs_ThrowsExceptionWhenProjectNotFound_Test()
    {
        _ = _mockLogsRepo
            .Setup(m => m.GetLogsForProject(It.IsAny<int>(), It.IsAny<LogCursor?>()))
            .ThrowsAsync(new InvalidOperationException());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Log>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Log> query, Dictionary<string, string>? dict) => query);
        var request = new GetLogsQuery(null, null, null, null, 404);
        _ = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(request, It.IsAny<CancellationToken>())
        );

        _mockLogsRepo.Verify(m => m.GetLogsForProject(404, null), Times.Once);
    }
}
