using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementations
{
    public class UserAccountService(HttpClient httpClient, CustomAuthenticationStateProvider authStateProvider) : IUserAccountService
    {
        private const string AuthUrl = "api/authentication";

        public async Task<GeneralResponse> RegisterAsync(Register user)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                //Validation failures come back as ProblemDetails, which have no Message
                return result?.Message is not null ? result : new GeneralResponse(false, "Please check the form and try again.");
            }
            catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException)
            {
                return new GeneralResponse(false, "Could not reach the server. Is the API running?");
            }
        }

        public async Task<LoginResponse> LoginAsync(Login user)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result?.Message is null) return new LoginResponse(false, "Please check the form and try again.");

                if (result.Flag)
                {
                    await authStateProvider.MarkUserAsLoggedInAsync(new UserSession { Token = result.Token, RefreshToken = result.RefreshToken });
                }
                return result;
            }
            catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException)
            {
                return new LoginResponse(false, "Could not reach the server. Is the API running?");
            }
        }

        public async Task LogoutAsync() => await authStateProvider.MarkUserAsLoggedOutAsync();
    }
}
