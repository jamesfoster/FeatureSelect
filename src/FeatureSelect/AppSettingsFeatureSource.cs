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

            if (string.IsNullOrEmpty(state))
            {
                return null;
            }

            var options = GetFeatureOptions(featureName);

            return new FeatureFactory().Create(featureName, state, options);
        }

        public bool SetFeature(string featureName, string state, IDictionary<string, string> options = null)
        {
            FeatureSettings[featureName] = state;

            var existingOptions = FeatureSettings
                .Where(x => IsFeatureOption(featureName, x))
                .Select(x => x.Key)
                .ToList();

            foreach (var option in existingOptions)
            {
                FeatureSettings.Remove(option);
            }

            if (options != null)
            {
                foreach (var option in options)
                {
                    var optionName = string.Format("{0}.{1}", featureName, option.Key);

                    FeatureSettings[optionName] = option.Value;
                }
            }

            return true;
        }

        public IEnumerable<IFeature> ListFeatures()
        {
            return FeatureSettings
                .Keys
                .Select(GetFeature);
        }

        private string GetFeatureState(string featureName)
        {
            if (!FeatureSettings.ContainsKey(featureName))
            {
                return null;
            }

            return FeatureSettings[featureName];
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