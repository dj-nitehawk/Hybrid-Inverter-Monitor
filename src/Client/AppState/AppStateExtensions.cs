using Microsoft.JSInterop;
using System.Text.Json;

namespace InverterMon.Client.AppState;

public static class StateExtensions
{
    public static async Task<T?> LoadAsync<T>(this IJSRuntime jsRuntime) where T : class
    {
        var data = await jsRuntime.InvokeAsync<string>("localStorage.getItem", typeof(T).FullName);

        if (!string.IsNullOrEmpty(data))
            return JsonSerializer.Deserialize<T>(data);

        return null;
    }

    public static ValueTask SaveAsync<T>(this IJSRuntime jsRuntime, T state) where T : class
    {
        return jsRuntime.InvokeVoidAsync("localStorage.setItem", typeof(T).FullName, JsonSerializer.Serialize(state));
    }
}
