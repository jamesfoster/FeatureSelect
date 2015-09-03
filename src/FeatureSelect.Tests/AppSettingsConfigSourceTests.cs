namespace FeatureSelect.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class AppSettingsConfigSourceTests
    {
        private AppSettingsFeatureSource source;

        [SetUp]
        public void SetUp()
        {
            source = new AppSettingsFeatureSource();
        }

        [Test]
        public void If_feature_is_missing_in_config_then_return_null()
        {
            var feature = source.GetFeature("MissingFeature");

            Assert.That(feature, Is.Null);
        }

        [Test]
        public void If_feature_present_but_is_invalid_then_feature_is_invalid()
        {
            var feature = source.GetFeature("InvalidFeature");

            Assert.That(feature, Is.InstanceOf<InvalidFeature>());
        }

        [Test]
        public void If_feature_is_on_in_config_then_feature_is_on()
        {
            var feature = source.GetFeature("OnFeature");

            Assert.That(feature, Is.InstanceOf<OnFeature>());
        }

        [Test]
        public void If_feature_is_off_in_config_then_feature_is_off()
        {
            var feature = source.GetFeature("OffFeature");

            Assert.That(feature, Is.InstanceOf<OffFeature>());
        }

        [Test]
        public void If_feature_is_property_in_config_then_feature_is_PropetyFeature_with_correct_values()
        {
            var feature = source.GetFeature("PropertyFeature");

            var propertyFeature = (PropertyFeature)feature;

            Assert.That(propertyFeature.Property, Is.EqualTo("Foo"));
        }

        [Test]
        public void If_you_modify_the_source_at_runtime_you_get_the_new_expected_value()
        {
            source.FeatureSettings["OnFeature"] = "Off";

            var feature = source.GetFeature("OnFeature");

            Assert.That(feature, Is.InstanceOf<OffFeature>());
        }
    }
}
