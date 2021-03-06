using AlbaVulpes.API.Extensions;
using AlbaVulpes.API.Models.Config;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AlbaVulpes.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAmazonServices();
            
            services.AddDatabaseServices();

            services.AddCookieAuthentication(Configuration);

            services.AddAutoMapper();
            services.AddValidator();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper autoMapper)
        {
            app.UseConsoleLogging();

            if (!env.IsDevelopment())
            {
                app.UseFileLogging();
                app.UseSeqLogging();
            }

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            autoMapper.ConfigurationProvider.AssertConfigurationIsValid();

            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowCredentials()
            );

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}

