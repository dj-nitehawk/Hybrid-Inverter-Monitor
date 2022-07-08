global using FastEndpoints;
using FastEndpoints.Swagger;
using InverterMon.Server.InverterService;
using System.Net;

var builder = WebApplication.CreateBuilder();

builder.WebHost.ConfigureKestrel(o =>
{
    o.Listen(IPAddress.Any, 80);
    //o.Listen(IPAddress.Any, 443, o => o.UseHttps("ssl.crt"));
});

builder.Services.AddSingleton(new CommandQueue());

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerDoc();
else
    builder.Services.AddHostedService<InverterWorker>();

builder.Services.AddFastEndpoints();

var app = builder.Build();

//app.UseHttpsRedirection();

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