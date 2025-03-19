using Api.GHN.Implementation;
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
builder.Services.AddTransient<IEmailService, EmailService>();

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
builder.Services.AddScoped<ICloudinaryProxy, CloudinaryProxy>();
//PayOS
builder.Services.AddScoped<IPayosProxy, PayosProxy>();
//vnPay
builder.Services.AddScoped<IVnPayProxy, VnPayProxy>();
//GHN
builder.Services.AddScoped<IGhnProxy,GhnApiProxy>();

builder.Services.AddScoped<UserManager<AspNetUser>>();
builder.Services.AddScoped<SignInManager<AspNetUser>>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.AllowedForNewUsers = false;
});

builder.Services.AddHttpContextAccessor();
//Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout của Session
    options.Cookie.HttpOnly = true; // Bảo mật cookie
    options.Cookie.IsEssential = true; // Đánh dấu cookie là cần thiết (GDPR compliance)
});
//Connect DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<Swd392OssContext>(options => options.UseSqlServer(connectionString));

//Config Identity
builder.Services.AddIdentity<AspNetUser, AspNetRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<Swd392OssContext>()
    .AddDefaultTokenProviders();

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

// Tự add thêm role vào db nếu ae chưa có
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AspNetRole>>();

    await SeedData.SeedRolesAsync(roleManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseSession();
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