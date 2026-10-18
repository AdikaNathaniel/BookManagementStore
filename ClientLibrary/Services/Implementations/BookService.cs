using BaseLibrary.Entities;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementations
{
    public class BookService(HttpClient httpClient, CustomAuthenticationStateProvider authStateProvider)
        : GenericService<Book>(httpClient, authStateProvider), IBookService
    {
        public async Task<List<Book>> SearchAsync(string? text, int? categoryId)
        {
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(text)) query.Add($"text={Uri.EscapeDataString(text.Trim())}");
            if (categoryId is not null) query.Add($"categoryId={categoryId}");

            var url = query.Count == 0 ? BaseUrl : $"{BaseUrl}/search?{string.Join("&", query)}";
            return await HttpClient.GetFromJsonAsync<List<Book>>(url) ?? [];
        }
    }
}
