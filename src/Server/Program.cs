global using FastEndpoints;
using FastEndpoints.Swagger;
using InverterMon.Server.Database;
using InverterMon.Server.InverterService;
using System.Net;

var builder = WebApplication.CreateBuilder();
builder.WebHost.ConfigureKestrel(o => o.Listen(IPAddress.Any, 80));
builder.Services.AddSingleton<CommandQueue>();
builder.Services.AddSingleton<Database>();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerDoc();
}
else
{
    builder.Services.AddHostedService<CommandExecutor>();
    builder.Services.AddHostedService<StatusRetriever>();
}
builder.Services.AddFastEndpoints();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseOpenApi();
    app.UseSwaggerUi3(s => s.ConfigureDefaults());
}
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseRouting();
app.UseAuthorization();
app.UseFastEndpoints(c => c.RoutingOptions = o => o.Prefix = "api");
app.Run();