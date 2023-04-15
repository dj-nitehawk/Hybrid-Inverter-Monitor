using InverterMon.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    Timeout = TimeSpan.FromSeconds(3)
});
_ = InverterMon.Client.Pages.Index.StartStatusStreaming(builder.HostEnvironment.BaseAddress);
_ = InverterMon.Client.Pages.BMS.StartStatusStreaming(builder.HostEnvironment.BaseAddress);
await builder.Build().RunAsync();