using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetShop.Configurations;
using PetShop.Data;
using PetShop.Entity;
using serverapi.Services;
using serverapi.Services.Iservice;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// add PetShopDbContext
builder.Services.AddDbContext<PetShopDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// add service jwt
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(jwt => {
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
.AddGoogle(options => {
    options.ClientId = builder.Configuration["Authentication:Jwt:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Jwt:ClientSecret"]!;
});


// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});


builder.Services.AddIdentityCore<NguoiDung>().AddEntityFrameworkStores<PetShopDbContext>();


// add service
builder.Services.AddScoped<IGoogleService, GoogleService>();

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