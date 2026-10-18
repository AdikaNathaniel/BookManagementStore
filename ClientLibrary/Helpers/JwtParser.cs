using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ClientLibrary.Helpers
{
    //Reads the claims out of a JWT. The client only reads them for display;
    //the server is what actually checks the token's signature.
    public static class JwtParser
    {
        public static List<Claim> ReadClaims(string token)
        {
            var claims = new List<Claim>();
            var parts = token.Split('.');
            if (parts.Length != 3) return claims;

            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

            using var document = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in property.Value.EnumerateArray())
                        claims.Add(new Claim(property.Name, item.ToString()));
                }
                else
                {
                    claims.Add(new Claim(property.Name, property.Value.ToString()));
                }
            }
            return claims;
        }

        public static bool IsExpired(IEnumerable<Claim> claims)
        {
            var exp = claims.FirstOrDefault(_ => _.Type == "exp")?.Value;
            if (exp is null || !long.TryParse(exp, out var seconds)) return true;
            //Treat tokens as expired a minute early so a request never goes out with a dying token
            return DateTimeOffset.FromUnixTimeSeconds(seconds) <= DateTimeOffset.UtcNow.AddMinutes(1);
        }
    }
}
