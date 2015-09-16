
namespace FeatureSelect.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using NUnit.Framework;

    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;


    [TestFixture]
    public class FeatureSelectorTests
    {
        private readonly Fixture fixture = new Fixture();

        private FeatureSelector featureSelector;

        [SetUp]
        public void SetUp()
        {
            fixture.Customize(new AutoMoqCustomization());
        }

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
                    source = fixture.Create<Mock<IFeatureSource>>();

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
                            .Returns(new InvalidFeature(featureName));

                        AssertFeatureDisabled();
                    }

                    [Test]
                    public void If_feature_is_On_in_a_config_source_then_the_feature_is_enabled()
                    {
                        source
                            .Setup(x => x.GetFeature(featureName))
                            .Returns(new OnFeature(featureName));

                        AssertFeatureEnabled();
                    }

                    [Test]
                    public void If_feature_is_Off_in_a_config_source_then_the_feature_is_disabled()
                    {
                        source
                            .Setup(x => x.GetFeature(featureName))
                            .Returns(new OffFeature(featureName));

                        AssertFeatureDisabled();
                    }
                }

                public class Given_the_feature_is_set_to_Property_Foo_equal_to_A_B_or_C : When_adding_a_FeatureSource
                {
                    private PropertyFeature feature;

                    [SetUp]
                    public void SetUp()
                    {
                        var options = new Dictionary<string, string> {{"Foo", "A, B, C"}};
                        feature = new PropertyFeature(featureName, options);

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
                    public void The_feature_is_enabled_even_if_wrong_case_and_padded_with_spaces()
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

                public class When_listing_the_features : When_adding_a_FeatureSource
                {
                    [Test]
                    public void Requests_the_list_of_features_from_the_source()
                    {
                        featureSelector.ListFeatures();

                        source.Verify(x => x.ListFeatures());
                    }

                    [Test]
                    public void Returns_the_list_of_features_from_the_source()
                    {
                        var features = fixture.CreateMany<TestFeature>().ToList();

                        source.Setup(x => x.ListFeatures()).Returns(features);

                        var result = featureSelector.ListFeatures();

                        Assert.That(result, Is.EqualTo(features.OrderBy(x => x.Name)));
                    }

                    [Test]
                    public void Returns_the_list_of_features_from_all_sources()
                    {
                        var source2 = fixture.Create<Mock<IFeatureSource>>();
                        featureSelector.AddFeatureSource(source2.Object);

                        var features = fixture.CreateMany<TestFeature>().ToArray();
                        var features2 = fixture.CreateMany<TestFeature>().ToArray();

                        source.Setup(x => x.ListFeatures()).Returns(features);
                        source2.Setup(x => x.ListFeatures()).Returns(features2);

                        var result = featureSelector.ListFeatures();

                        Assert.That(result, Is.EqualTo(features.Concat(features2).OrderBy(x => x.Name)));
                    }

                    [Test]
                    public void Returns_the_first_feature_with_a_given_name_from_all_sources()
                    {
                        var source2 = fixture.Create<Mock<IFeatureSource>>();
                        featureSelector.AddFeatureSource(source2.Object);

                        var features = fixture.CreateMany<TestFeature>().ToArray();
                        var features2 =
                            from f in features
                            let clone = fixture.Build<TestFeature>().With(y => y.Name, f.Name).Create()
                            select clone;

                        source.Setup(x => x.ListFeatures()).Returns(features);
                        source2.Setup(x => x.ListFeatures()).Returns(features2);

                        var result = featureSelector.ListFeatures();

                        Assert.That(result, Is.EqualTo(features.OrderBy(x => x.Name)));
                    }

                    [Test]
                    public void Includes_any_unknown_features_requested()
                    {
                        var unknownFeatureName = fixture.Create<string>();

                        source.Setup(x => x.GetFeature(unknownFeatureName)).Returns((IFeature) null);

                        featureSelector.Enabled(unknownFeatureName);

                        var result = featureSelector.ListFeatures();

                        Assert.That(result, Has.Exactly(1).Property("Name").EqualTo(unknownFeatureName).And.TypeOf<InvalidFeature>());
                    }
                }


                public class When_setting_a_features : When_adding_a_FeatureSource
                {
                    [Test]
                    public void Sets_the_feature_on_the_source()
                    {
                        featureSelector.SetFeature("A", "B");

                        source.Verify(x => x.SetFeature("A", "B", null));
                    }

                    [Test]
                    public void If_there_are_multiple_sources_sets_it_on_the_first_source_if_it_returns_true()
                    {
                        var source2 = fixture.Create<Mock<IFeatureSource>>();
                        featureSelector.AddFeatureSource(source2.Object);

                        source.Setup(x => x.SetFeature("A", "B", null)).Returns(true);
                        source2.Setup(x => x.SetFeature("A", "B", null)).Returns(false);

                        featureSelector.SetFeature("A", "B");

                        source.Verify(x => x.SetFeature("A", "B", null));
                        source2.Verify(x => x.SetFeature("A", "B", null), Times.Never());
                    }

                    [Test]
                    public void If_there_are_multiple_sources_continues_setting_it_if_the_previous_sources_returned_false()
                    {
                        var source2 = fixture.Create<Mock<IFeatureSource>>();
                        featureSelector.AddFeatureSource(source2.Object);

                        source.Setup(x => x.SetFeature("A", "B", null)).Returns(false);
                        source2.Setup(x => x.SetFeature("A", "B", null)).Returns(false);

                        featureSelector.SetFeature("A", "B");

                        source.Verify(x => x.SetFeature("A", "B", null));
                        source2.Verify(x => x.SetFeature("A", "B", null));
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

            private class TestFeature : IFeature
            {
                public string Name { get; set; }

                public bool IsEnabled(object context)
                {
                    return false;
                }
            }
        }
    }
}
