using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx)
    {
        // Prevent MIME sniffing
        ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
        // Clickjacking
        ctx.Response.Headers["X-Frame-Options"] = "DENY";
        // Basic XSS protection (legacy)
        ctx.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        // Referrer policy
        ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
        // Minimal CSP - tune per app (disallow inline by default)
        ctx.Response.Headers["Content-Security-Policy"] = "default-src 'self'; object-src 'none'; frame-ancestors 'none';";
        // Feature-Policy / Permissions-Policy (example)
        ctx.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=()";
        await _next(ctx);
    }
}