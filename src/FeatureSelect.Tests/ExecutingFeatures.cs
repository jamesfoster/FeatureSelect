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

        [Test]
        public void When_a_feature_is_disabled_executes_disabled_callback_no_result()
        {
            var sut = new DisabledFeatureSourceStub();

            sut.GetFeature("my-feature")
                .Execute(
                    ifEnabled: () => Assert.Fail("Feature should be disabled"),
                    ifDisabled: () => { }
                );
        }

        [Test]
        public void When_a_feature_is_enabled_executes_enabled_callback_no_result()
        {
            var sut = new EnabledFeatureSourceStub();

            sut.GetFeature("my-feature")
                .Execute(
                    ifEnabled: () => { },
                    ifDisabled: () => Assert.Fail("Feature should be disabled")
                );
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