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

var builder = WebApplication.CreateBuilder();
_ = int.TryParse(builder.Configuration["LaunchSettings:WebPort"] ?? "80", out var port);
builder.WebHost.ConfigureKestrel(o => o.Listen(IPAddress.Any, port));
builder.Services.AddSingleton<UserSettings>();
builder.Services.AddSingleton<CommandQueue>();
builder.Services.AddSingleton<JkBms>();
builder.Services.AddSingleton<Database>();
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<CommandExecutor>();
    builder.Services.AddHostedService<StatusRetriever>();
}
builder.Services.AddFastEndpoints();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseRouting();
app.UseAuthorization();
app.UseFastEndpoints(c => c.Endpoints.RoutePrefix = "api");
app.Run();