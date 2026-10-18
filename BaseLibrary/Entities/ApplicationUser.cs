namespace BaseLibrary.Entities
{
    public class ApplicationUser
    {
        //Any user of the store, whether an administrator or a customer
        public int Id { get; set; }

        public string? Fullname { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
