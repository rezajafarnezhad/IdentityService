using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddTestUsers(new List<TestUser>()
    {
        new TestUser()
        {
            IsActive = true,
            Password = "123456",
            Username = "ali",
            SubjectId = "1",
        }
    })
    .AddInMemoryClients(new List<Client>()
    {
        new Client()
        {
            ClientName = "FrontEnd web",
            ClientId = "FrontEndWeb",
            ClientSecrets = {new Secret("123456".Sha256())},
            AllowedGrantTypes =[GrantType.ClientCredentials],
            AllowedScopes = ["orderService.FullAccess"]

        },
        new Client()
        {
            ClientName = "FrontEnd web Code",
            ClientId = "frontendwebcode",
            ClientSecrets = {new Secret("123456".Sha256())},
            AllowedGrantTypes =[GrantType.AuthorizationCode],
            RedirectUris ={"https://localhost:7242/signin-oidc"},
            PostLogoutRedirectUris = {"https://localhost:7242/signout-callback-oidc"},
            AllowedScopes = {"openid", "profile", "orderService.GetOrder", "basketService.FullAccess", "apigatewayforweb.FullAccess" }
        },
        new Client()
        {
            ClientName = "Admin web Code",
            ClientId = "Adminfrontendwebcode",
            ClientSecrets = {new Secret("123456".Sha256())},
            AllowedGrantTypes =[GrantType.AuthorizationCode],
            RedirectUris ={"https://localhost:7242/signin-oidc"},
            PostLogoutRedirectUris = {"https://localhost:7242/signout-callback-oidc"},
            AllowedScopes = {"openid", "profile", "orderService.GetOrder", "basketService.FullAccess" }
        }
    })
    .AddInMemoryIdentityResources(new List<IdentityResource>()
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    })
    .AddInMemoryApiScopes(new List<ApiScope>()
    {
        new ApiScope("orderService.Management"),
        new ApiScope("orderService.GetOrder"),
        new ApiScope("basketService.FullAccess"),
        new ApiScope("apigatewayforweb.FullAccess"),
    })
    .AddInMemoryApiResources(new List<ApiResource>()
    {
        new ApiResource("orderService","orderServiceApi")
        {
            Scopes ={ "orderService.Management", "orderService.GetOrder" }
        },
        new ApiResource("basketService","basketServiceApi")
        {
            Scopes ={ "basketService.FullAccess" }
        },
        new ApiResource("apigatewayforweb","apigatewayforwebServiceApi")
        {
            Scopes ={ "apigatewayforweb.FullAccess" }
        }
    })

    ;



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
app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
