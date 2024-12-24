global using FastEndpoints;
using System.Globalization;
using System.Net;
using InverterMon.Server;
using InverterMon.Server.BatteryService;
using InverterMon.Server.InverterService;
using InverterMon.Server.Persistance;
using InverterMon.Server.Persistance.Settings;

//avoid parsing issues with non-english cultures
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var bld = WebApplication.CreateBuilder();

_ = int.TryParse(bld.Configuration["LaunchSettings:WebPort"] ?? "80", out var port);
bld.WebHost.ConfigureKestrel(o => o.Listen(IPAddress.Any, port));

bld.Services
   .AddSingleton<UserSettings>()
   .AddSingleton<CommandQueue>()
   .AddSingleton<Database>()
   .AddSingleton<JkBms>();

if (!bld.Environment.IsDevelopment())
{
    bld.Services
       .AddHostedService<CommandExecutor>()
       .AddHostedService<StatusRetriever>();
}

bld.Services.AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All);

var app = bld.Build();

if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();

app.UseBlazorFrameworkFiles()
   .UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseRouting()
   .UseFastEndpoints(
       c =>
       {
           c.Endpoints.RoutePrefix = "api";
           c.Binding.ReflectionCache.AddFromInverterMonServer();
       });
app.Run();