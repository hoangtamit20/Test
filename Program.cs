using System.Globalization;
using System.Reflection;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetShop.Configurations;
using PetShop.Data;
using serverapi.Configurations;
using serverapi.Entity;
using serverapi.Libraries.SignalRs;
using serverapi.Repository.OrderDetailRepository;
using serverapi.Repository.OrderRepository;
using serverapi.Repository.PaymentRepository;
using serverapi.Services;
using serverapi.Services.Iservice;
using serverapi.Validator;

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

    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Petshop service api",
        Description = "Sample .NET api by ",
        Contact = new OpenApiContact()
        {
            Name = "Hoang Trong Tam",
            Url = new Uri("https://www.youtube.com")
        }
    });
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var path = Path.Combine(AppContext.BaseDirectory, xmlFileName);
    options.IncludeXmlComments(path);
});
// add PetShopDbContext
builder.Services.AddDbContextFactory<PetShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

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
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});


// Configure authorization
builder.Services.AddAuthorization(
//     options =>
// {
//     options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
//     options.AddPolicy("User", policy => policy.RequireRole("User"));
// }
);


builder.Services.AddIdentityCore<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PetShopDbContext>()
                .AddDefaultTokenProviders(); // -> to generate token

// add service jwt
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// add service
builder.Services.AddHttpClient();

builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();


// add validator service
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();

// add MediatR
// builder.Services.AddMediatR(options => {
//     options.RegisterServicesFromAssembly(typeof(CreateMerchant).Assembly);
// });

builder.Services.AddHttpContextAccessor();

// add payment method service
//vnpay service
builder.Services.Configure<VnPayConfig>(
    builder.Configuration.GetSection(VnPayConfig.ConfigName));
// momo service
builder.Services.Configure<MomoConfig>(
    builder.Configuration.GetSection(MomoConfig.ConfigName));


// add configure the behavior of API responses when the model state is invalid
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// add sendmail service
builder.Services.AddTransient<IEmailSender, EmailSender>();

// add signalR
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseCors(options =>
{
    options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
});

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

// app.MapHub<NotificationHub>("/notificationHub");

app.Run();