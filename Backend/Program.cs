using Backend.Domain.Models;
using Backend.Domain.Services;
using Backend.Mapping;
using Backend.Persistence.Context;
using Backend.Persistence.Data;
using Backend.Services;
using Backend.Utility;
using Backend.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Services.AddControllers();

// Configure Entity Framework with MySQL
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString,
    ServerVersion.AutoDetect(connectionString));
});


// Configure Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// Register the IEmailSender interface
builder.Services.AddSingleton<IEmailSender<AppUser>, EmailSender>();

// Register JWT manager
builder.Services.AddScoped<IJwtTokenManager, JwtTokenManager>();

builder.Services.AddScoped<IAuthService, AuthService>();

// Get JWT Authentication
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new Exception("JwtSecretKey missing.");
var jwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer") ?? throw new Exception("Jwt:Issuer missing.");
var jwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience") ?? throw new Exception("Jwt:Audience missing.");

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Sets JWT as the default authentication scheme
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Defines JWT as the default challenge scheme
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;     // Ensures the token is stored when authentication succeeds

    options.TokenValidationParameters = new TokenValidationParameters     // Defines token validation rules
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),      // Uses the secret key for validating the token signature
        ValidateLifetime = true,    // Ensures the token hasn't expired
        ValidateAudience = true,    // Checks if the token's audience matches the expected audience
        ValidAudience = jwtAudience,   // Sets the valid audience from configuration
        ValidateIssuer = true,      // Ensures the token's issuer is valid
        ValidIssuer = jwtIssuer,        // Sets the valid issuer from configuration
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(AuthProfile));


// Add Authorization
builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

// Run the role seeder and default admin account at application startup
using (var scope = app.Services.CreateScope())
{
    // Get services from DI container
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    // Ensure roles exist
    await RoleAndAdminSeeder.SeedRolesAsync(roleManager);

    // Ensure there is at least one admin account
    await RoleAndAdminSeeder.SeedAdminAsync(userManager, roleManager);
}


app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
