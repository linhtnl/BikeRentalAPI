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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Utilities;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using BikeRental.API.Handler;
using BikeRental.Business.Extensions;
using BikeRental.API.Config;

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
            //AuthConfig.ConfigAuthentication(services, _configuration);

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

            AuthConfig.ConfigAuthentication(services, _configuration);

            //services
            //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.Authority = "https://securetoken.google.com/chothuexemay-35838";
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidIssuer = "https://securetoken.google.com/chothuexemay-35838",
            //            ValidateAudience = true,
            //            ValidAudience = "chothuexemay-35838",
            //            ValidateLifetime = true
            //        };
            //    });

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

                mc.CreateMap<Wallet, WalletCreateRequest>();
                mc.CreateMap<WalletCreateRequest, Wallet>();

                mc.CreateMap<TransactionHistory, TransactionHistoryViewModel>();
                mc.CreateMap<TransactionHistoryViewModel, TransactionHistory>();

                mc.CreateMap<Bike, BikeViewModel>();
                mc.CreateMap<BikeViewModel, Bike>();

                mc.CreateMap<Bike, BikeCreateRequest>();
                mc.CreateMap<BikeCreateRequest, Bike>();

                mc.CreateMap<Bike, BikeUpdateRequest>();
                mc.CreateMap<BikeUpdateRequest, Bike>();

                mc.CreateMap<Bike, BikeDeleteSuccessViewModel>();
                mc.CreateMap<BikeDeleteSuccessViewModel, Bike>();

                mc.CreateMap<Bike, BikeFindingViewModel>();
                mc.CreateMap<BikeFindingViewModel, Bike>();

                mc.CreateMap<Brand, BrandViewModel>();
                mc.CreateMap<BrandViewModel, Brand>();

                mc.CreateMap<Category, CategoryViewModel>();
                mc.CreateMap<CategoryViewModel, Category>();

                mc.CreateMap<Category, CategoryCreateModel>();
                mc.CreateMap<CategoryCreateModel, Category>();

                mc.CreateMap<Category, CategoryCustomViewModel>();
                mc.CreateMap<CategoryCustomViewModel, Category>();

                mc.CreateMap<Campaign, CampaignViewModel>();
                mc.CreateMap<CampaignViewModel, Campaign>();

                mc.CreateMap<CampaignCreateRequest, Campaign>();
                mc.CreateMap<Campaign, CampaignCreateRequest>();

                mc.CreateMap<Campaign, CampaignUpdateRequest>();
                mc.CreateMap<CampaignUpdateRequest, Campaign>();

                mc.CreateMap<Voucher, VoucherViewModel>();
                mc.CreateMap<VoucherViewModel, Voucher>();

                mc.CreateMap<VoucherItem, VoucherItemViewModel>();
                mc.CreateMap<VoucherItemViewModel, VoucherItem>();

                mc.CreateMap<Owner, OwnerViewModel>();
                mc.CreateMap<OwnerViewModel, Owner>();

                mc.CreateMap<Owner, OwnerRatingViewModel>();
                mc.CreateMap<OwnerRatingViewModel, Owner>();

                mc.CreateMap<Owner, OwnerDetailViewModel>();
                mc.CreateMap<OwnerDetailViewModel, Owner>();

                mc.CreateMap<Owner, OwnerCreateRequest>();
                mc.CreateMap<OwnerCreateRequest, Owner>();

                mc.CreateMap<Owner, OwnerByAreaViewModel>();
                mc.CreateMap<OwnerByAreaViewModel, Owner>();

                mc.CreateMap<Feedback, FeedbackCreateRequest>();
                mc.CreateMap<FeedbackCreateRequest, Feedback>();

                mc.CreateMap<PriceList, PriceListViewModel>();
                mc.CreateMap<PriceListViewModel, PriceList>();

                mc.CreateMap<PriceList, PricelistCreateRequest>();
                mc.CreateMap<PricelistCreateRequest, PriceList>();

                mc.CreateMap<PriceList, PriceListByAreaViewModel>();
                mc.CreateMap<PriceListByAreaViewModel, PriceList>();

                mc.CreateMap<VoucherItemCreateRequest, VoucherItem>();
                mc.CreateMap<VoucherItem, VoucherItemCreateRequest>();

                mc.CreateMap<WalletCreateRequest, Wallet>();
                mc.CreateMap<Wallet, WalletCreateRequest>();

                mc.CreateMap<VoucherCreateRequest, Voucher>();
                mc.CreateMap<Voucher, VoucherCreateRequest>();

                mc.CreateMap<VoucherUpdateRequest, Voucher>();
                mc.CreateMap<Voucher, VoucherUpdateRequest>();

                mc.CreateMap<VoucherExchangeHistory, VoucherExchangeHistoryViewModel>();
                mc.CreateMap<VoucherExchangeHistoryViewModel, VoucherExchangeHistory>();

                mc.CreateMap<VoucherExchangeHistoryUpdateRequest, VoucherExchangeHistory>();
                mc.CreateMap<VoucherExchangeHistory, VoucherExchangeHistoryUpdateRequest>();

                mc.CreateMap<Payment, PaymentViewModel>();
                mc.CreateMap<PaymentViewModel, Payment>();

                mc.CreateMap<Payment, PaymentCreateRequest>();
                mc.CreateMap<PaymentCreateRequest, Payment>();

                mc.CreateMap<Payment, PaymentUpdateRequest>();
                mc.CreateMap<PaymentUpdateRequest, Payment>();

                mc.CreateMap<Booking, BookingViewModel>();
                mc.CreateMap<BookingViewModel, Booking>();

                mc.CreateMap<Customer, CustomerCreateRequest>();
                mc.CreateMap<CustomerCreateRequest, Customer>();

                mc.CreateMap<Admin, AdminViewModel>();
                mc.CreateMap<AdminViewModel, Admin>();

                mc.CreateMap<Booking, BookingCreateRequest>();
                mc.CreateMap<BookingCreateRequest, Booking>();

                mc.CreateMap<Booking, BookingSuccessViewModel>();
                mc.CreateMap<BookingSuccessViewModel, Booking>();

                mc.CreateMap<VoucherItemCreateRequest, VoucherItemCreateSuccessViewModel>();
                mc.CreateMap<VoucherItemCreateSuccessViewModel, VoucherItemCreateRequest>();

                mc.CreateMap<MotorType, MotorTypeViewModel>();
                mc.CreateMap<MotorTypeViewModel, MotorType>();
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
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "BikeRentalAPI", Version = "v2" });
            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddCors();
            services.ConfigureFilter<ErrorHandlingFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseSwaggerUI(
            options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            
        }
    }
}
