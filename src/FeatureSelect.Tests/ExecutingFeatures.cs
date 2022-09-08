namespace FeatureSelect.Tests
{
    public class ExecutingFeatures
    {
        [Test]
        public void When_a_feature_is_disabled_executes_disabled_callback()
        {
            var sut = new DisabledFeatureSourceStub();

            var result = sut.GetFeature("my-feature").Execute(() => "enabled", () => "not enabled");

            Assert.That(result, Is.EqualTo("not enabled"));
        }

        [Test]
        public void When_a_feature_is_enabled_executes_enabled_callback()
        {
            var sut = new EnabledFeatureSourceStub();

            var result = sut.GetFeature("my-feature").Execute(() => "enabled", () => "not enabled");

            Assert.That(result, Is.EqualTo("enabled"));
        }
    }

    internal class DisabledFeatureSourceStub : FeatureSource
    {
        public Feature GetFeature(string feature) => Feature.Disabled;
    }

    internal class EnabledFeatureSourceStub : FeatureSource
    {
        public Feature GetFeature(string feature) => Feature.Enabled;
    }
}