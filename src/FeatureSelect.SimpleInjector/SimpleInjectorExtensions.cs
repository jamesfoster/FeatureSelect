using SimpleInjector;

namespace FeatureSelect.SimpleInjector
{
	public static class SimpleInjectorExtensions
	{
		public static void AddFeatureInjectionBehavior(this ContainerOptions options)
		{
			var originalBehavior = options.DependencyInjectionBehavior;

			options.DependencyInjectionBehavior = new FeatureInjectionBehavior(
				options.Container,
				originalBehavior
			);
		}
	}
}