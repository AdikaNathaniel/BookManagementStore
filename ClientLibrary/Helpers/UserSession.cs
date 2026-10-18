namespace ClientLibrary.Helpers
{
    //What is kept in the browser's local storage after logging in
    public class UserSession
    {
        public string? Token { get; set; }

        public string? RefreshToken { get; set; }
    }
}
