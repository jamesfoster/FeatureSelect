namespace FeatureSelect
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class AppSettingsFeatureSource : IFeatureSource
    {
        private string prefix = "feature.";

        private Dictionary<string, string> _featureSettings;
        public Dictionary<string, string> FeatureSettings
        {
            get
            {
                if (_featureSettings == null)
                {
                    _featureSettings = LoadSettings();
                }

                return _featureSettings;
            }
        }

        private Dictionary<string, string> LoadSettings()
        {
            var appSettings = ConfigurationManager.AppSettings;

            return appSettings
                .AllKeys
                .Where(key => key.StartsWith(prefix))
                .ToDictionary(key => key.Substring(prefix.Length), key => appSettings[key]);
        }

        public IFeature GetFeature(string featureName)
        {
            var state = GetFeatureState(featureName);

            if (state == null)
            {
                return null;
            }

            var options = GetFeatureOptions(featureName);

            return new FeatureFactory().Create(featureName, state.Value, options);
        }

        public IEnumerable<IFeature> ListFeatures()
        {
            return FeatureSettings
                .Keys
                .Select(GetFeature);
        }

        private FeatureState? GetFeatureState(string featureName)
        {
            if (!FeatureSettings.ContainsKey(featureName))
            {
                return null;
            }

            var setting = FeatureSettings[featureName];

            FeatureState state;
            if (Enum.TryParse(setting, out state))
            {
                return state;
            }

            return FeatureState.Invalid;
        }

        private Dictionary<string, string> GetFeatureOptions(string featureName)
        {
            return FeatureSettings
                .Where(setting => IsFeatureOption(featureName, setting))
                .ToDictionary(setting => GetOptionName(featureName, setting), setting => setting.Value);
        }

        private static bool IsFeatureOption(string featureName, KeyValuePair<string, string> setting)
        {
            return setting.Key.StartsWith(featureName) && setting.Key.Length != featureName.Length;
        }

        private static string GetOptionName(string featureName, KeyValuePair<string, string> setting)
        {
            return setting.Key.Substring(featureName.Length + 1);
        }
    }
}