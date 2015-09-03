namespace FeatureSelect
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(ToStringJsonConverter))]
    public interface IFeature
    {
        bool IsEnabled(object context);
    }
}