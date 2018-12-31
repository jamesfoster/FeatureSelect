using System;

namespace FeatureSelect
{
	public class SimpleFeatureSelector : FeatureSelector
	{
		private readonly FeatureSource source;

		public SimpleFeatureSelector(FeatureSource source)
		{
			this.source = source;
		}

		public T Execute<T>(
			string feature,
			Func<string, Maybe<string>> context,
			Func<T> ifEnabled,
			Func<T> ifDisabled)
		{
			var state = source.GetFeatureState(feature, context);

			return state == FeatureState.Enabled ? ifEnabled() : ifDisabled();
		}
	}
}