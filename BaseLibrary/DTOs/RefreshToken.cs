using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class RefreshToken
    {
        [Required]
        public string? Token { get; set; }
    }
}
