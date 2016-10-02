using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StatlerWaldorfCorp.TeamService.Models;
using StatlerWaldorfCorp.TeamService.LocationClient;
using StatlerWaldorfCorp.TeamService.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Steeltoe.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector.PostgreSql.EFCore;

namespace StatlerWaldorfCorp.TeamService {
    public class Startup
    {
        public static string[] Args {get; set;} = new string[] {};        
        private ILogger logger;
        private ILoggerFactory loggerFactory;
        
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(Startup.Args)
                .AddCloudFoundry();

            Configuration = builder.Build();

            this.loggerFactory = loggerFactory;
            this.loggerFactory.AddConsole(LogLevel.Information);
            this.loggerFactory.AddDebug();

            this.logger = this.loggerFactory.CreateLogger("Startup");            
        }
                
        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddMvc();
            services.AddScoped<ITeamRepository, PostgresTeamRepository>();
            services.AddScoped<ILocationClient, HttpLocationClient>();

            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration));            
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}