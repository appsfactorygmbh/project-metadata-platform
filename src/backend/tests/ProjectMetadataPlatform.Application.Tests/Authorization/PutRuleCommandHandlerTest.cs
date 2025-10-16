using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using Casbin.Model;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Logs;
using static ProjectMetadataPlatform.Domain.Authorization.PolicyRule;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class PutRuleCommandHandlerTest
{
    private PutRuleCommandHandler _handler;

    private Mock<IEnforcerWrapper> _enforcer;

    private Mock<ILogRepository> _logRepository;

    private Mock<IUnitOfWork> _unitOfWork;

    [SetUp]
    public void SetUp()
    {
        _enforcer = new Mock<IEnforcerWrapper>();
        _logRepository = new Mock<ILogRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new PutRuleCommandHandler(
            _enforcer.Object,
            _logRepository.Object,
            _unitOfWork.Object
        );
    }

    [Test]
    public async Task ConvertToPolicyRuleNoRuleElementGroups()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup> { };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("true"));
    }

    [Test]
    public async Task ConvertToPolicyRuleEmptyRuleElementGroups()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup { Logic = Logic.AND },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("true"));
    }

    [Test]
    public async Task ConvertToPolicyRuleNoRuleElement()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup { Logic = Logic.AND, RuleElements = [] },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("true"));
    }

    [Test]
    public async Task ConvertToPolicyRuleOneRuleElementEqual()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.AND,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.EQUAL,
                        Value = JsonSerializer.Serialize(true),
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("(Test.TestAttribute == true)"));
    }

    [Test]
    public async Task ConvertToPolicyRuleOneRuleElementUnEqual()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.AND,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.UNEQUAL,
                        Value = JsonSerializer.Serialize(false),
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("(Test.TestAttribute != false)"));
    }

    [Test]
    public async Task ConvertToPolicyRuleOneRuleElementContains()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.AND,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.CONTAINS,
                        Value = 32,
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("(Test.TestAttribute.Contains(32))"));
    }

    [Test]
    public async Task ConvertToPolicyRuleOneRuleElementNotIn()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.AND,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.NOT_IN,
                        Value = 32,
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("(!Test.TestAttribute.Contains(32))"));
    }

    [Test]
    public async Task ConvertToPolicyRuleOneRuleElementStartsWith()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.AND,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.STARTS_WITH,
                        Value = JsonSerializer.Serialize("TestValue"),
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("(Test.TestAttribute.StartsWith(\"TestValue\"))"));
    }

    [Test]
    public async Task ConvertToPolicyRuleOneRuleElementEndsWith()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.AND,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.STARTS_WITH,
                        Value = JsonSerializer.Serialize("TestValue"),
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("(Test.TestAttribute.StartsWith(\"TestValue\"))"));
    }

    [Test]
    public async Task ConvertToPolicyRuleOneRuleElementEmpty()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.OR,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.EMPTY,
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(result, Is.EqualTo("(!Test.TestAttribute.Any())"));
    }

    [Test]
    public async Task ConvertToPolicyRuleMultipleRuleElements()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.OR,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.EMPTY,
                    },
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "OtherTestAttribute",
                        Operation = Operation.CONTAINS,
                        Value = 22,
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(
            result,
            Is.EqualTo("(!Test.TestAttribute.Any() || Test.OtherTestAttribute.Contains(22))")
        );
    }

    [Test]
    public async Task ConvertToPolicyRuleMultipleRuleGroups()
    {
        var ruleElementGroups = new List<PolicyRule.RuleElementGroup>
        {
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.OR,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "TestAttribute",
                        Operation = Operation.EMPTY,
                    },
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "OtherTestAttribute",
                        Operation = Operation.CONTAINS,
                        Value = 22,
                    },
                },
            },
            new PolicyRule.RuleElementGroup
            {
                Logic = Logic.AND,
                RuleElements = new List<PolicyRule.RuleElementGroup.RuleElement>
                {
                    new PolicyRule.RuleElementGroup.RuleElement
                    {
                        Attribute = "AnotherTestAttribute",
                        Operation = Operation.EQUAL,
                        Value = JsonSerializer.Serialize("TestValue"),
                    },
                },
            },
        };
        var result = PutRuleCommandHandler.ConvertToPolicyRuleString(ruleElementGroups, "Test");
        Assert.That(
            result,
            Is.EqualTo(
                "(!Test.TestAttribute.Any() || Test.OtherTestAttribute.Contains(22)) && (Test.AnotherTestAttribute == \"TestValue\")"
            )
        );
    }

    [Test]
    public async Task HandlePutRuleCommandSuccesful()
    {
        _enforcer.Setup(m => m.AddPolicyAsync(It.IsAny<string[]>())).ReturnsAsync(true);
        _enforcer.Setup(m => m.SavePolicyAsync()).ReturnsAsync(true);
        var rule = new PolicyRule
        {
            Action = "All",
            Effect = Effect.ALLOW,
            SubjectRule = new List<RuleElementGroup>
            {
                new RuleElementGroup
                {
                    Logic = Logic.AND,
                    RuleElements = new List<RuleElementGroup.RuleElement>
                    {
                        new RuleElementGroup.RuleElement
                        {
                            Attribute = "TestAttribute",
                            Operation = Operation.EQUAL,
                            Value = 20,
                        },
                    },
                },
            },
            ObjectRule = new List<RuleElementGroup>
            {
                new RuleElementGroup
                {
                    Logic = Logic.AND,
                    RuleElements = new List<RuleElementGroup.RuleElement>
                    {
                        new RuleElementGroup.RuleElement
                        {
                            Attribute = "OtherTestAttribute",
                            Operation = Operation.EMPTY,
                        },
                    },
                },
            },
        };
        var command = new PutRuleCommand(rule);
        var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(true));

        _logRepository.Verify(
            r =>
                r.AddAuthorizationLogForCurrentUser(
                    It.Is<Domain.Logs.Action>(action => action == Domain.Logs.Action.ADDED_RULE),
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 1
                        && changes[0].OldValue == ""
                        && changes[0].NewValue
                            == "(r.sub.TestAttribute == 20) && (!r.obj.OtherTestAttribute.Any()) && true && \"All\" && allow"
                        && changes[0].Property == nameof(command.PolicyRule)
                    )
                ),
            Times.Once
        );
        _enforcer.Verify(
            e =>
                e.AddPolicyAsync(
                    It.Is<string>(sub_rule => sub_rule == "(r.sub.TestAttribute == 20)"),
                    It.Is<string>(obj_rule => obj_rule == "(!r.obj.OtherTestAttribute.Any())"),
                    It.Is<string>(env_rule => env_rule == "true"),
                    It.Is<string>(action => action == "All"),
                    It.Is<string>(effect => effect == "allow")
                ),
            Times.Once
        );
        _enforcer.Verify(e => e.SavePolicyAsync(), Times.Once);

        _unitOfWork.Verify(r => r.CompleteAsync(), Times.Once);
    }

    [Test]
    public async Task HandlePutRuleCantAddRule()
    {
        _enforcer.Setup(m => m.AddPolicyAsync(It.IsAny<string[]>())).ReturnsAsync(false);
        _enforcer.Setup(m => m.SavePolicyAsync()).ReturnsAsync(true);
        var rule = new PolicyRule
        {
            Action = "All",
            Effect = Effect.ALLOW,
            SubjectRule = new List<RuleElementGroup>
            {
                new RuleElementGroup
                {
                    Logic = Logic.AND,
                    RuleElements = new List<RuleElementGroup.RuleElement>
                    {
                        new RuleElementGroup.RuleElement
                        {
                            Attribute = "TestAttribute",
                            Operation = Operation.EQUAL,
                            Value = 20,
                        },
                    },
                },
            },
            ObjectRule = new List<RuleElementGroup>
            {
                new RuleElementGroup
                {
                    Logic = Logic.AND,
                    RuleElements = new List<RuleElementGroup.RuleElement>
                    {
                        new RuleElementGroup.RuleElement
                        {
                            Attribute = "OtherTestAttribute",
                            Operation = Operation.EMPTY,
                        },
                    },
                },
            },
        };
        var command = new PutRuleCommand(rule);
        var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(false));

        _logRepository.Verify(
            r =>
                r.AddAuthorizationLogForCurrentUser(
                    It.Is<Domain.Logs.Action>(action => action == Domain.Logs.Action.ADDED_RULE),
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 1
                        && changes[0].OldValue == ""
                        && changes[0].NewValue
                            == "(r.sub.TestAttribute == 20) && (!r.obj.OtherTestAttribute.Any()) && true && \"All\" && allow"
                        && changes[0].Property == nameof(command.PolicyRule)
                    )
                ),
            Times.Once
        );
        _enforcer.Verify(
            e =>
                e.AddPolicyAsync(
                    It.Is<string>(sub_rule => sub_rule == "(r.sub.TestAttribute == 20)"),
                    It.Is<string>(obj_rule => obj_rule == "(!r.obj.OtherTestAttribute.Any())"),
                    It.Is<string>(env_rule => env_rule == "true"),
                    It.Is<string>(action => action == "All"),
                    It.Is<string>(effect => effect == "allow")
                ),
            Times.Once
        );

        _enforcer.Verify(e => e.SavePolicyAsync(), Times.Never);

        _unitOfWork.Verify(r => r.CompleteAsync(), Times.Never);
    }
}
