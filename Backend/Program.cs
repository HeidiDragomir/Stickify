using Backend.Domain.Models;
using Backend.Persistence.Context;
using Backend.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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


// Configure JWT Authentication
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");
var jwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
var jwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Sets JWT as the default authentication scheme
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Defines JWT as the default challenge scheme
})
.AddJwtBearer(options =>
{
    var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

    options.SaveToken = true;     // Ensures the token is stored when authentication succeeds

    options.TokenValidationParameters = new TokenValidationParameters     // Defines token validation rules
    {
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),      // Uses the secret key for validating the token signature
        ValidateLifetime = true,    // Ensures the token hasn't expired
        ValidateAudience = true,    // Checks if the token's audience matches the expected audience
        ValidAudience = jwtAudience,   // Sets the valid audience from configuration
        ValidateIssuer = true,      // Ensures the token's issuer is valid
        ValidIssuer = jwtIssuer,        // Sets the valid issuer from configuration
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});


// Add Authorization
builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000") // Add frontend URLs later
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
