using System.Net;
using TicketWebApp.Contexts;
using TicketWebApp.Models;

namespace TicketWebApp.Middleware
{
    /// <summary>
    /// Global exception middleware that logs unhandled errors to Errorlogs table
    /// and returns a clean 500 JSON response.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ComplaintContext db)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 1) Log to console/app logs
                _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);

                // 2) Best-effort DB logging (never throw from here)
                try
                {
                    db.Errorlogs.Add(new ErrorLog
                    {
                        ErrorMessage = ex.Message,
                        ErrorNumber = ex.HResult,
                        CreatedAt = DateTime.UtcNow
                    });
                    await db.SaveChangesAsync();
                }
                catch { /* swallow secondary logging errors */ }

                // 3) Clean 500 response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
            }
        }
    }
}