﻿using Api.GHN.Implementation;
using Api.GHN.Interface;
using Api.Implementation;
using Api.Interface;
using Api.Payos.Implementation;
using Api.Payos.Interface;
using Api.vnPay.Implementation;
using Api.vnPay.Interface;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Repository.Implementation;
using Repository.Interface;
using Repository.Interface.Api.Interface;
using Service;
using Service.Implementation;
using Service.Interface;

var builder = WebApplication.CreateBuilder(args);

//emailSender
builder.Services.AddTransient<IEmailSender, EmailSender>();
//Category
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); 
builder.Services.AddScoped<ICategoryService, CategoryService>(); 
//Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
//Cart
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
//Order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
//User
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
//Cloundinary
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
//PayOS
builder.Services.AddScoped<IPayosService, PayosService>();
//vnPay
builder.Services.AddScoped<IVnPayService, VnPayService>();

builder.Services.AddScoped<IGhnService,GhnApiService>();


//Connect DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<Swd392OssContext>(options => options.UseSqlServer(connectionString));

//Config Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<Swd392OssContext>();

var clientId = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET");


builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]
             ?? throw new Exception("Invalid google client Id");
         googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
             ?? throw new Exception("Invalid google client secret");
        googleOptions.CallbackPath = "/signin-google";
        googleOptions.SaveTokens = true;
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "UserAuthCookie";
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


builder.Services.AddRazorPages();

var app = builder.Build();

app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();