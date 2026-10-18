using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementations
{
    //Calls the API's CRUD endpoints. Routes follow the controller names, e.g. Book -> api/book
    public class GenericService<T>(HttpClient httpClient, CustomAuthenticationStateProvider authStateProvider) : IGenericService<T>
    {
        protected readonly HttpClient HttpClient = httpClient;

        protected static string BaseUrl => $"api/{typeof(T).Name.ToLower()}";

        public async Task<List<T>> GetAllAsync() =>
            await HttpClient.GetFromJsonAsync<List<T>>(BaseUrl) ?? [];

        public async Task<T?> GetByIdAsync(int id)
        {
            var response = await HttpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return default;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<GeneralResponse> InsertAsync(T item) =>
            await SendAsync(HttpMethod.Post, BaseUrl, item);

        public async Task<GeneralResponse> UpdateAsync(T item) =>
            await SendAsync(HttpMethod.Put, BaseUrl, item);

        public async Task<GeneralResponse> DeleteAsync(int id) =>
            await SendAsync(HttpMethod.Delete, $"{BaseUrl}/{id}", default);

        //Sends a request that needs the administrator's token
        private async Task<GeneralResponse> SendAsync(HttpMethod method, string url, T? body)
        {
            var token = await authStateProvider.GetValidTokenAsync();
            if (token is null) return new GeneralResponse(false, "Your session has expired. Please log in again.");

            using var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (body is not null) request.Content = JsonContent.Create(body);

            try
            {
                var response = await HttpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new GeneralResponse(false, "Please log in again.");
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    return new GeneralResponse(false, "Only administrators can change the catalogue.");

                //Validation failures come back as ProblemDetails, not GeneralResponse
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                return result?.Message is not null
                    ? result
                    : new GeneralResponse(response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "Saved" : "The server rejected the request. Check the form and try again.");
            }
            catch (HttpRequestException)
            {
                return new GeneralResponse(false, "Could not reach the server. Is the API running?");
            }
            catch (System.Text.Json.JsonException)
            {
                return new GeneralResponse(false, "Unexpected response from the server.");
            }
        }
    }
}
