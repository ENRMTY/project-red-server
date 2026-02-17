using Microsoft.EntityFrameworkCore;
using ProjectRed.Application.Services.Auth;
using ProjectRed.Application.Validators;
using ProjectRed.Core.Configuration;
using ProjectRed.Core.Interfaces.Repositories;
using ProjectRed.Core.Interfaces.Services.Auth;
using ProjectRed.Core.Interfaces.Services.Validators;
using ProjectRed.Infrastructure.Data;
using ProjectRed.Infrastructure.Repositories;

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

// repositories
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAuthRepository, UserAuthRepository>();

// utilities
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IPasswordValidator, PasswordValidator>();

// configs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

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
app.UseAuthorization();
app.MapControllers();
app.Run();
