using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

using static System.Reflection.BindingFlags;

namespace FeatureSelect.NUnit;

public abstract class IgnoreIfFeatureAttribute : NUnitAttribute, IApplyToTest
{
    private readonly string name;
    private readonly bool ignoreIfDisabled;

    public IgnoreIfFeatureAttribute(string name, bool ignoreIfDisabled = true)
    {
        this.name = name;
        this.ignoreIfDisabled = ignoreIfDisabled;
    }

    public void ApplyToTest(Test test)
    {
        if (test.RunState == RunState.NotRunnable) return;

        var features = GetFeatureSource(test);

        if (features == null)
        {
            test.RunState = RunState.NotRunnable;
            test.Properties.Set(PropertyNames.SkipReason, "Unable to find FeatureSource");
            return;
        }

        var state = ignoreIfDisabled ? "disabled" : "enabled";

        int allow() => 0;
        int ignore()
        {
            test.RunState = RunState.Ignored;
            test.Properties.Set(PropertyNames.SkipReason, $"Feature '{name}' is {state}");
            return 0;
        }

        var feature = features.GetFeature(name);

        _ = ignoreIfDisabled
            ? feature.Execute(ifEnabled: allow, ifDisabled: ignore)
            : feature.Execute(ifEnabled: ignore, ifDisabled: allow);
    }

    private FeatureSource? GetFeatureSource(Test test)
    {
        var fixture = test.Fixture;
        if (fixture == null) return null;

        var type = fixture.GetType();
        var fields = type.GetFields(Instance | DeclaredOnly | Public | NonPublic);
        var sources = fields
            .Where(x => typeof(FeatureSource).IsAssignableFrom(x.FieldType))
            .ToArray();
        if (sources.Length != 1) return null;

        return (FeatureSource?)sources[0].GetValue(fixture);
    }
}