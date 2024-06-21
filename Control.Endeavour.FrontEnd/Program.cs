using Control.Endeavour.FrontEnd;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.Services.Services.Storage;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using DevExpress.Blazor;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Control.Endeavour.FrontEnd.StateContainer.ManagementTray;
using System.Globalization;
using Microsoft.JSInterop;
using System.Reflection;
using Telerik.Blazor.Services;
using Control.Endeavour.FrontEnd.Services.Interfaces.AIService;
using Control.Endeavour.FrontEnd.Services.Services.AIService;
using Control.Endeavour.FrontEnd.Services.Services.Handler;
using Control.Endeavour.FrontEnd.Services.Interfaces.DriveService;
using Control.Endeavour.FrontEnd.Services.Services.DriveService;
using Telerik.ReportViewer.BlazorNative.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<ProfileStateContainer>();
builder.Services.AddSingleton<DocumentsStateContainer>();
builder.Services.AddSingleton<FilingStateContainer>();
builder.Services.AddScoped<EventAggregatorService>();
builder.Services.AddSingleton<ManagementTrayStateContainer>();

//AIService
builder.Services.AddScoped<IAnswerGeneratorService, AnswerGeneratorService>();

//GraphService
builder.Services.AddScoped<IGraphService, GraphService>();

//Session storage y Local storage
builder.Services.AddScoped<ISessionStorage, SessionStorageService>();
builder.Services.AddScoped<ILocalStorage, LocalStorageService>();

//Manejo del token de seguridad

#region Http con handler
builder.Services.AddTransient<AuthorizationMessageHandler>();
builder.Services.AddHttpClient("ApiClient", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceConfiguration:UrlApiGateway"));
    })
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")); 
#endregion

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationJWTService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationJWTService>(provider => provider.GetRequiredService<AuthenticationJWTService>());
builder.Services.AddScoped<IAuthenticationJWT, AuthenticationJWTService>(provider => provider.GetRequiredService<AuthenticationJWTService>());
builder.Services.AddScoped<RenewTokenService>();
builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization.*", LogLevel.None);
builder.Logging.AddFilter("System.Net.Http.HttpClient.ApiClient.LogicalHandler", LogLevel.None);
builder.Logging.AddFilter("System.Net.Http.HttpClient.ApiClient.ClientHandler", LogLevel.None);

builder.Services.AddSpeechSynthesis();

//Para el funcionamiento del DevExpress
builder.Services.AddDevExpressBlazor(configure => configure.BootstrapVersion = BootstrapVersion.v5);

//Traducciones

builder.Services.AddLocalization();
var host = builder.Build();
var js = host.Services.GetRequiredService<IJSRuntime>();
var culture = await js.InvokeAsync<string>("culture.get");
//Las 2 siguientes para que se hagan las traducciones en los componentes de Telerik
builder.Services.AddTelerikBlazor();
builder.Services.AddSingleton(typeof(ITelerikReportingStringLocalizer), typeof(TelerikReportStringLocalizer));
builder.Services.AddSingleton(typeof(ITelerikStringLocalizer), typeof(TelerikResxLocalizer));

if (culture == null)
{
    var defaultCulture = new CultureInfo("es-ES");
    CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
    CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
}
else
{
    var userCulture = new CultureInfo(culture);
    CultureInfo.DefaultThreadCurrentCulture = userCulture;
    CultureInfo.DefaultThreadCurrentUICulture = userCulture;
}

await builder.Build().RunAsync();