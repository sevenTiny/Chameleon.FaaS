using Chameleon.Faas.Management.Entities;
using Chameleon.Faas.Management.Repository;
using Chameleon.Faas.Management.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Chameleon.Faas.Management.Api
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
            //add infrastructure

            //add DbContext
            services.AddDbContext<FaasDbContext>();
            //add repository
            BatchInject(typeof(IFaasScriptRepository).Assembly, (type, impType) => services.AddScoped(type, impType));
            //add service
            BatchInject(typeof(IFaasScriptService).Assembly, (type, impType) => services.AddScoped(type, impType));
            //add application


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chameleon.Faas.Management.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chameleon.Faas.Management.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void BatchInject(Assembly assembly, Action<Type, Type> injectAction)
        {
            var types = assembly.GetTypes();
            var interfaces = types.Where(t => t.IsInterface);
            var impTypes = types.Except(interfaces).ToList();
            foreach (var item in interfaces)
            {
                var impType = impTypes.FirstOrDefault(t => item.IsAssignableFrom(t));
                if (impType != null)
                {
                    injectAction(item, impType);
                }
            }
        }
    }
}
