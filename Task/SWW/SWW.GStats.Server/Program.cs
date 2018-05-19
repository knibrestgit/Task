using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace SWW.GStats.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.File(GetLoggerFileName(), rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            try 
            {
                BuildWebHost(args).Run();
            } 
            catch (Exception ex) 
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            } 
            finally 
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();

            return WebHost.CreateDefaultBuilder(args)
                          .UseConfiguration(configuration)
                          .UseStartup<Startup>()
                          .UseSerilog()
                          .Build();
        }

        private static string GetLoggerFileName() {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileName = Path.Combine(exePath, "logs", "log.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            return fileName;
        }
    }
}
