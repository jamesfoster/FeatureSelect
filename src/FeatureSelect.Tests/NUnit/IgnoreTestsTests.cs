using FeatureSelect.NUnit;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace FeatureSelect.Tests.NUnit;

public class IgnoreTestsTests
{
    private TestDummy test;
    private TestClassWithFeatures fixture;

    [SetUp]
    public void SetUp()
    {
        fixture = new TestClassWithFeatures();
        test = new TestDummy
        {
            RunState = RunState.Runnable,
            Method = new MethodWrapper(typeof(TestClassWithFeatures), nameof(TestClassWithFeatures.Test)),
            Fixture = fixture
        };
    }

    public string? TestSkipReason => test.Properties.Get(PropertyNames.SkipReason) as string;

    [Test]
    public void When_feature_source_not_available__marks_test_as_NotRunnable()
    {
        fixture.features = null;

        new IgnoreIfEnabledAttribute("MyFeature").ApplyToTest(test);
        Assert.Multiple(() =>
        {
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(TestSkipReason, Is.EqualTo("Unable to find FeatureSource"));
        });
    }

    [Test]
    public void IgnoreIfDisabled__When_feature_is_disabled__marks_test_as_Ignored()
    {
        fixture.features = new DisabledFeatureSource();

        new IgnoreIfDisabledAttribute("MyFeature").ApplyToTest(test);
        Assert.Multiple(() =>
        {
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(TestSkipReason, Is.EqualTo("Feature 'MyFeature' is disabled"));
        });
    }

    [Test]
    public void IgnoreIfDisabled__When_feature_is_enabled__does_not_modify_test()
    {
        fixture.features = new EnabledFeatureSource();

        new IgnoreIfDisabledAttribute("MyFeature").ApplyToTest(test);
        Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
    }

    [Test]
    public void IgnoreIfEnabled__When_feature_is_enabled__marks_test_as_Ignored()
    {
        fixture.features = new EnabledFeatureSource();

        new IgnoreIfEnabledAttribute("MyFeature").ApplyToTest(test);
        Assert.Multiple(() =>
        {
            Assert.That(test.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(TestSkipReason, Is.EqualTo("Feature 'MyFeature' is enabled"));
        });
    }

    [Test]
    public void IgnoreIfEnabled__When_feature_is_disabled__does_not_modify_test()
    {
        fixture.features = new DisabledFeatureSource();

        new IgnoreIfEnabledAttribute("MyFeature").ApplyToTest(test);
        Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
    }

    private class TestClassWithFeatures
    {
        public FeatureSource? features;

        public void Test()
        { }
    }

    private class TestDummy : Test
    {
        public TestDummy() : base("Dummy")
        {
        }

        public override object?[] Arguments { get; } = Array.Empty<object>();
        public override bool HasChildren { get; } = false;
        public override IList<ITest> Tests { get; } = Array.Empty<ITest>();

        public override string XmlElementName => throw new NotImplementedException();

        public override TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();

        public override TestResult MakeTestResult() => throw new NotImplementedException();
    }
}