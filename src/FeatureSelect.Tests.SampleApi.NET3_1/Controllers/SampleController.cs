using FeatureSelect.AspNetCore;

using Microsoft.AspNetCore.Mvc;

namespace FeatureSelect.Tests.Web.Controllers;

[IfEnabled("Sample")]
public class SampleController
{
    [HttpGet("/feature1")]
    [IfEnabled("feature1")]
    public string Feature1() => "Feature One";

    [HttpGet("/feature2")]
    [IfEnabled("feature2")]
    public string FutureFeature2() => "Future Feature Two";

    [HttpGet("/feature2")]
    [IfDisabled("feature2")]
    public string CurrentFeature2() => "Current Feature Two";

    [HttpGet("/feature3")]
    public string Feature3([FromServices] FeatureSource features) => features
        .GetFeature("feature3")
        .Execute(
            ifEnabled: () => "Feature3 is enabled",
            ifDisabled: () => "Feature3 is disabled"
        );
}