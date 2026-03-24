using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicketWebApp.Contexts;
using TicketWebApp.Interfaces;
using TicketWebApp.Repositories;
using TicketWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Controllers + (basic) Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// 1) Controllers + (basic) Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// 2) DbContext (SQL Server)
builder.Services.AddDbContext<ComplaintContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Development"));
});

// 3) CORS (open policy, like FirstAPI)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4) Repositories (generic)
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

// 5) Services
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IAutoAssignmentService, AutoAssignmentService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Auth-related services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 6) Authentication (JWT) – only JwtBearer is required
string jwtKey = builder.Configuration["Keys:Jwt"]
    ?? throw new InvalidOperationException("Secret key not found in configuration. Add Keys:Jwt in appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // set true in prod behind HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// 7) Static files (for /wwwroot/uploads)
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors();
app.UseMiddleware<TicketWebApp.Middleware.ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();