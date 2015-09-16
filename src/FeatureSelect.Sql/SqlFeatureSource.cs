namespace FeatureSelect.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;

    public class SqlFeatureSource : IFeatureSource
    {
        private readonly string tableName;

        private readonly string connectionString;

        public SqlFeatureSource(string connectionStringName, string tableName)
        {
            this.tableName = tableName;
            connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public IFeature GetFeature(string featureName)
        {
            var data = GetFeatureData();

            if (data == null || data.Count == 0)
            {
                return null;
            }

            var feature = data.First();

            var options = GetFeatureOptions(feature);

            return new FeatureFactory().Create(featureName, feature.State, options);
        }

        public IEnumerable<IFeature> ListFeatures()
        {
            var data = GetFeatureData();

            if (data == null)
            {
                return null;
            }

            var factory = new FeatureFactory();

            return
                from feature in data
                let options = GetFeatureOptions(feature)
                select factory.Create(feature.Name, feature.State, options);
        }

        private List<FeatureData> GetFeatureData()
        {
            List<FeatureData> data;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                data = GetFeatureData(conn);

                conn.Close();
            }
            return data;
        }

        private List<FeatureData> GetFeatureData(SqlConnection connection, string featureName = null)
        {
            var commandText = string.Format("SELECT Name, State, PropertyName, PropertyValues FROM {0}", tableName);

            if (!string.IsNullOrEmpty(featureName))
            {
                commandText += string.Format(" WHERE Name = '{0}'", featureName);
            }

            var cmd = connection.CreateCommand();

            cmd.CommandText = commandText;

            SqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception)
            {
                return null;
            }

            var results = new List<FeatureData>();

            while (reader.Read())
            {
                results.Add(
                    new FeatureData
                        {
                            Name = reader.GetString(0),
                            State = GetState(reader.GetString(1)),
                            PropertyName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            PropertyValues = reader.IsDBNull(3) ? null : reader.GetString(3)
                        });
            }

            return results;
        }

        private static FeatureState GetState(string value)
        {
            FeatureState state;

            if (!Enum.TryParse(value, out state))
            {
                return FeatureState.Invalid;
            }

            return state;
        }

        private Dictionary<string, string> GetFeatureOptions(FeatureData data)
        {
            var options = new Dictionary<string, string>();

            if (data.PropertyName != null)
            {
                options.Add(data.PropertyName, data.PropertyValues);
            }

            return options;
        }

        private class FeatureData
        {
            public string Name { get; set; }

            public FeatureState State { get; set; }

            public string PropertyName { get; set; }

            public string PropertyValues { get; set; }
        }
    }
}
