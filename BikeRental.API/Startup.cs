using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BikeRental.Business.Services;
using AutoMapper;
using BikeRental.Data.ViewModels;
using BikeRental.Business.DI;

namespace BikeRental.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ChoThueXeMayContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            services.AddHttpClient();
            services.AddScoped<ChoThueXeMayContext>();
            services.ConfigServicesDI();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<Area, AreaViewModel>();
                mc.CreateMap<AreaViewModel, Area>();

                mc.CreateMap<Customer, CustomerViewModel>();
                mc.CreateMap<CustomerViewModel, Customer>();

                mc.CreateMap<Bike, BikeViewModel>();
                mc.CreateMap<BikeViewModel, Bike>();

                mc.CreateMap<Wallet, WalletViewModel>();
                mc.CreateMap<WalletViewModel, Wallet>();

                mc.CreateMap<TransactionHistory, TransactionHistoryViewModel>();
                mc.CreateMap<TransactionHistoryViewModel, TransactionHistory>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BikeRentalAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeRentalAPI v1"));          

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
