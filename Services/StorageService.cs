// Services/StorageService.cs
using Blazored.LocalStorage;
using ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;
using Microsoft.JSInterop;

namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Services;

public class StorageService : IStorageService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private readonly string _namespace;

    public StorageService(IJSRuntime jsRuntime, string appNamespace)
    {
        _jsRuntime = jsRuntime;
        _namespace = appNamespace;
    }

    public async Task<T> GetItemValueByKeyAsync<T>(string key)
    {
        var data = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _namespace);
        if (string.IsNullOrEmpty(data))
        {
            return default;
        }

        var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
        return (T)settings[key];
    }

    public T GetItemValueByKey<T>(string key)
    {
        var data = _jsRuntime.InvokeAsync<string>("localStorage.getItem", _namespace).Result; // Bloccante
        if (string.IsNullOrEmpty(data))
        {
            return default;
        }

        var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
        return (T)settings[key];
    }



    public async Task SetItemValueById(string id, object newValue)
    {
        var data = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _namespace);
        var settings = string.IsNullOrEmpty(data) ? new Dictionary<string, object>() : Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
        settings[id] = newValue;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _namespace, Newtonsoft.Json.JsonConvert.SerializeObject(settings));
    }

    public StorageService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<T> GetItemAsync<T>(string key)
    {
        return await _localStorage.GetItemAsync<T>(key);
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await _localStorage.SetItemAsync(key, value);
    }
}