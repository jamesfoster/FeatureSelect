namespace FeatureSelect
{
    public class InvalidFeature : IFeature
    {
        public string Name { get; set; }

        public InvalidFeature(string name)
        {
            Name = name;
        }

        public bool IsEnabled(object context) { return false; }
    }
}