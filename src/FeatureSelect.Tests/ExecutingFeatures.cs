namespace FeatureSelect.Tests;

public class ExecutingFeatures
{
    [Test]
    public void When_a_feature_is_disabled_executes_disabled_callback()
    {
        var sut = new DisabledFeatureSource();

        var result = sut.GetFeature("my-feature").Execute(() => "enabled", () => "not enabled");

        Assert.That(result, Is.EqualTo("not enabled"));
    }

    [Test]
    public void When_a_feature_is_enabled_executes_enabled_callback()
    {
        var sut = new EnabledFeatureSource();

        var result = sut.GetFeature("my-feature").Execute(() => "enabled", () => "not enabled");

        Assert.That(result, Is.EqualTo("enabled"));
    }

    [Test]
    public void When_a_feature_is_disabled_executes_disabled_callback_no_result()
    {
        var sut = new DisabledFeatureSource();

        sut.GetFeature("my-feature")
            .Execute(
                ifEnabled: () => Assert.Fail("Feature should be disabled"),
                ifDisabled: () => { }
            );
    }

    [Test]
    public void When_a_feature_is_enabled_executes_enabled_callback_no_result()
    {
        var sut = new EnabledFeatureSource();

        sut.GetFeature("my-feature")
            .Execute(
                ifEnabled: () => { },
                ifDisabled: () => Assert.Fail("Feature should be disabled")
            );
    }
}