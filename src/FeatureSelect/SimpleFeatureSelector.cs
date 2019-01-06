namespace FeatureSelect
{
	public class SimpleFeatureSelector : FeatureSelector
	{
		private readonly FeatureSource source;

		public SimpleFeatureSelector(FeatureSource source)
		{
			this.source = source;
		}

		public FeatureExecutor Freeze(
			string feature,
			FeatureContext context)
		{
			var state = source.GetFeatureState(feature, context);

			return state == FeatureState.Enabled ? Enabled : Disabled;
		}

		private static FeatureExecutor Enabled => new EnabledExecutor();
		private static FeatureExecutor Disabled => new DisabledExecutor();
	}
}