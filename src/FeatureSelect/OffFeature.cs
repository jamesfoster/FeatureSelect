namespace FeatureSelect
{
    public class OffFeature : IFeature
    {
        public bool IsEnabled(object context) { return false; }

        public override string ToString() { return "Off"; }
    }
}