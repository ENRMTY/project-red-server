using Microsoft.EntityFrameworkCore;
using ProjectRed.Application.Services.Auth;
using ProjectRed.Application.Validators;
using ProjectRed.Core.Configuration;
using ProjectRed.Core.Interfaces.Repositories;
using ProjectRed.Core.Interfaces.Services.Auth;
using ProjectRed.Core.Interfaces.Services.Email;
using ProjectRed.Core.Interfaces.Services.Validators;
using ProjectRed.Infrastructure.Data;
using ProjectRed.Infrastructure.Repositories;
using ProjectRed.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// set up database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(envConnection))
{
    connectionString = envConnection;
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// register app services
// services
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// repositories
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAuthRepository, UserAuthRepository>();

// utilities
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IPasswordValidator, PasswordValidator>();

// configs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton(sp => sp
    .GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtSettings>>().Value);

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

var key = Encoding.UTF8.GetBytes(jwtSettings!.Key);

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            NameClaimType = "username",
            RoleClaimType = "role"
        };
    });

// register other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
Console.WriteLine("ENV: " + builder.Environment.EnvironmentName);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
