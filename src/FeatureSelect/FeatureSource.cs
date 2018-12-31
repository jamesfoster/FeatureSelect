using System;

namespace FeatureSelect
{
	public interface FeatureSource
	{
		FeatureState GetFeatureState(string feature, Func<string, Maybe<string>> context);
	}
}