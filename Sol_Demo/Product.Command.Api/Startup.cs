using Framework.HangFire.MediatR.Extension;
using Framework.SqlClient.Extensions;
using Hangfire;
using MediatR;
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
using System.Threading.Tasks;

namespace Product.Command.Api
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
            services.AddControllers();

            services.AddMediatR(typeof(Startup));

            services.AddSqlProvider(Configuration.GetConnectionString("DefaultConnection"));

            services.AddHangfire((config) =>
            {
                config.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"));
                config.UseMediatR();
            });
            services.AddHangfireServer();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            services.AddStackExchangeRedisCache((setup) =>
            {
                setup.InstanceName = "TestRedisCloud";

                //setup.Configuration = "redis-10512.c100.us-east-1-4.ec2.cloud.redislabs.com:10512";
                setup.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                {
                    EndPoints = { "redis-10965.c261.us-east-1-4.ec2.cloud.redislabs.com:10965" },
                    Password = "fL1gmv35qwVEquINThDMkNMmOh3QQVZU"
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product.Command.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product.Command.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}