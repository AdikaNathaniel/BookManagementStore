using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        //Relationship of one to many: one category can hold many books
        [JsonIgnore]
        public List<Book>? Books { get; set; }
    }
}
