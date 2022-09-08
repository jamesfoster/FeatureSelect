using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

using System.Net;

#if NETCOREAPP3_1
using FeatureSelect.Tests.SampleApi.NEW3_1;
#endif

namespace FeatureSelect.Tests
{
    public class DisablingControllerActions
    {
        [Test]
        public async Task IfEnabled_route_allowed_if_feature_enabled()
        {
            var client = BuildClientWithConfig("Features:feature1", "enabled");

            await AssertOk(client, "/feature1", "Feature One");
        }

        [Test]
        public async Task IfEnabled_route_returns_404_if_feature_disabled()
        {
            var client = BuildClientWithConfig("Features:feature1", "disabled");

            await AssertNotFound(client, "/feature1");
        }

        [Test]
        public async Task Two_identical_routes__calls_IfEnabled_version_if_feature_enabled()
        {
            var client = BuildClientWithConfig("Features:feature2", "enabled");

            await AssertOk(client, "/feature2", "Future Feature Two");
        }

        [Test]
        public async Task Two_identical_routes__calls_IfDisabled_version_if_feature_disabled()
        {
            var client = BuildClientWithConfig("Features:feature2", "disabled");

            await AssertOk(client, "/feature2", "Current Feature Two");
        }

        [Test]
        public async Task Can_inject_FeatureSource_as_a_service()
        {
            var client = BuildClientWithConfig("Features:feature3", "enabled");
            await AssertOk(client, "/feature3", "Feature3 is enabled");

            client = BuildClientWithConfig("Features:feature3", "disabled");
            await AssertOk(client, "/feature3", "Feature3 is disabled");
        }

        [Test]
        public async Task IfEnabled_controller_is_removed_if_feature_disabled()
        {
            var client = BuildClientWithConfig(new() {
                { "Features:Sample", "disabled" },
                { "Features:feature1", "enabled" },
                { "Features:feature2", "enabled" }
            });

            await AssertNotFound(client, "/feature1");
            await AssertNotFound(client, "/feature2");
        }

        private async Task AssertNotFound(HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        private async Task AssertOk(HttpClient client, string url, string expectedBody)
        {
            var response = await client.GetAsync(url);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = await response.Content.ReadAsStringAsync();
            Assert.That(body, Is.EqualTo(expectedBody));
        }

        private HttpClient BuildClientWithConfig(string name, string value)
        {
            return BuildClientWithConfig(new Dictionary<string, string> { { name, value } });
        }

        private HttpClient BuildClientWithConfig(Dictionary<string, string> config)
        {
            return new WebApplicationFactory<SampleApiEntryPoint>()
                .WithWebHostBuilder(builder => builder
                    .ConfigureAppConfiguration((_, configBuilder) =>
                        configBuilder.AddInMemoryCollection(config)
                    )
                )
                .CreateClient();
        }
    }
}