
namespace FeatureSelect.Tests
{
    using Moq;

    using NUnit.Framework;

    using Ploeh.AutoFixture;


    [TestFixture]
    public class FeatureSelectorTests
    {
        private readonly Fixture fixture = new Fixture();

        private FeatureSelector featureSelector;

        public class Given_a_new_FeatureSelector : FeatureSelectorTests
        {
            private string featureName;

            [SetUp]
            public void SetUp()
            {
                featureName = fixture.Create<string>();
                featureSelector = new FeatureSelector();
            }

            [Test]
            public void The_feature_is_disabled()
            {
                AssertFeatureDisabled();
            }

            public class When_adding_a_FeatureSource : Given_a_new_FeatureSelector
            {
                private Mock<IFeatureSource> source;

                [SetUp]
                public void SetUp()
                {
                    source = new Mock<IFeatureSource>();

                    featureSelector.AddFeatureSource(source.Object);
                }

                [Test]
                public void There_should_be_one_source()
                {
                    Assert.That(featureSelector.Sources, Has.Exactly(1).EqualTo(source.Object));
                }

                public class Static_feature_selection : When_adding_a_FeatureSource
                {
                    [Test]
                    public void If_feature_is_missing_from_all_config_sources_then_the_feature_is_disabled()
                    {
                        source
                            .Setup(x => x.GetFeature(featureName))
                            .Returns(new InvalidFeature());

                        AssertFeatureDisabled();
                    }

                    [Test]
                    public void If_feature_is_On_in_a_config_source_then_the_feature_is_enabled()
                    {
                        source
                            .Setup(x => x.GetFeature(featureName))
                            .Returns(new OnFeature());

                        AssertFeatureEnabled();
                    }

                    [Test]
                    public void If_feature_is_Off_in_a_config_source_then_the_feature_is_disabled()
                    {
                        source
                            .Setup(x => x.GetFeature(featureName))
                            .Returns(new OffFeature());

                        AssertFeatureDisabled();
                    }
                }

                public class Given_the_feature_is_set_to_Property_Foo_equal_to_A_B_or_C : When_adding_a_FeatureSource
                {
                    private PropertyFeature feature;

                    [SetUp]
                    public void SetUp()
                    {
                        feature = new PropertyFeature("Foo", new[] {"A", "B", "C"});

                        source
                            .Setup(x => x.GetFeature(featureName))
                            .Returns(feature);
                    }

                    [Test]
                    public void If_the_context_is_null_then_the_feature_is_disabled()
                    {
                        AssertFeatureDisabled(null);
                    }

                    [Test]
                    public void If_the_context_is_an_object_without_the_Foo_property_then_the_feature_is_disabled()
                    {
                        var context = new
                            {
                                Bar = "baz"
                            };

                        AssertFeatureDisabled(context);
                    }

                    [Test]
                    public void If_the_context_is_an_object_with_a_Foo_property_equal_to_B_then_the_feature_is_enabled()
                    {
                        var context = new
                            {
                                Foo = "B"
                            };

                        AssertFeatureEnabled(context);
                    }

                    [Test]
                    public void The_feature_is_enabled_even_if_lowercase_and_padded_with_spaces()
                    {
                        var context = new
                            {
                                Foo = "b "
                            };

                        AssertFeatureEnabled(context);
                    }

                    [Test]
                    public void If_the_context_is_an_object_with_a_Foo_property_equal_to_D_then_the_feature_is_disabled()
                    {
                        var context = new
                            {
                                Foo = "D"
                            };

                        AssertFeatureDisabled(context);
                    }

                    private void AssertFeatureEnabled(object context)
                    {
                        var enabled = featureSelector.Enabled(featureName, context);
                        Assert.That(enabled, Is.True);
                        var disabled = featureSelector.Disabled(featureName, context);
                        Assert.That(disabled, Is.False);
                    }

                    private void AssertFeatureDisabled(object context)
                    {
                        var enabled = featureSelector.Enabled(featureName, context);
                        Assert.That(enabled, Is.False);
                        var disabled = featureSelector.Disabled(featureName, context);
                        Assert.That(disabled, Is.True);
                    }
                }
            }

            private void AssertFeatureEnabled()
            {
                var enabled = featureSelector.Enabled(featureName);
                Assert.That(enabled, Is.True);
                var disabled = featureSelector.Disabled(featureName);
                Assert.That(disabled, Is.False);
            }

            private void AssertFeatureDisabled()
            {
                var enabled = featureSelector.Enabled(featureName);
                Assert.That(enabled, Is.False);
                var disabled = featureSelector.Disabled(featureName);
                Assert.That(disabled, Is.True);
            }
        }
    }
}
