namespace FeatureSelect
{
    public interface IFeature
    {
        bool IsEnabled(object context);
    }
}