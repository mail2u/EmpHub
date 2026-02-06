using EmpHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Duende.IdentityModel;
using EmpHub;
using EmpHub.Extension;

var builder = WebApplication.CreateBuilder(args);

UserExtensions.Initialize(builder.Configuration);

builder.Services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddAreaPageRoute("Identity", "/Login", "");

    options.Conventions.AllowAnonymousToPage("/Account/Login");

    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AuthorizeFolder("/Account");
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AuthorizeFolder("/Calendar");
    options.Conventions.AuthorizeFolder("/Dashboard");
    options.Conventions.AuthorizeFolder("/Document");
    options.Conventions.AuthorizeFolder("/Employee");
    options.Conventions.AuthorizeFolder("/Report");
    options.Conventions.AuthorizeFolder("/Service");
    options.Conventions.AuthorizeFolder("/Structure");

    options.Conventions.AuthorizeFolder("/Admin", "Admin");
    options.Conventions.AuthorizeFolder("/Dashboard", "Admin");
    options.Conventions.AuthorizePage("/Employee/Index", "Admin");
    options.Conventions.AuthorizePage("/Service/Approve", "Admin");

    options.Conventions.AuthorizeFolder("/Calendar", "DEV");
    options.Conventions.AuthorizeFolder("/Structure", "DEV");

    options.Conventions.AddPageRoute("/Employee/Detail", "/Employee/{id?}");
    options.Conventions.AddPageRoute("/Service/Detail", "/Service/{id?}");
    options.Conventions.AddPageRoute("/Structure/Detail", "/Structure/{id?}");
});

//Policy
builder.Services.AddAuthorization(options =>
{
    //Admin
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[admin]"));
    });
    //Authorization
    options.AddPolicy("Authorization", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[authorization]"));
    });
    //Admin
    options.AddPolicy("Dashboard", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[dashboard]"));
    });
    //Admin
    options.AddPolicy("Employee", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[employee]"));
    });
    //Admin
    options.AddPolicy("HRService", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[hrservice]"));
    });
    //Admin
    options.AddPolicy("Organization", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[organization]"));
    });
    //Admin
    options.AddPolicy("Report", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[report]"));
    });
    //Admin
    options.AddPolicy("Setting", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => (context.User.FindFirst(ClaimTypes.Authentication) != null ? context.User.FindFirst(ClaimTypes.Authentication).Value : "").ToLower().Contains("[setting]"));
    });
});

builder.Services.Configure<WebAPIModels>(builder.Configuration.GetSection("WebAPI"));
builder.Services.Configure<WebInfoModels>(builder.Configuration.GetSection("WebInfo"));

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
//{
//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
//    options.Authority = builder.Configuration["Client:Authority"];
//    options.ClientId = builder.Configuration["Client:ClientId"];
//    options.ClientSecret = builder.Configuration["Client:ClientSecret"];
//    options.ResponseType = "code";
//    options.Scope.Add("openid");
//    options.Scope.Add("profile"); 
//    options.Scope.Add("api1");
//    options.SaveTokens = true;
//    options.GetClaimsFromUserInfoEndpoint = true;
//    options.ClaimActions.MapUniqueJsonKey("sub", "sub");
//    options.ClaimActions.MapUniqueJsonKey("profile", "profile");
//    options.ClaimActions.MapUniqueJsonKey("name", "name");

//    options.Events = new OpenIdConnectEvents
//    {
//        OnTokenValidated = context =>
//        {
//            var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;
//            var subClaim = claimsIdentity.FindFirst(JwtClaimTypes.Subject)?.Value;

//            if (string.IsNullOrEmpty(subClaim))
//            {
//                subClaim = context.SecurityToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
//                if (!string.IsNullOrEmpty(subClaim))
//                {
//                    claimsIdentity.AddClaim(new Claim(JwtClaimTypes.Subject, subClaim));
//                }
//            }

//            return Task.CompletedTask;
//        }
//    };
//})
;

IdentityModelEventSource.ShowPII = true;

// เพิ่มบริการสำหรับ Session
builder.Services.AddDistributedMemoryCache(); // ใช้หน่วยความจำเพื่อเก็บข้อมูล Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(200); // กำหนดเวลาหมดอายุของ Session
    options.Cookie.HttpOnly = true; // ป้องกันการเข้าถึง Session ผ่าน JavaScript
    options.Cookie.IsEssential = true; // ทำให้ Cookie มีความสำคัญ
});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.ExpireTimeSpan = TimeSpan.FromDays(5);
//});

//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.Limits.MaxRequestLineSize = 8192; // ขยายขนาด URL ที่รองรับ
//    serverOptions.Limits.MaxRequestHeadersTotalSize = 1048576; // 1MB
//    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50 MB
//});

//Antiforgery
//builder.Services.AddAntiforgery(options =>
//{
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
