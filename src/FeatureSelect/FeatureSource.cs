namespace FeatureSelect
{
	public interface FeatureSource
	{
		FeatureState GetFeatureState(string feature, FeatureContext context);
	}
}