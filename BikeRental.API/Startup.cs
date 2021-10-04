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
using BikeRental.API.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Utilities;
using Microsoft.AspNetCore.Mvc.Versioning;

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

            using var json = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("BikeRental.API.Firebase.firebase_config.json");
            var something = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromStream(json)
            });
            services.AddRouting();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://securetoken.google.com/chothuexemay-35838";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/chothuexemay-35838",
                        ValidateAudience = true,
                        ValidAudience = "chothuexemay-35838",
                        ValidateLifetime = true
                    };
                });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<Area, AreaViewModel>();
                mc.CreateMap<AreaViewModel, Area>();

                mc.CreateMap<Area, AreaCreateModel>();
                mc.CreateMap<AreaCreateModel, Area>();

                mc.CreateMap<Customer, CustomerViewModel>();
                mc.CreateMap<CustomerViewModel, Customer>();

                mc.CreateMap<Wallet, WalletViewModel>();
                mc.CreateMap<WalletViewModel, Wallet>();

                mc.CreateMap<TransactionHistory, TransactionHistoryViewModel>();
                mc.CreateMap<TransactionHistoryViewModel, TransactionHistory>();

                mc.CreateMap<Bike, BikeViewModel>();
                mc.CreateMap<BikeViewModel, Bike>();

                mc.CreateMap<Brand, BrandViewModel>();
                mc.CreateMap<BrandViewModel, Brand>();

                mc.CreateMap<Category, CategoryViewModel>();
                mc.CreateMap<CategoryViewModel, Category>();

                mc.CreateMap<Category, CategoryCreateModel>();
                mc.CreateMap<CategoryCreateModel, Category>();

                mc.CreateMap<Campaign, CampaignViewModel>();
                mc.CreateMap<CampaignViewModel, Campaign>();

                mc.CreateMap<Voucher, VoucherViewModel>();
                mc.CreateMap<VoucherViewModel, Voucher>();

                mc.CreateMap<VoucherItem, VoucherItemViewModel>();
                mc.CreateMap<VoucherItemViewModel, VoucherItem>();

                mc.CreateMap<Owner, OwnerViewModel>();
                mc.CreateMap<OwnerViewModel, Owner>();

                mc.CreateMap<Owner, OwnerRegisterRequest>();
                mc.CreateMap<OwnerRegisterRequest, Owner>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                int versionNum = int.Parse(_configuration.GetSection("Version").GetSection("VersionNumber").Value.ToString());
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });

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
