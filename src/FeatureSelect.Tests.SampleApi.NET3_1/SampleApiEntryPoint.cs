using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FeatureSelect.Tests.SampleApi.NEW3_1
{
    public class SampleApiEntryPoint
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
