using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ClientLibrary.Helpers
{
    //Tells Blazor who is logged in, based on the JWT saved in local storage
    public class CustomAuthenticationStateProvider(LocalStorageService localStorage, HttpClient httpClient) : AuthenticationStateProvider
    {
        private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetValidTokenAsync();
            if (token is null) return Anonymous;

            //The JWT uses short claim names ("unique_name", "role") once serialised
            var identity = new ClaimsIdentity(JwtParser.ReadClaims(token), "jwt", "unique_name", "role");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        //Returns a usable access token, refreshing it with the refresh token if it has expired
        public async Task<string?> GetValidTokenAsync()
        {
            var session = await localStorage.GetSessionAsync();
            if (string.IsNullOrEmpty(session?.Token)) return null;

            if (!JwtParser.IsExpired(JwtParser.ReadClaims(session.Token))) return session.Token;
            if (string.IsNullOrEmpty(session.RefreshToken))
            {
                await localStorage.RemoveSessionAsync();
                return null;
            }

            try
            {
                var response = await httpClient.PostAsJsonAsync("api/authentication/refresh-token", new RefreshToken { Token = session.RefreshToken });
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result is { Flag: true })
                {
                    await localStorage.SetSessionAsync(new UserSession { Token = result.Token, RefreshToken = result.RefreshToken });
                    return result.Token;
                }
            }
            catch (HttpRequestException)
            {
                //Server unreachable: fall through and log out
            }

            await localStorage.RemoveSessionAsync();
            return null;
        }

        public async Task MarkUserAsLoggedInAsync(UserSession session)
        {
            await localStorage.SetSessionAsync(session);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await localStorage.RemoveSessionAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }
    }
}
