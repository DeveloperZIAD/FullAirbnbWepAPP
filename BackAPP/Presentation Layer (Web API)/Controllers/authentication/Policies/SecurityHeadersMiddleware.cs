using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx)
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin"; // تعديل لضمان عمل الـ Referrer مع CORS

    // التعديل الجوهري هنا:
    // 1. connect-src: يسمح للفرونت إند بالاتصال بالـ API
    // 2. img-src: يسمح بعرض الصور من السيرفر ومن Cloudinary
    ctx.Response.Headers["Content-Security-Policy"] = 
        "default-src 'self'; " +
        "connect-src 'self' https://front-production-951a.up.railway.app; " + 
        "img-src 'self' data: https://res.cloudinary.com; " +
        "object-src 'none'; " +
        "frame-ancestors 'none';";

    ctx.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=()";
    await _next(ctx);
}
}