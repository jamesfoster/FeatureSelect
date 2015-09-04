namespace FeatureSelect
{
    public interface IFeature
    {
        string Name { get; }

        bool IsEnabled(object context);
    }
}