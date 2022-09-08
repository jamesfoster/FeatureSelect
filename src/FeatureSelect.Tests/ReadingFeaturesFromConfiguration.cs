using Microsoft.Extensions.Configuration;

namespace FeatureSelect.Tests
{
    public class ReadingFeaturesFromConfiguration
    {
        internal ConfigurationFeatureSource SUT { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("sample.json", optional: false)
                .Build();

            SUT = new ConfigurationFeatureSource(config.GetSection("Features"));
        }

        [Test]
        public void Unknown_feature_is_disabled()
        {
            AssertDisabled("my-unknown-feature");
        }

        [Test]
        public void Disabled_feature_is_disabled()
        {
            AssertDisabled("my-disabled-feature");
        }

        [Test]
        public void Enabled_feature_is_enabled()
        {
            AssertEnabled("my-enabled-feature");
        }

        private void AssertDisabled(string name)
        {
            var feature = SUT.GetFeature(name);

            var result = feature.Execute(() => "enabled", () => "disabled");

            Assert.That(result, Is.EqualTo("disabled"));
        }

        private void AssertEnabled(string name)
        {
            var feature = SUT.GetFeature(name);

            var result = feature.Execute(() => "enabled", () => "disabled");

            Assert.That(result, Is.EqualTo("enabled"));
        }
    }
}