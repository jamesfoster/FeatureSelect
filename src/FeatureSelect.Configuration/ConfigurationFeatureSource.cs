using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace FeatureSelect.Configuration
{
	public class ConfigurationFeatureSource : FeatureSource
	{
		private readonly IConfiguration config;

		public ConfigurationFeatureSource(IConfiguration config)
		{
			this.config = config;
		}

		public FeatureState GetFeatureState(string feature, Func<string, Maybe<string>> context)
		{
			var section = config.GetSection(feature);

			if (!section.Exists()) return FeatureState.Unknown;

			switch (section.Value?.ToLower())
			{
				case "enabled": return FeatureState.Enabled;
				case "disabled": return FeatureState.Disabled;
				default: return ExamineContext(section, context);
			}
		}

		private static FeatureState ExamineContext(
			IConfiguration section,
			Func<string, Maybe<string>> context)
		{
			var children = section.GetChildren().ToList();

			return children.Count > 0 && children.All(x => Matches(x, context(x.Key)))
				? FeatureState.Enabled
				: FeatureState.Disabled;
		}

		private static bool Matches(IConfigurationSection section, Maybe<string> context)
		{
			return context.Match(
				just: value => Matches(section, value),
				nothing: () => string.IsNullOrEmpty(section.Value) && !section.GetChildren().Any()
			);
		}

		private static bool Matches(IConfigurationSection section, string context)
		{
			if (section.Value != null)
				return section.Value == context;

			if (IsArray(section))
				return section.GetChildren().Any(x => Matches(x, context));

			if (IsAll(section))
				return section.GetSection("$all").GetChildren().All(x => Matches(x, context));

			if (IsRegex(section, out var regex))
				return regex.IsMatch(context);

			if (IsGreaterThan(section, out var i))
				return int.TryParse(context, out var j) && j > i;

			if (IsGreaterThanOrEqual(section, out i))
				return int.TryParse(context, out var j) && j >= i;

			if (IsLessThan(section, out i))
				return int.TryParse(context, out var j) && j < i;

			if (IsLessThanOrEqual(section, out i))
				return int.TryParse(context, out var j) && j <= i;

			return false;
		}

		private static bool IsArray(IConfiguration section)
		{
			var children = section.GetChildren().ToList();

			return children.Any()
			       && !children.Where((x, i) => x.Key != i.ToString()).Any();
		}

		private static bool IsAll(IConfiguration section)
		{
			return section.GetSection("$all").Exists();
		}

		private static bool IsRegex(IConfiguration section, out Regex regex)
		{
			var pattern = section["$regex"];
			var isRegex = pattern != null;

			regex = isRegex ? new Regex(pattern) : null;

			return isRegex;
		}

		private static bool IsGreaterThan(IConfiguration section, out int amount)
		{
			return TryParseInt(section, "$gt", out amount);
		}

		private static bool IsGreaterThanOrEqual(IConfiguration section, out int amount)
		{
			return TryParseInt(section, "$gte", out amount);
		}

		private static bool IsLessThan(IConfiguration section, out int amount)
		{
			return TryParseInt(section, "$lt", out amount);
		}

		private static bool IsLessThanOrEqual(IConfiguration section, out int amount)
		{
			return TryParseInt(section, "$lte", out amount);
		}

		private static bool TryParseInt(IConfiguration section, string key, out int amount)
		{
			var value = section[key];
			var exists = value != null;

			amount = exists ? int.Parse(value) : 0;

			return exists;
		}
	}
}