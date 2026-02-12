using Diabits.Web;
using Diabits.Web.Features.Auth.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices();

// Auth state provider
builder.Services.AddSingleton<JwtAuthStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthStateProvider>());

builder.Services.AddSingleton<TokenStorage>();
builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<AuthorizedHandler>();
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
})
.AddHttpMessageHandler<AuthorizedHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

await builder.Build().RunAsync();