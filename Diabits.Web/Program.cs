using ApexCharts;

using Diabits.Web;
using Diabits.Web.Infrastructure.Api;
using Diabits.Web.Services.Auth;
using Diabits.Web.Services.HealthData;
using Diabits.Web.Services.Invites;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.ClearAfterNavigation = true;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});

builder.Services.AddApexCharts(e =>
{
    e.GlobalOptions = new ApexChartBaseOptions
    {
        Debug = true,
        Theme = new Theme
        {
            Mode = Mode.Light,
            Palette = PaletteType.Palette1,
            Monochrome = new ThemeMonochrome {
                Enabled = true,
                Color = "#e27396",
                ShadeTo = Mode.Light,
                ShadeIntensity = 1
            }
        }
    };
});

builder.Services.AddSingleton<JwtAuthStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddSingleton<TokenStorage>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<InviteService>();
builder.Services.AddScoped<HealthDataService>();

builder.Services.AddScoped<AuthorizationHandler>();

builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
})
.AddHttpMessageHandler<AuthorizationHandler>(); // Automatically adds JWT to all requests

await builder.Build().RunAsync();