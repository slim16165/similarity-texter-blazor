namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

public interface IStorageService
{
    Task<T> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
}