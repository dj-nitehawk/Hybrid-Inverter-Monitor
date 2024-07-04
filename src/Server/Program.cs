global using FastEndpoints;
using InverterMon.Server.BatteryService;
using InverterMon.Server.InverterService;
using InverterMon.Server.Persistance;
using InverterMon.Server.Persistance.Settings;
using System.Globalization;
using System.Net;

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
   .AddSingleton<JkBms>()
   .AddSingleton<Database>();

if (!bld.Environment.IsDevelopment())
{
    bld.Services
       .AddHostedService<CommandExecutor>()
       .AddHostedService<StatusRetriever>();
}

bld.Services.AddFastEndpoints();

var app = bld.Build();

if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();

app.UseBlazorFrameworkFiles()
   .UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseRouting()
   .UseFastEndpoints(c => c.Endpoints.RoutePrefix = "api");
app.Run();