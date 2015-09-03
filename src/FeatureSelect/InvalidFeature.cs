namespace FeatureSelect
{
    public class InvalidFeature : IFeature
    {
        public bool IsEnabled(object context) { return false; }

        public override string ToString() { return "Invalid"; }
    }
}