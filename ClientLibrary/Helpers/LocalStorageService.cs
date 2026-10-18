using Microsoft.JSInterop;
using System.Text.Json;

namespace ClientLibrary.Helpers
{
    public class LocalStorageService(IJSRuntime jsRuntime)
    {
        private const string StorageKey = "bookstore-session";

        public async Task<UserSession?> GetSessionAsync()
        {
            var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                return JsonSerializer.Deserialize<UserSession>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public async Task SetSessionAsync(UserSession session) =>
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, JsonSerializer.Serialize(session));

        public async Task RemoveSessionAsync() =>
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }
}
