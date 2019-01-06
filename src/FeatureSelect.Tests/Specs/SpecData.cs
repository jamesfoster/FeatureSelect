using System;
using System.Collections.Generic;
using SimpleInjector;

namespace FeatureSelect.Tests.Specs
{
	public static class SpecData
	{
		public static FeatureSource FeatureSource
		{
			get => Spec.GetOrDefault(Feature.EmptySource);
			set => Spec.Set(value);
		}

		public static FeatureSelector FeatureSelector
		{
			get => Spec.GetOrDefault<FeatureSelector>(new SimpleFeatureSelector(FeatureSource));
			set => Spec.Set(value);
		}

		public static FeatureContext FeatureContext
		{
			get => Spec.GetOrDefault<FeatureContext>("FeatureContext", _ => Maybe.Nothing<string>());
			set => Spec.Set(value, "FeatureContext");
		}

		public static Dictionary<string, FeatureExecutor> FrozenFeatures
		{
			get
			{
				if (!Spec.TryGetValue<Dictionary<string, FeatureExecutor>>(out var value))
				{
					value = new Dictionary<string, FeatureExecutor>();
					Spec.Set(value);
				}

				return value;
			}
		}

		public static object Result
		{
			get => Spec.Get<object>("Result");
			set => Spec.Set(value, "Result");
		}

		public static Exception ThrownException
		{
			get => Spec.Get<Exception>();
			set => Spec.Set(value);
		}

		public static Func<string> IfEnabled
		{
			get => () =>
			{
				Spec.Set(true, "EnabledCalled");
				return Spec.GetOrDefault<Func<string>>("IfEnabled", () => "a")();
			};
			set => Spec.Set(value, "IfEnabled");
		}

		public static Func<string> IfDisabled
		{
			get => () =>
			{
				Spec.Set(true, "DisabledCalled");
				return Spec.GetOrDefault<Func<string>>("IfDisabled", () => "b")();
			};
			set => Spec.Set(value, "IfDisabled");
		}

		public static Container Container
		{
			get => Spec.Get<Container>();
			set => Spec.Set(value);
		}
	}
}