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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmallTalks.Core.Services;
using Swashbuckle.AspNetCore.Swagger;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;

namespace SmallTalks.Api
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
            RegisterBlipTypes();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services = Core.ContainerProvider.RegisterTypes(services);
            services.AddSingleton<IDateTimeDectector, WatsonDatetimeDetector>();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SmallTalks", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "SmallTalks V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();

            string baseDir = env.WebRootPath;
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(baseDir, "App_Data"));

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
