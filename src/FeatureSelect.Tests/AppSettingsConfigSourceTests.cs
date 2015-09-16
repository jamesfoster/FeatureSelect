namespace FeatureSelect.Tests
{
    using System.Collections.Generic;

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
            Assert.That(propertyFeature.Values, Is.EqualTo(new[] { "A", "B", "C" }));
        }

        [Test]
        public void Lists_all_features()
        {
            var features = source.ListFeatures();

            Assert.That(features, Has.Exactly(1).Property("Name").EqualTo("OnFeature"));
        }

        [Test]
        public void Setting_a_feature_overrides_the_exiting_feature()
        {
            source.SetFeature("OnFeature", "Off");

            var feature = source.GetFeature("OnFeature");

            Assert.That(feature, Is.InstanceOf<OffFeature>());
        }

        [Test]
        public void Setting_a_feature_also_updates_the_options()
        {
            var options = new Dictionary<string, string> {{"Foo", "Bar, Baz"}};

            source.SetFeature("OnFeature", "Property", options);

            var feature = source.GetFeature("OnFeature");

            var propertyFeature = (PropertyFeature) feature;

            Assert.That(propertyFeature.Property, Is.EqualTo("Foo"));
            Assert.That(propertyFeature.Values, Is.EqualTo(new[] {"Bar", "Baz"}));
        }

        [Test]
        public void Setting_a_feature_also_clears_the_options_if_currently_set()
        {
            var options = new Dictionary<string, string> {{"Bar", "Foo"}};

            source.SetFeature("PropertyFeature", "Property", options);

            var feature = source.GetFeature("PropertyFeature");

            var propertyFeature = (PropertyFeature) feature;

            Assert.That(propertyFeature.Property, Is.EqualTo("Bar"));
            Assert.That(propertyFeature.Values, Is.EqualTo(new[] {"Foo"}));
        }
    }
}
