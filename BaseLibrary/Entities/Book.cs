using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        [RegularExpression(@"^(97[89])?\d{9}[\dXx]$", ErrorMessage = "ISBN must be 10 or 13 digits, without dashes.")]
        public string? Isbn { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Range(0, 100000)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Range(1000, 2100)]
        public int PublishedYear { get; set; }

        //Relationship of many to one: many books map to one author and one category
        [Range(1, int.MaxValue, ErrorMessage = "Please select an author.")]
        public int AuthorId { get; set; }

        public Author? Author { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
