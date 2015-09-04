namespace FeatureSelect.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class FeatureFactoryTests
    {
        private string featureName;

        [SetUp]
        public void SetUp()
        {
            featureName = "TheFeature";
        }

        [Test]
        public void If_the_state_is_On_then_create_a_OnFeature()
        {
            var feature = new FeatureFactory().Create(featureName, FeatureState.On);

            Assert.That(feature, Is.InstanceOf<OnFeature>());
        }

        [Test]
        public void If_the_state_is_Off_then_create_a_OffFeature()
        {
            var feature = new FeatureFactory().Create(featureName, FeatureState.Off);

            Assert.That(feature, Is.InstanceOf<OffFeature>());
        }

        [Test]
        public void If_the_state_is_Invalid_then_create_a_InvalidFeature()
        {
            var feature = new FeatureFactory().Create(featureName, FeatureState.Invalid);

            Assert.That(feature, Is.InstanceOf<InvalidFeature>());
        }

        [Test]
        public void If_the_state_is_Property_then_create_a_PropertyFeature()
        {
            var options = new Dictionary<string, string>
                {
                    {"Foo", "A,B,C"}
                };

            var feature = new FeatureFactory().Create(featureName, FeatureState.Property, options);

            var propertyFeature = (PropertyFeature) feature;

            Assert.That(propertyFeature.Property, Is.EqualTo("Foo"));
            Assert.That(propertyFeature.Values, Is.EqualTo(new[] {"A", "B", "C"}));
        }
    }
}