namespace FeatureSelect
{
	public class Feature
	{
		public static FeatureSource EmptySource = new EmptyFeatureSource();

		private class EmptyFeatureSource : FeatureSource
		{
			public FeatureState GetFeatureState(
				string feature,
				FeatureContext context
			) => FeatureState.Unknown;
		}
	}
}