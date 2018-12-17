using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NPMGame.API.Extensions.Api;
using NPMGame.API.Extensions.Data;
using NPMGame.Core.Extensions;

namespace NPMGame.API
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
            services.AddAutoMapper();

            services.AddHttpClient();
            services.AddCors();

            services.AddMarten();
            services.AddCoreServices();
            services.AddApiServices();

            services.AddCookieAuthentication(Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services
                .AddSignalR()
                .AddJsonProtocol(builder =>
                {
                    builder.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                    builder.PayloadSerializerSettings.Converters.Add(new StringEnumConverter());

                    builder.PayloadSerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    builder.PayloadSerializerSettings.Formatting = Formatting.None;
                    builder.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    builder.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());

                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper autoMapper)
        {
            if (env.IsDevelopment())
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

            app.UseHubMappings();

            app.UseMvc();
        }
    }
}

