namespace ServerLibrary.Helpers
{
    public static class Constants
    {
        //const so they can be used in [Authorize(Roles = ...)] attributes
        public const string Admin = "Admin";

        public const string User = "User";
    }
}
