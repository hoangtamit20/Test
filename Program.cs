using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetShop.Configurations;
using PetShop.Data;
using PetShop.Entity;
using serverapi.Entity;
using serverapi.Repository.OrderDetailRepository;
using serverapi.Repository.OrderRepository;
using serverapi.Repository.PaymentRepository;
using serverapi.Services;
using serverapi.Services.Iservice;
using serverapi.Validator;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// configure swagger authen for test api
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "Please enter your token with this format: ''Bearer YOUR_TOKEN''",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });

});
// add PetShopDbContext
builder.Services.AddDbContext<PetShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// add service jwt
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(jwt =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtConfig:SecretKey").Value!);
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, //for dev
        ValidateAudience = false, // for dev
        RequireExpirationTime = false, // for dev --- needs to be update when refresh token is
        ValidateLifetime = true,

    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Jwt:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Jwt:ClientSecret"]!;
});


// Configure authorization
builder.Services.AddAuthorization(
//     options =>
// {
//     options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
//     options.AddPolicy("User", policy => policy.RequireRole("User"));
// }
);


builder.Services.AddIdentityCore<NguoiDung>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PetShopDbContext>();


// add service
builder.Services.AddHttpClient();

builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();


// add validator service
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();