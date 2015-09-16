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
            var data = GetFeatureData(featureName);

            if (data == null || data.Count == 0)
            {
                return null;
            }

            var feature = data.First();

            var options = GetFeatureOptions(feature);

            return new FeatureFactory().Create(featureName, feature.State, options);
        }

        public bool SetFeature(string featureName, string state, IDictionary<string, string> options)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SetFeatureData(conn.CreateCommand(), featureName, state, options);

                conn.Close();
            }

            return true;
        }

        private void SetFeatureData(SqlCommand cmd, string featureName, string state, IDictionary<string, string> options)
        {
            cmd.CommandText = string.Format("UPDATE {0} SET State = @State, PropertyName = @PropertyName, PropertyValues = @PropertyValues WHERE Name = @Name", tableName);

            var propertyName = options == null ? "" : options.First().Key;
            var propertyValues = options == null ? "" : options.First().Value;

            cmd.Parameters.Add(new SqlParameter("@State", state));
            cmd.Parameters.Add(new SqlParameter("@PropertyName", propertyName));
            cmd.Parameters.Add(new SqlParameter("@PropertyValues", propertyValues));
            cmd.Parameters.Add(new SqlParameter("@Name", featureName));

            var rows = cmd.ExecuteNonQuery();

            if (rows > 0)
            {
                return;
            }

            cmd.CommandText = string.Format("INSERT INTO {0} (Name, State, PropertyName, PropertyValues) VALUES (@Name, @State, @PropertyName, @PropertyValues)", tableName);

            cmd.ExecuteNonQuery();
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

        private List<FeatureData> GetFeatureData(string featureName = null)
        {
            List<FeatureData> data;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                data = GetFeatureData(conn.CreateCommand(), featureName);

                conn.Close();
            }

            return data;
        }

        private List<FeatureData> GetFeatureData(SqlCommand cmd, string featureName)
        {
            var commandText = string.Format("SELECT Name, State, PropertyName, PropertyValues FROM {0}", tableName);

            if (!string.IsNullOrEmpty(featureName))
            {
                commandText += " WHERE Name = @Name";
                cmd.Parameters.Add(new SqlParameter("@Name", featureName));
            }

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
                            State = reader.GetString(1).Trim(),
                            PropertyName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            PropertyValues = reader.IsDBNull(3) ? null : reader.GetString(3)
                        });
            }

            return results;
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

            public string State { get; set; }

            public string PropertyName { get; set; }

            public string PropertyValues { get; set; }
        }
    }
}
