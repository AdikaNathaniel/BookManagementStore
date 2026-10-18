using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(2000)]
        public string? Biography { get; set; }

        //Relationship of one to many: one author can write many books
        [JsonIgnore]
        public List<Book>? Books { get; set; }
    }
}
