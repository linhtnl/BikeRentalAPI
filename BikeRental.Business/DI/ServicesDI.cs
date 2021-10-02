using BikeRental.Business.Services;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.DI
{
    public static class ServicesDI
    {
        public static void ConfigServicesDI(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<DbContext, ChoThueXeMayContext>();

            services.AddTransient<IAreaRepository, AreaRepository>();
            services.AddTransient<IAreaService, AreaService>();

            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ICustomerService, CustomerService>();

            services.AddTransient<IWalletRepository, WalletRepository>();
            services.AddTransient<IWalletService, WalletService>();

            services.AddTransient<ITransactionHistoryRepository, TransactionHistoryRepository>();
            services.AddTransient<ITransactionHistoryService, TransactionHistoryService>();

            services.AddTransient<IAdminRepository, AdminRepository>();
            services.AddTransient<IAdminService, AdminService>();

            services.AddTransient<IBikeRepository, BikeRepository>();
            services.AddTransient<IBikeService, BikeService>();

            services.AddTransient<IOwnerRepository, OwnerRepository>();
            services.AddTransient<IOwnerService, OwnerService>();

            services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            services.AddTransient<IFeedbackService, FeedbackService>();

            services.AddTransient<IBrandRepository, BrandRepository>();
            services.AddTransient<IBrandService, BrandService>();

            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<ICategoryService, CategoryService>();

            services.AddTransient<ICampaignRepository, CampaignRepository>();
            services.AddTransient<ICampaignService, CampaignService>();

            services.AddTransient<IVoucherRepository, VoucherRepository>();
            services.AddTransient<IVoucherService, VoucherService>();
        }
    }
}
