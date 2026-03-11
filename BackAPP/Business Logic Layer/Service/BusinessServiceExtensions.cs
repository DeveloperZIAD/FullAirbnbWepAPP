using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Business_Logic_Layer.Extensions
{
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessAndDataServices(this IServiceCollection services)
        {
            // هنا طبقة الـ Business بتسجل الـ UnitOfWork لأنها شيفاها
            services.AddScoped<UnitOfWork>();

            // تسجيل السيرفسز الخاصة بالبزنس
            services.AddScoped<ListingService>();
            services.AddScoped<BookingService>();
            services.AddScoped<AccountService>();
            services.AddScoped<UserService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<HostProfileService>();
            
      
            services.AddScoped<ListingImageService>();
            
            services.AddScoped<CategoryService>();
            services.AddScoped<AmenityService>();
            services.AddScoped<MessagingService>();


            services.AddScoped<ReviewService>();
            services.AddScoped<WishlistService>();
            services.AddScoped<AdminActivityLogService>();
            return services;
        }
    }
}