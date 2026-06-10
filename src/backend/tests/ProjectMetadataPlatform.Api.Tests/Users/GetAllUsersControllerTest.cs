using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class GetAllUsersControllerTest
{
    private UsersController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new UsersController(_mediator.Object, null!);
    }

    [Test]
    public async Task Get_ReturnsAllUsers()
    {
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                EmployeeId = "Id",
                Id = "1",
                Email = "Hinz",
                IsActive = true,
                IsScimProvisioned = false,
            },
            new ApplicationUser
            {
                EmployeeId = "3",
                Id = "2",
                Email = "Kunz",
                IsActive = true,
                IsScimProvisioned = false,
                Company = new() { CompanyName = "Appsfactory" },
                Departments =
                [
                    new() { DepartmentName = "Design" },
                    new() { DepartmentName = "QA" },
                ],
                BusinessUnits = [new() { BusinessUnitName = "Health" }],
                Teams =
                [
                    new Team
                    {
                        TeamName = "Team",
                        BusinessUnit = new() { BusinessUnitName = "Health" },
                        BusinessUnitId = 1,
                    },
                ],
                TeamSupport = [],
            },
        };
        _ = _mediator
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _controller.Get();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as GetUsersResponse;

        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Resources.ToList(), Has.Count.EqualTo(response.TotalResults));
            Assert.That(response.Resources.ElementAt(0).Id, Is.EqualTo("Id"));
            Assert.That(response.Resources.ElementAt(0).UserName, Is.EqualTo("Hinz"));
            Assert.That(response.Resources.ElementAt(0).ExternalId, Is.EqualTo("Id"));
            Assert.That(response.Resources.ElementAt(0).Active, Is.EqualTo(true));
            Assert.That(
                response.Resources.ElementAt(0).PmpUser!.IsScimProvisioned,
                Is.EqualTo(false)
            );
            Assert.That(response.Resources.ElementAt(1).Id, Is.EqualTo("3"));
            Assert.That(response.Resources.ElementAt(1).UserName, Is.EqualTo("Kunz"));
            Assert.That(response.Resources.ElementAt(1).ExternalId, Is.EqualTo("3"));
            Assert.That(response.Resources.ElementAt(1).Active, Is.EqualTo(true));
            Assert.That(
                response.Resources.ElementAt(1).PmpUser!.IsScimProvisioned,
                Is.EqualTo(false)
            );
            Assert.That(
                response.Resources.ElementAt(1).EnterpriseUser!.Organization,
                Is.EqualTo("Appsfactory")
            );
            Assert.That(
                response.Resources.ElementAt(1).PmpUser!.Departments,
                Is.EqualTo(new List<string> { "Design", "QA" })
            );
            Assert.That(
                response.Resources.ElementAt(1).PmpUser!.BusinessUnits,
                Is.EqualTo(new List<string> { "Health" })
            );
            Assert.That(
                response.Resources.ElementAt(1).PmpUser!.Team,
                Is.EqualTo(new List<string> { "Team" })
            );
            Assert.That(
                response.Resources.ElementAt(1).PmpUser!.TeamSupport,
                Is.EqualTo(new List<string> { })
            );
        });
    }

    [Test]
    public async Task Get_ReturnsEmptyList()
    {
        _ = _mediator
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ApplicationUser>());

        var result = await _controller.Get();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as GetUsersResponse;
        Assert.That(response!.Resources, Is.Not.Null.And.Empty);
    }

    [Test]
    public void Get_ReturnsMediatorException()
    {
        _ = _mediator
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        _ = Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Get());
    }
}
