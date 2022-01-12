using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyPet.BLL.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyPet.Api
{
    public class Program
    {
        public static void Main(string[] args) // or async Task
        {
            //configure logging first
            ConfigureLogging();

            //then create the host, so that if the host fails we can log errors
            var host = CreateHostBuilder(args).Build();           

            host.Run();
        }

        private static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true)
              .Build();

            Log.Logger = new LoggerConfiguration()            
              .Enrich.FromLogContext()
              .Enrich.WithMachineName()
              .WriteTo.Debug()
              .WriteTo.Console()
              .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
              .Enrich.WithProperty("Environment", environment)
              .ReadFrom.Configuration(configuration)
              .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<Startup>();
           })
           .ConfigureAppConfiguration(configuration =>
           {
               configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
               configuration.AddJsonFile(
             $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
             optional: true);
           })
           .UseSerilog();
        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
