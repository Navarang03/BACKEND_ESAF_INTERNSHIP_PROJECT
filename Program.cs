//using Microsoft.EntityFrameworkCore;
//using EmployeeManagementAPI.Data;

//var builder = WebApplication.CreateBuilder(args);

//// Configure services
//builder.Services.AddControllers();

//// ? Add CORS policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularApp", policy =>
//    {
//        policy.WithOrigins("http://localhost:4200")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//// Add Swagger/OpenAPI
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// Configure EF Core to use SQL Server
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//var app = builder.Build();

//// Configure middleware
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//// ? Use CORS policy before Authorization
//app.UseCors("AllowAngularApp");

//app.UseAuthorization();

//// Map controller routes
//app.MapControllers();

//app.Run();

using Microsoft.EntityFrameworkCore;
using EmployeeManagementAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EmployeeManagementAPI.Helpers; // if JwtHelper is in Helpers folder

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Setup
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(jwtSecretKey) || jwtSecretKey.Length < 32)
    throw new Exception("JWT Secret Key is missing or too short.");

var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ?? Token helper
builder.Services.AddScoped<JwtHelper>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
