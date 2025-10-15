using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Casbin;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class PutRuleCommandHandlerTest
{
    private PutRuleCommandHandler _handler;

    private IMock<IEnforcer> _enforcer;

    private IMock<ILogRepository> _logRepository;

    private IMock<IUnitOfWork> _unitOfWork;

    [SetUp]
    public void SetUp()
    {
        _enforcer = new Mock<IEnforcer>();
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
                Logic = Logic.AND,
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
}
