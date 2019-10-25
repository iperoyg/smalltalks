using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DateTimeDectector.Core;
using DateTimeDectector.Domain;
using Lime.Protocol.Serialization;
using Lime.Protocol;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmallTalks.Core.Services;
using Swashbuckle.AspNetCore.Swagger;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;
using Serilog;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using SmallTalks.Api.Filters;
using SmallTalks.Api.Middlewares;
using SmallTalks.Api.Extensions;
using SmallTalks.Api.Filters;
using SmallTalks.Api.Facades;
using SmallTalks.Api.Facades.Interfaces;

namespace SmallTalks.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterBlipTypes();
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services = Core.ContainerProvider.RegisterTypes(services);

            services
                .AddSingleton<IDateTimeDectector, WatsonDatetimeDetector>()
                .AddSingleton<Serilog.ILogger>(Log.Logger)
                .AddSingleton<IAnalysisFacade, AnalysisFacade>()
                .AddScoped<CustomAuthenticationFilter>();

            // Adds versioning, defaults to v1
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionReader = new MediaTypeApiVersionReader();
            });

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new Info { Title = "Smalltalks", Version = "v2" });
                c.SwaggerDoc("v1", new Info { Title = "Smalltalks", Version = "v1" });
                c.DocInclusionPredicate((version, apiDescription) =>
                {
                    // Swagger is dumb and uses {version} instead of number on Swagger Doc
                    var actionApiVersionModel = apiDescription.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == version);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == version);
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
                c.OperationFilter<ApiVersionOperationFilter>();
            });

            services
                .AddMvc()
                .AddJsonOptions(opt =>
                {
                    var serializer = opt.SerializerSettings;
                    if (serializer != null)
                    {
                        var settings = serializer as JsonSerializerSettings;
                        settings.NullValueHandling = NullValueHandling.Ignore;
                    }
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("./swagger/v2/swagger.json", "Smalltalks v2");
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "Smalltalks v1");
            });

            app.UseMiddleware<RequestLogger>();
            app.UseCors("AllowAll");
            app.UseMvc();
        }

        private static void RegisterBlipTypes()
        {
            TypeUtil.RegisterDocument<AnalysisResponse>();
            TypeUtil.RegisterDocument<Intention>();
            TypeUtil.RegisterDocument<Answer>();
            TypeUtil.RegisterDocument<Question>();
            TypeUtil.RegisterDocument<Entity>();
        }
    }
}