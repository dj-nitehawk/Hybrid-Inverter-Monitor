global using FastEndpoints;
using FastEndpoints.Swagger;
using InverterMon.Server.InverterService;

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton(new CommandQueue());
if (!builder.Environment.IsDevelopment())
    builder.Services.AddHostedService<InverterWorker>();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseRouting();
app.UseAuthorization();
app.UseFastEndpoints(c => c.RoutingOptions = o => o.Prefix = "api");
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());
app.Run("http://::80");
