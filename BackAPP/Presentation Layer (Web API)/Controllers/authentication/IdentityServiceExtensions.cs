// تأكد من مسار الـ Policies والـ Handlers الصحيح في مشروعك
using AirbnbClone.Api.Controllers.authentication;
using AirbnbClone.Api.Controllers.authentication.Policies;
using Business_Logic_Layer.Interfaces.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Presentation_Layer__Web_API_.Controllers.authentication.Policies;
using System.Text;

namespace AirbnbClone.Api.Controllers.Authentication.Extensions
{
    public static class IdentityServiceExtensions
    {

        //الملف الذي أرفقته أنت (Validation): وظيفته "التحقق". هو "رجل الأمن" الذي يقف على باب الـ API،
        //يقرأ التوكن القادم من الفرونت إند ويتأكد أنه سليم وغير منتهي الصلاحية. بدونه، لن يعرف السيرفر من هو المستخدم.
        //ملف TokenService (Generation): وظيفته "الإنشاء". هو "الموظف" الذي يصدر البطاقة (التوكن) للمستخدم عند تسجيل الدخول.
        //في TokenService: لكي توقع به التوكن (Signing).
        // في IdentityServiceExtensions: لكي تفك تشفير التوكن وتتأكد أن التوقيع صحيح.
        //
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // 1. إعدادات الـ JWT (قراءة المفتاح)
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                         ?? config["Jwt:Key"]
                         ?? "A_Very_Strong_Default_Key_For_Development_32_Chars";

            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // عطلها مؤقتاً
                    ValidateAudience = false, // عطلها مؤقتاً عشان يقبل "empty" أو غيرها
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // استخراج التوكين من Query String (الرابط)
                        var accessToken = context.Request.Query["access_token"];

                        // التأكد من المسار (يجب أن يطابق تماماً ما كتبته في MapHub)
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            });


            // 2. إعدادات الـ Authorization والـ Policies
            services.AddAuthorization(options =>
            { //only the custom policies here 
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("HostOnly", policy => policy.RequireRole("Host"));
                options.AddPolicy("GuestOnly", policy => policy.RequireRole("Guest"));


                // all the custom policies in the file AuthorizationPolicies.cs
                // إضافة أي Policies مخصصة من الملف الخارجي
                AuthorizationPolicies.AddCustomPolicies(options);
            });

            // 3. تسجيل الـ Handlers الخاصة بالصلاحيات المتقدمة
            services.AddSingleton<IAuthorizationHandler, OwnerRequirementHandler>();
            services.AddSingleton<IAuthorizationHandler, AccountOwnerHandler>();
            // this form token service is used to generate the token for the user after login or registration
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}