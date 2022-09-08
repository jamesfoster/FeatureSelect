using System.Diagnostics.CodeAnalysis;

namespace FeatureSelect;

public interface FeatureSource
{
    [return: NotNull]
    Feature GetFeature(string feature);
}