namespace FeatureSelect.Sql.Tests
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;

    using NUnit.Framework;

    [TestFixture]
    public class SqlFeatureSourceTests
    {
        private SqlFeatureSource source;

        [SetUp]
        public void SetUp()
        {
            source = new SqlFeatureSource("FeatureTestDB", "Features");
        }

        [TearDown]
        public void TearDown()
        {
            ResetData();
        }

        private void ResetData()
        {
            var connStr = ConfigurationManager.ConnectionStrings["FeatureTestDB"].ConnectionString;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Features";
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        [Test]
        public void Returns_null_if_the_feature_doesnt_exist()
        {
            var feature = source.GetFeature("DoesntExist");

            Assert.That(feature, Is.Null);
        }

        [Test]
        public void Can_create_a_feature()
        {
            source.SetFeature("NewFeature", "On", null);

            var feature = source.GetFeature("NewFeature");
            
            Assert.That(feature, Is.InstanceOf<OnFeature>());
        }

        [Test]
        public void Can_update_a_feature()
        {
            source.SetFeature("NewFeature", "On", null);
            source.SetFeature("NewFeature", "Off", null);

            var feature = source.GetFeature("NewFeature");

            Assert.That(feature, Is.InstanceOf<OffFeature>());
        }

        [Test]
        public void Can_update_a_feature_with_options()
        {
            source.SetFeature("NewFeature", "On", null);
            source.SetFeature("NewFeature", "Property", new Dictionary<string, string> {{"Prop", "Value1, Value2"}});

            var feature = (PropertyFeature)source.GetFeature("NewFeature");

            Assert.That(feature.Property, Is.EqualTo("Prop"));
            Assert.That(feature.Values, Is.EqualTo(new[] {"Value1", "Value2"}));
        }
    }
}
