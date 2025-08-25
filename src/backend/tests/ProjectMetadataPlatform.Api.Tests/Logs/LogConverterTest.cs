using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Logs;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Api.Tests.Logs;

public class LogConverterTest
{
    private LogConverter _logConverter;

    [SetUp]
    public void Setup()
    {
        _logConverter = new LogConverter();
    }

    [Test]
    public void ConvertToUpdatedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 41,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Slartibartfast",
            Author = new IdentityUser { Email = "Slartibartfast" },
            ProjectId = 43,
            ProjectName = "Norway",
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    Property = "Fjords",
                    OldValue = "None",
                    NewValue = "Many",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo("Slartibartfast updated project Norway:  set Fjords from None to Many")
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToArchivedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "43",
            AuthorEmail = "Deep Thought",
            Author = new IdentityUser { Email = "Deep Thought" },
            ProjectId = 44,
            Project = new Project
            {
                ProjectName = "Ultimate Question of Life, the Universe and Everything",
                Slug = "ultimate question of life, the universe and everything",
                ClientName = "Mice",
            },
            ProjectName = "Ultimate Question of Life, the Universe and Everything",
            Action = Action.ARCHIVED_PROJECT,
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Deep Thought archived project Ultimate Question of Life, the Universe and Everything"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToAddedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 43,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "44",
            AuthorEmail = "Infinite Improbability Drive",
            Author = new IdentityUser { Email = "Infinite Improbability Drive" },
            ProjectId = 45,
            Project = new Project
            {
                ProjectName = "Atmosphere",
                Slug = "atmosphere",
                ClientName = "",
            },
            ProjectName = "Atmosphere",
            Action = Action.ADDED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange
                {
                    Property = "flyingObjects",
                    OldValue = "",
                    NewValue = "Wale",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Infinite Improbability Drive added a new plugin to project Atmosphere with properties: flyingObjects = Wale"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToUpdatedProjectPluginLog_Test()
    {
        var log = new Log
        {
            Id = 44,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "45",
            AuthorEmail = "Ground",
            Author = new IdentityUser { Email = "Ground" },
            ProjectId = 46,
            Project = new Project
            {
                ProjectName = "Wale",
                Slug = "wale",
                ClientName = "",
            },
            ProjectName = "Wale",
            Action = Action.UPDATED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange
                {
                    Property = "alive",
                    OldValue = "yes",
                    NewValue = "no",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Ground updated plugin properties in project Wale:  set alive from yes to no"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToRemovedProjectPluginLog_Test()
    {
        var log = new Log
        {
            Id = 45,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "46",
            AuthorEmail = "Prostetnic Vogon Jeltz",
            Author = new IdentityUser { Email = "Prostetnic Vogon Jeltz" },
            ProjectId = 47,
            Project = new Project
            {
                ProjectName = "Solarsystem",
                Slug = "solarsystem",
                ClientName = "Mice",
            },
            ProjectName = "Solarsystem",
            Action = Action.REMOVED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange
                {
                    Property = "Earth",
                    OldValue = "intact",
                    NewValue = "",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Prostetnic Vogon Jeltz removed a plugin from project Solarsystem with properties: Earth = intact"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToUnarchivedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 46,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "47",
            AuthorEmail = "Earth",
            Author = new IdentityUser { Email = "Earth" },
            ProjectId = 48,
            Project = new Project
            {
                ProjectName = "Ultimate Question of Life, the Universe and Everything",
                Slug = "ultimate question of life, the universe and everything",
                ClientName = "Mice",
            },
            ProjectName = "Ultimate Question of Life, the Universe and Everything",
            Action = Action.UNARCHIVED_PROJECT,
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Earth unarchived project Ultimate Question of Life, the Universe and Everything"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToProjectLogDeletedUser_Test()
    {
        var log = new Log
        {
            Id = 47,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "48",
            AuthorEmail = "Earth Population",
            ProjectId = 49,
            Action = Action.UPDATED_PROJECT,
            ProjectName = "Living Beings",
            Changes =
            [
                new LogChange
                {
                    Property = "Home Planet",
                    OldValue = "Earth",
                    NewValue = "None",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Earth Population (deleted user) updated project Living Beings:  set Home Planet from Earth to None"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogAddedUser_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Infinite Improbability Drive",
            Author = new IdentityUser { Email = "Infinite Improbability Drive" },
            Action = Action.ADDED_USER,
            Changes =
            [
                new LogChange
                {
                    Property = "Email",
                    OldValue = "",
                    NewValue = "Bowl of Petunias",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Infinite Improbability Drive added a new user with properties: Email = Bowl of Petunias"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogUpdatedUser_Test()
    {
        var gandalf = new IdentityUser { Email = "Gandalf" };
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Gandalf",
            Author = gandalf,
            AffectedUser = gandalf,
            AffectedUserEmail = "Gandalf",
            Action = Action.UPDATED_USER,
            Changes =
            [
                new LogChange
                {
                    Property = "Email",
                    OldValue = "Gandalf the Grey",
                    NewValue = "Gandalf the White",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Gandalf updated user Gandalf: set Email from Gandalf the Grey to Gandalf the White"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogRemovedUser_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Ground",
            Author = new IdentityUser { Email = "Ground" },
            Action = Action.REMOVED_USER,
            AffectedUserEmail = "whale@air.com",
            Changes =
            [
                new LogChange
                {
                    Property = "Email",
                    OldValue = "Whale",
                    NewValue = "",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Ground removed user whale@air.com"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogRemovedProject_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Luke Skywalker",
            Author = new IdentityUser { Email = "Luke Skywalker" },
            Action = Action.REMOVED_PROJECT,
            ProjectName = "DeathStar",
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo("Luke Skywalker removed project DeathStar")
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogAddedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Chancellor Palpatine",
            Author = new IdentityUser { Email = "Chancellor Palpatine" },
            Action = Action.ADDED_GLOBAL_PLUGIN,
            Changes =
            [
                new LogChange
                {
                    Property = "PluginName",
                    OldValue = "",
                    NewValue = "Grand Army of the Republic",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Chancellor Palpatine added a new global plugin with properties: PluginName = Grand Army of the Republic"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogUpdatedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Darth Sidious",
            Author = new IdentityUser { Email = "Darth Sidious" },
            Action = Action.UPDATED_GLOBAL_PLUGIN,
            Changes =
            [
                new LogChange
                {
                    Property = "Email",
                    OldValue = "Republic",
                    NewValue = "First Galactic Empire",
                },
            ],
            GlobalPluginName = "DeathStar",
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Darth Sidious updated global plugin DeathStar: set Email from Republic to First Galactic Empire"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogArchivedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Zip",
            Author = new IdentityUser { Email = "Zip" },
            Action = Action.ARCHIVED_GLOBAL_PLUGIN,
            GlobalPluginName = "Directory",
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Zip archived global plugin Directory"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogUnarchivedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Unzip",
            Author = new IdentityUser { Email = "Unzip" },
            Action = Action.UNARCHIVED_GLOBAL_PLUGIN,
            GlobalPluginName = "Directory",
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo("Unzip unarchived global plugin Directory")
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogRemovedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Recursively",
            Author = new IdentityUser { Email = "Recursively" },
            Action = Action.REMOVED_GLOBAL_PLUGIN,
            GlobalPluginName = "root",
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo("Recursively removed global plugin root")
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogAddedTeam_Test()
    {
        var createdTeam = new Team() { TeamName = "root", BusinessUnit = "Test BU" };

        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Recursively",
            Author = new IdentityUser { Email = "Recursively" },
            Action = Action.ADDED_TEAM,
            TeamName = "root",
            Changes =
            [
                new()
                {
                    Property = "TeamName",
                    OldValue = "",
                    NewValue = "root",
                },
                new()
                {
                    Property = "BusinessUnit",
                    OldValue = "",
                    NewValue = "Test Bu",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Recursively created a new team with properties: TeamName = root, BusinessUnit = Test Bu"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogUpdatedTeam_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Recursively",
            Author = new IdentityUser { Email = "Recursively" },
            Action = Action.UPDATED_TEAM,
            TeamName = "root",
            Changes =
            [
                new()
                {
                    Property = "TeamName",
                    OldValue = "root",
                    NewValue = "New_Team_Name",
                },
            ],
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo(
                    "Recursively updated team root:  set TeamName from root to New_Team_Name"
                )
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public void ConvertToLogDeletedTeam_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Recursively",
            Author = new IdentityUser { Email = "Recursively" },
            Action = Action.REMOVED_TEAM,
            TeamName = "New_Team_Name",
        };

        var logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(
                logResponse.LogMessage,
                Is.EqualTo("Recursively removed team New_Team_Name")
            );
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }
}
