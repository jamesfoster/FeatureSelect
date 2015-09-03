namespace FeatureSelect.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;

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
            FeatureData data;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                data = GetFeatureData(featureName, conn);

                conn.Close();
            }

            if (data == null)
            {
                return null;
            }

            var options = GetFeatureOptions(data);

            return new FeatureFactory().Create(data.State, options);
        }

        private FeatureData GetFeatureData(string featureName, SqlConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("SELECT State, PropertyName, PropertyValues FROM {0} WHERE Name = '{1}'", tableName, featureName);

            SqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception)
            {
                return null;
            }

            if (!reader.Read())
            {
                return null;
            }

            return new FeatureData
                {
                    State = GetState(reader.GetString(0)),
                    PropertyName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    PropertyValues = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
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
            public FeatureState State { get; set; }

            public string PropertyName { get; set; }

            public string PropertyValues { get; set; }
        }
    }
}
