using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using SWW.GStats.BusinessLogic.Services;
using SWW.GStats.DataAccess;

namespace SWW.GStats.Server
{
    public class Startup
    {

        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var sqliteFileName = ConfigureSqliteFile();

            services.AddMvc();

            services.AddMemoryCache()
                    .AddDbContext<StatsContext>(options => options.UseSqlite($"Filename={sqliteFileName}"))
                    .AddTransient<ServerService, ServerService>()
                    .AddTransient<ReportsService, ReportsService>();

        }

        private string GetExecutablePath() {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private string ConfigureSqliteFile() {
            var fileName = Path.Combine(GetExecutablePath(),"db","stats.sqlite");
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            return fileName;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
