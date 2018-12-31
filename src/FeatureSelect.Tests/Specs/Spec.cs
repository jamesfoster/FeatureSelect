using TechTalk.SpecFlow;

namespace FeatureSelect.Tests.Specs
{
	public static class Spec
	{
		public static T Get<T>(string key) =>
			ScenarioContext.Current.Get<T>(key);

		public static T Get<T>() =>
			ScenarioContext.Current.Get<T>();

		public static T GetOrDefault<T>(string key, T defaultValue) =>
			TryGetValue<T>(key, out var value) ? value : defaultValue;

		public static T GetOrDefault<T>(T defaultValue) =>
			TryGetValue<T>(out var value) ? value : defaultValue;

		public static bool TryGetValue<T>(string key, out T value) =>
			ScenarioContext.Current.TryGetValue(key, out value);

		public static bool TryGetValue<T>(out T value) =>
			ScenarioContext.Current.TryGetValue(out value);

		public static void Set<T>(T value, string key) =>
			ScenarioContext.Current.Set(value, key);

		public static void Set<T>(T value) =>
			ScenarioContext.Current.Set(value);
	}
}
