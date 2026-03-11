using AirbnbClone.Api.Controllers.Authentication.Extensions;
using Business_Logic_Layer.Extensions;
using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.Interfaces.GenData;
using Business_Logic_Layer.Service;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation_Layer__Web_API_.Controllers.authentication.Policies;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Core Services (DI Container)
// ==========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// --- Data & Business Logic Layer ---
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<IDatabaseSeederService, DatabaseSeederService>();
builder.Services.AddBusinessAndDataServices();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IMessagingService, MessagingService>();
// ��� ����� ���� ���� ��� ����� (Auth + Policies + Handlers)
builder.Services.AddIdentityServices(builder.Configuration);


// ==========================================
// 4. Swagger (Security Definitions)
// ==========================================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Airbnb Clone API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter: Bearer {your_token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// ==========================================
// 5. Build & Middleware Pipeline
// ==========================================
// CORS example
// 1. احصل على النطاق المسموح به من المتغيرات (مثلاً من Railway)
string allowedOrigin = builder.Configuration["AllowedOrigins"] ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictCors", policy =>
    {
        policy.WithOrigins(allowedOrigin.Split(',')) // يدعم إدخال أكثر من رابط مفصول بفاصلة
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // مطلوبة لأنك تستخدم withCredentials: true
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "Too many requests. Please slow down and try again after a minute."
        });
    };
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1); // ������� �������
        opt.PermitLimit = 100; // ��� ������� ������� ��� �� �������
        opt.QueueLimit = 0; // ��� ������ ������� �������
    });
});



// chathub
// 1. ����� ������
builder.Services.AddSignalR();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // <--- ��� ������ ���� ����� ����� ������!
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


var app = builder.Build();

// 1. تفعيل حماية HTTPS و HSTS (للإنتاج فقط)
if (app.Environment.IsProduction())
{
    app.UseHsts(); 
    app.UseForwardedHeaders(); // ضروري جداً لـ Railway
}

// 2. تفعيل الـ Middleware الأمنية المخصصة (Security Headers)
// تأكد من إضافة: app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

// 3. باقي الـ Middleware بالترتيب الصحيح
app.UseRateLimiter();
app.UseCors("StrictCors");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<Business_Logic_Layer.Hubs.ChatHub>("/chathub");

app.Run();