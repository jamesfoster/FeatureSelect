using SimpleInjector;
using SimpleInjector.Advanced;

namespace FeatureSelect.SimpleInjector
{
	public class FeatureInjectionBehavior : IDependencyInjectionBehavior
	{
		private readonly Container container;
		private readonly IDependencyInjectionBehavior behavior;

		public FeatureInjectionBehavior(Container container, IDependencyInjectionBehavior behavior)
		{
			this.container = container;
			this.behavior = behavior;
		}

		public void Verify(InjectionConsumerInfo consumer)
		{
			behavior.Verify(consumer);
		}

		public InstanceProducer GetInstanceProducer(InjectionConsumerInfo consumer, bool throwOnFailure)
		{
			return FeatureProducer(consumer) ?? behavior.GetInstanceProducer(consumer, throwOnFailure);
		}

		private InstanceProducer FeatureProducer(InjectionConsumerInfo consumer)
		{
			if (!IsFeature(consumer, out var feature)) return null;

			FeatureExecutor InstanceCreator()
			{
				var selector = container.GetInstance<FeatureSelector>();
				var context = container.GetInstance<FeatureContext>();
				return selector.Freeze(feature, context);
			}

			var registration = Lifestyle.Transient.CreateRegistration(InstanceCreator, container);

			return new InstanceProducer(typeof(FeatureExecutor), registration);
		}

		private static bool IsFeature(InjectionConsumerInfo consumer, out string feature)
		{
			var target = consumer.Target;

			if (target.TargetType != typeof(FeatureExecutor))
			{
				feature = null;
				return false;
			}

			var attribute = target.GetCustomAttribute<FeatureAttribute>();

			feature = attribute?.Feature ?? target.Name;
			return true;
		}
	}
}