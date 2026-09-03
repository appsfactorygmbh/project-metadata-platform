using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure;
using Microsoft.Identity.Client.Extensibility;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class PluginBillingManagement : IntegrationTestsBase
{
    private int _projectId = 0;
    private int _pluginId = 0;

    private int _globalBillingId = 0;

    private static StringContent CreateRequest(int billingId) =>
        StringContent(
            """{ "displayName": "gitLab", "currency": "de-de", "budgetLimit": 500, "hostingFee": 300, "targetMargin": 30, "timeFrame": "NEVER", "billingId": """
                + billingId.ToString()
                + """  }"""
        );

    private static StringContent CreateRequest2(int billingId) =>
        StringContent(
            """{ "displayName": "devOps", "currency": "de-at", "budgetLimit": 501, "hostingFee": 301, "targetMargin": 0, "timeFrame": "DATE", "date": "2012-04-21T18:25:43-00:00", "billingId": """
                + billingId.ToString()
                + """  }"""
        );

    private static StringContent InvalidRequest1(int billingId) =>
        StringContent(
            """{ "displayName": "devOps", "currency": "de-at", "budgetLimit": 501, "hostingFee": 301, "targetMargin": 0, "timeFrame": "DATE", "billingId": """
                + billingId.ToString()
                + """  }"""
        );

    private static StringContent InvalidRequest2(int billingId) =>
        StringContent(
            """{ "displayName": "gitLab", "currency": "de-de", "budgetLimit": 500, "hostingFee": 300, "targetMargin": 30, "timeFrame": "NEVER", "notes": " """
                + new string('a', 291)
                + """ ", "billingId": """
                + billingId.ToString()
                + """  }"""
        );

    [SetUp]
    public async Task Setup()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        _projectId = await CreateProject(client, "Example Project");
        _pluginId = await CreateProjectPlugin(client, "Plugin");
        _globalBillingId = await CreateGlobalBilling(client);
    }

    [Test]
    public async Task CreatePluginBillingObject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var result = (
            await client.PutAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                CreateRequest(_globalBillingId)
            )
        );
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var billing = (
            await ToJsonElement(
                client.GetAsync($"/Projects/{_projectId}/plugins/{_pluginId}/billing")
            )
        );

        Assert.Multiple(() =>
        {
            Assert.That(billing.GetProperty("displayName").GetString(), Is.EqualTo("gitLab"));
            Assert.That(billing.GetProperty("currency").GetString(), Is.EqualTo("de-de"));
            Assert.That(billing.GetProperty("budgetLimit").GetDecimal(), Is.EqualTo(500));
            Assert.That(billing.GetProperty("hostingFee").GetDecimal(), Is.EqualTo(300));
            Assert.That(billing.GetProperty("targetMargin").GetInt32(), Is.EqualTo(30));
            Assert.That(billing.GetProperty("timeFrame").GetString(), Is.EqualTo("NEVER"));
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(6));

            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added new billing information to project testProject with properties: ProjectPlugin = Plugin, DisplayName = gitLab, GlobalBilling = devOps, BudgetLimit = 500, HostingFee = 300, Currency = de-de, TargetMargin = 30, TimeFrame = NEVER, Notes = null"
                )
            );
        });
    }

    [Test]
    public async Task UpdatePluginBilling()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var result = (
            await client.PutAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                CreateRequest(_globalBillingId)
            )
        );
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var updatedPluginBilling = await ToJsonElement(
            client.PatchAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                CreateRequest2(_globalBillingId)
            )
        );
        Assert.Multiple(() =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    updatedPluginBilling.GetProperty("displayName").GetString(),
                    Is.EqualTo("devOps")
                );
                Assert.That(
                    updatedPluginBilling.GetProperty("currency").GetString(),
                    Is.EqualTo("de-at")
                );
                Assert.That(
                    updatedPluginBilling.GetProperty("budgetLimit").GetDecimal(),
                    Is.EqualTo(501)
                );
                Assert.That(
                    updatedPluginBilling.GetProperty("hostingFee").GetDecimal(),
                    Is.EqualTo(301)
                );
                Assert.That(
                    updatedPluginBilling.GetProperty("targetMargin").GetInt32(),
                    Is.EqualTo(0)
                );
                Assert.That(
                    updatedPluginBilling.GetProperty("timeFrame").GetString(),
                    Is.EqualTo("DATE")
                );
                Assert.That(
                    updatedPluginBilling.GetProperty("date").GetString(),
                    Is.EqualTo("2012-04-21T18:25:43+00:00")
                );
            });
        });

        var billing = (
            await ToJsonElement(
                client.GetAsync($"/Projects/{_projectId}/plugins/{_pluginId}/billing")
            )
        );
        Assert.Multiple(() =>
        {
            Assert.That(
                updatedPluginBilling.GetProperty("displayName").GetString(),
                Is.EqualTo("devOps")
            );
            Assert.That(
                updatedPluginBilling.GetProperty("currency").GetString(),
                Is.EqualTo("de-at")
            );
            Assert.That(
                updatedPluginBilling.GetProperty("budgetLimit").GetDecimal(),
                Is.EqualTo(501)
            );
            Assert.That(
                updatedPluginBilling.GetProperty("hostingFee").GetDecimal(),
                Is.EqualTo(301)
            );
            Assert.That(updatedPluginBilling.GetProperty("targetMargin").GetInt32(), Is.EqualTo(0));
            Assert.That(
                updatedPluginBilling.GetProperty("timeFrame").GetString(),
                Is.EqualTo("DATE")
            );
            Assert.That(
                updatedPluginBilling.GetProperty("date").GetString(),
                Is.EqualTo("2012-04-21T18:25:43+00:00")
            );
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(7));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added new billing information to project testProject with properties: ProjectPlugin = Plugin, DisplayName = gitLab, GlobalBilling = devOps, BudgetLimit = 500, HostingFee = 300, Currency = de-de, TargetMargin = 30, TimeFrame = NEVER, Notes = null"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin updated billing information in project testProject: set DisplayName from gitLab to devOps, set BudgetLimit from 500 to 501, set HostingFee from 300 to 301, set Currency from de-de to de-at, set TargetMargin from 30 to 0, set TimeFrame from NEVER to 21.04.2012 00:00:00"
                )
            );
        });
    }

    [Test]
    public async Task DeletePluginBilling()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var result = (
            await client.PutAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                CreateRequest(_globalBillingId)
            )
        );
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var response = await client.DeleteAsync(
            $"/Projects/{_projectId}/plugins/{_pluginId}/billing"
        );
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var errorResponse = await ToErrorResponse(
            client.GetAsync($"/Projects/{_projectId}/plugins/{_pluginId}/billing"),
            HttpStatusCode.NotFound
        );

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(7));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added new billing information to project testProject with properties: ProjectPlugin = Plugin, DisplayName = gitLab, GlobalBilling = devOps, BudgetLimit = 500, HostingFee = 300, Currency = de-de, TargetMargin = 30, TimeFrame = NEVER, Notes = null"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin removed billing information from project testProject")
            );
        });
    }

    [Test]
    public async Task PluginBillingMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var response = await client.PutAsync(
            $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
            CreateRequest(_globalBillingId)
        );
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var errorResponse = await ToErrorResponse(
            client.PutAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                CreateRequest2(_globalBillingId)
            ),
            HttpStatusCode.Conflict
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("Billing information already exists for the plugin with the id 1.")
        );
    }

    [Test]
    public async Task PluginBillingTypeDateNeedsDate()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var errorResponse = await ToErrorResponse(
            client.PutAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                InvalidRequest1(_globalBillingId)
            ),
            HttpStatusCode.BadRequest
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("Billing Information of this type needs a date.")
        );

        var response = await client.PutAsync(
            $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
            CreateRequest(_globalBillingId)
        );
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var errorResponse2 = await ToErrorResponse(
            client.PatchAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                InvalidRequest1(_globalBillingId)
            ),
            HttpStatusCode.BadRequest
        );

        // Assert
        Assert.That(
            errorResponse2.Message,
            Is.EqualTo("Billing Information of this type needs a date.")
        );
    }

    [Test]
    public async Task PluginBillingNotesCantBeBiggerThan280Chars()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var errorResponse = await ToErrorResponse(
            client.PutAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                InvalidRequest2(_globalBillingId)
            ),
            HttpStatusCode.BadRequest
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("The billing notes are 293 chars long. Maximum allowed is 280 chars.")
        );

        var response = await client.PutAsync(
            $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
            CreateRequest(_globalBillingId)
        );
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var errorResponse2 = await ToErrorResponse(
            client.PatchAsync(
                $"/Projects/{_projectId}/plugins/{_pluginId}/billing",
                InvalidRequest2(_globalBillingId)
            ),
            HttpStatusCode.BadRequest
        );

        // Assert
        Assert.That(
            errorResponse2.Message,
            Is.EqualTo("The billing notes are 293 chars long. Maximum allowed is 280 chars.")
        );
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidPluginId([Values] bool patch)
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = patch
            ? client.PatchAsync(
                $"/Projects/{_projectId}/plugins/99/billing",
                CreateRequest(_globalBillingId)
            )
            : client.DeleteAsync($"/Projects/{_projectId}/plugins/99/billing");

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo(
                $"No billing information for the plugin with the id 99 of the project with the id {_projectId} was found."
            )
        );
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidProjectId([Values] bool patch)
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = patch
            ? client.PatchAsync(
                $"/Projects/999/plugins/{_pluginId}/billing",
                CreateRequest(_globalBillingId)
            )
            : client.DeleteAsync($"/Projects/999/plugins/{_pluginId}/billing");

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo(
                $"No billing information for the plugin with the id {_pluginId} of the project with the id 999 was found."
            )
        );
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidGlobalBillingId()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = client.PutAsync(
            $"/Projects/{_projectId}/plugins/{_pluginId}/billing/",
            CreateRequest(999)
        );

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("The billing information with id 999 was not found.")
        );
    }

    private static async Task<int> CreateProject(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync(
                    "/Projects",
                    StringContent(
                        """
                            {
                              "projectName": "testProject",
                              "clientName": "testClient",
                              "companyId":
                        """
                            + await CreateCompany(client, "Default")
                            + """
                                  ,
                                  "companyState": "EXTERNAL",
                                  "ismsLevel": "NORMAL",
                                  "isEoC": false,
                                  "notes": "Example Notes"
                                }
                            """
                    )
                ),
                HttpStatusCode.Created
            )
        ).GetProperty("id").GetInt32();
    }

    private static async Task<int> CreateGlobalBilling(HttpClient client)
    {
        return (
            await ToJsonElement(
                client.PutAsync("/Billing", StringContent("""{ "billingKind": "devOps" }""")),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }

    private static async Task<int> CreateCompany(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync("/Companies", StringContent($"{{ \"companyName\": \"{name}\"}}")),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }

    private async Task<int> CreateProjectPlugin(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync(
                    $"/Projects/{_projectId}/plugins",
                    StringContent(
                        $"{{ \"url\": \"www.{name}.com\", \"displayName\": \"{name}\", \"pluginId\": {await CreateGlobalPlugin(client, name)}}}"
                    )
                ),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }

    private static async Task<int> CreateGlobalPlugin(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync(
                    "/Plugins",
                    StringContent(
                        $"{{ \"baseUrl\": \"www.{name}.com\", \"isArchived\": false, \"keys\": [], \"pluginName\": \"{name}\"}}"
                    )
                ),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }
}
