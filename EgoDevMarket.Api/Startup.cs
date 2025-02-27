// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using EgoDevMarket.Api.Brokers.DateTimes;
using EgoDevMarket.Api.Brokers.Loggings;
using EgoDevMarket.Api.Brokers.Storages;
using EgoDevMarket.Api.Services.Foundations.Orders;
using EgoDevMarket.Api.Services.Foundations.Products;
using EgoDevMarket.Api.Services.Foundations.Searchs;
using EgoDevMarket.Api.Services.Foundations.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EgoDevMarket.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddOData(options =>
                    options.Select().Filter().OrderBy().Count().Expand());

            services.AddDbContext<StorageBroker>();

            services.AddCors(option =>
            {
                option.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo { Title = "EgoDevMarket.Api", Version = "v1" });
            });

            AddBrokers(services);
            AddFoundationServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(config =>
                config.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "EgoDevMarket.Api v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("MyPolicy");
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static void AddBrokers(IServiceCollection services)
        {
            services.AddTransient<IStorageBroker, StorageBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
        }

        private static void AddFoundationServices(IServiceCollection services)
        {
			services.AddTransient<IOrderService, OrderService>();
			services.AddTransient<IProductService, ProductService>();
			services.AddTransient<ISearchService, SearchService>();
			services.AddTransient<IUserService, UserService>();

        }
    }
}
