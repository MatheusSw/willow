using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using admin_api;
using admin_api.DTOs.Request;
using admin_api.DTOs.Response;
using admin_infrastructure.Db;
using admin_infrastructure.Db.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace admin_integration_tests.Controllers;

public class FeaturesControllerIntegrationTests : IClassFixture<AdminApiFactory>
{
    private readonly AdminApiFactory _factory;

    public FeaturesControllerIntegrationTests(AdminApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Feature_CRUD_Workflow()
    {
        // Arrange: seed org, project, api key
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FeatureToggleDbContext>();
        await db.Database.MigrateAsync();

        var org = new Organization { Id = Guid.NewGuid(), Name = $"org-{Guid.NewGuid():N}" };
        var project = new Project { Id = Guid.NewGuid(), OrgId = org.Id, Name = $"proj-{Guid.NewGuid():N}" };

        var rawKey = $"k_{Guid.NewGuid():N}";
        var hashed = ComputeSha256Base64(rawKey);
        var apiKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            OrgId = org.Id,
            ProjectId = project.Id,
            Role = "admin",
            Scopes = new[] { "features:read", "features:write" },
            HashedKey = hashed,
            Active = true
        };

        await db.Organizations.AddAsync(org);
        await db.Projects.AddAsync(project);
        await db.ApiKeys.AddAsync(apiKey);
        await db.SaveChangesAsync();

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", rawKey);

        var createdIds = new List<Guid>();

        try
        {
            // POST create feature
            var createReq = new CreateFeatureRequest
            {
                ProjectId = project.Id,
                Name = $"feat-{Guid.NewGuid():N}",
                Description = "first desc"
            };

            var postContent = new StringContent(JsonSerializer.Serialize(createReq), Encoding.UTF8, "application/json");
            var postResp = await client.PostAsync("v1/features", postContent);
            postResp.EnsureSuccessStatusCode();
            var created = await Deserialize<FeatureResponse>(postResp);
            Assert.Equal(createReq.ProjectId, created.ProjectId);
            Assert.Equal(createReq.Name, created.Name);
            Assert.Equal(createReq.Description, created.Description);
            Assert.NotEqual(Guid.Empty, created.Id);
            createdIds.Add(created.Id);

            // GET all (optionally filter by projectId)
            var listResp = await client.GetAsync($"/v1/features?projectId={project.Id}");
            if (listResp.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Assert.Fail("Expected features list to contain at least the created item, but got 204");
            }
            listResp.EnsureSuccessStatusCode();
            var list = await Deserialize<List<FeatureResponse>>(listResp);
            Assert.Contains(list, f => f.Id == created.Id && f.ProjectId == project.Id && f.Name == createReq.Name);

            // GET by id
            var getResp = await client.GetAsync($"/v1/features/{created.Id}");
            getResp.EnsureSuccessStatusCode();
            var got = await Deserialize<FeatureResponse>(getResp);
            Assert.Equal(created.Id, got.Id);
            Assert.Equal(created.ProjectId, got.ProjectId);
            Assert.Equal(created.Name, got.Name);
            Assert.Equal(created.Description, got.Description);

            // PUT update
            var updateReq = new UpdateFeatureRequest
            {
                ProjectId = project.Id,
                Name = $"{createReq.Name}-updated",
                Description = "updated desc"
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updateReq), Encoding.UTF8, "application/json");
            var putResp = await client.PutAsync($"/v1/features/{created.Id}", putContent);
            putResp.EnsureSuccessStatusCode();
            var updated = await Deserialize<FeatureResponse>(putResp);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal(updateReq.ProjectId, updated.ProjectId);
            Assert.Equal(updateReq.Name, updated.Name);
            Assert.Equal(updateReq.Description, updated.Description);
        }
        finally
        {
            // Cleanup created feature via API if present, else via DB
            using var cleanupScope = _factory.Services.CreateScope();
            var cleanupDb = cleanupScope.ServiceProvider.GetRequiredService<FeatureToggleDbContext>();

            if (createdIds.Count > 0)
            {
                foreach (var id in createdIds)
                {
                    var f = await cleanupDb.Features.FirstOrDefaultAsync(x => x.Id == id);
                    if (f != null)
                    {
                        cleanupDb.Features.Remove(f);
                    }
                }
            }

            // Also cleanup seed data
            var api = await cleanupDb.ApiKeys.FirstOrDefaultAsync(x => x.Id == apiKey.Id);
            if (api != null) cleanupDb.ApiKeys.Remove(api);
            var proj = await cleanupDb.Projects.FirstOrDefaultAsync(x => x.Id == project.Id);
            if (proj != null) cleanupDb.Projects.Remove(proj);
            var orgn = await cleanupDb.Organizations.FirstOrDefaultAsync(x => x.Id == org.Id);
            if (orgn != null) cleanupDb.Organizations.Remove(orgn);

            await cleanupDb.SaveChangesAsync();
        }
    }

    private static async Task<T> Deserialize<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    private static string ComputeSha256Base64(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}


