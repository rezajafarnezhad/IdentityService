using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityAppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<IdentityAppDbContext>(op=>op.UseSqlServer(builder.Configuration.GetConnectionString("AspIdentityConnection")));

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(new List<Client>()
    {
        new Client()
        {
            ClientName = "FrontEnd web",
            ClientId = "FrontEndWeb",
            ClientSecrets = { new Secret("123456".Sha256()) },
            AllowedGrantTypes = [GrantType.ClientCredentials],
            AllowedScopes = ["orderService.FullAccess"]

        },
        new Client()
        {
            ClientName = "FrontEnd web Code",
            ClientId = "frontendwebcode",
            ClientSecrets = { new Secret("123456".Sha256()) },
            AllowedGrantTypes = [GrantType.AuthorizationCode],
            RedirectUris = { "https://localhost:7242/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:7242/signout-callback-oidc" },
            AllowedScopes =
            {
                "openid", "profile", "orderService.GetOrder", "basketService.FullAccess", "apigatewayforweb.FullAccess","roles"
            },
            AllowOfflineAccess = true, //refresh token
            AccessTokenLifetime = 6000,
            RefreshTokenUsage = TokenUsage.ReUse,
            RefreshTokenExpiration = TokenExpiration.Sliding,
        },
        new Client()
        {
            ClientName = "Admin web Code",
            ClientId = "Adminfrontendwebcode",
            ClientSecrets = { new Secret("123456".Sha256()) },
            AllowedGrantTypes = [GrantType.AuthorizationCode],
            RedirectUris = { "https://localhost:7254/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:7254/signout-callback-oidc" },
            AllowedScopes = { "openid", "profile", "orderService.GetOrder", "basketService.FullAccess" ,"apigatewayforadmin.FullAccess","productServicePanel.FullAccess","roles"}
        }
    })
    .AddInMemoryIdentityResources(new List<IdentityResource>()
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource("roles","User role(s)",new List<string>(){"role"}),
    })
    .AddInMemoryApiScopes(new List<ApiScope>()
    {
        new ApiScope("orderService.Management"),
        new ApiScope("orderService.GetOrder"),
        new ApiScope("basketService.FullAccess"),
        new ApiScope("apigatewayforweb.FullAccess",new List<string>(){"role"}),
        new ApiScope("apigatewayforadmin.FullAccess",new List<string>(){"role"}),
        new ApiScope("productServicePanel.FullAccess"),
    })
    .AddInMemoryApiResources(new List<ApiResource>()
    {
        new ApiResource("orderService", "orderServiceApi")
        {
            Scopes = { "orderService.Management", "orderService.GetOrder" }
        },
        new ApiResource("basketService", "basketServiceApi")
        {
            Scopes = { "basketService.FullAccess" }
        },
        new ApiResource("apigatewayforweb", "apigatewayforwebServiceApi")
        {
            Scopes = { "apigatewayforweb.FullAccess" }
        },
        new ApiResource("apigatewayforadmin", "apigatewayforAdminServiceApi")
        {
            Scopes = { "apigatewayforadmin.FullAccess" }
        },
        new ApiResource("productService", "ProductServicePanel")
        {
            Scopes = { "productServicePanel.FullAccess" }
        }
    })
    .AddAspNetIdentity<IdentityUser>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();


}
SeedData.Seed(app);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
