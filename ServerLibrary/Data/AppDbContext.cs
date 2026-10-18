using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<SystemRole> SystemRoles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(book =>
            {
                book.Property(b => b.Price).HasPrecision(10, 2);
                book.HasIndex(b => b.Isbn).IsUnique();

                //An author or category that still has books cannot be deleted
                book.HasOne(b => b.Author).WithMany(a => a.Books)
                    .HasForeignKey(b => b.AuthorId).OnDelete(DeleteBehavior.Restrict);
                book.HasOne(b => b.Category).WithMany(c => c.Books)
                    .HasForeignKey(b => b.CategoryId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique();

            SeedCatalogue(modelBuilder);
        }

        //Sample data so the store has something to show on first run
        private static void SeedCatalogue(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fiction" },
                new Category { Id = 2, Name = "Science" },
                new Category { Id = 3, Name = "Technology" });

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "Chinua Achebe", Biography = "Nigerian novelist, poet and critic." },
                new Author { Id = 2, Name = "Chimamanda Ngozi Adichie", Biography = "Nigerian author of novels, short stories and essays." },
                new Author { Id = 3, Name = "Stephen Hawking", Biography = "English theoretical physicist and cosmologist." },
                new Author { Id = 4, Name = "Robert C. Martin", Biography = "American software engineer and author." });

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1, Title = "Things Fall Apart", Isbn = "9780385474542", Price = 14.99m, StockQuantity = 12,
                    PublishedYear = 1958, AuthorId = 1, CategoryId = 1,
                    Description = "The story of Okonkwo and the arrival of colonialism in an Igbo village."
                },
                new Book
                {
                    Id = 2, Title = "Half of a Yellow Sun", Isbn = "9781400095209", Price = 17.50m, StockQuantity = 8,
                    PublishedYear = 2006, AuthorId = 2, CategoryId = 1,
                    Description = "Lives intertwined during the Nigerian Civil War."
                },
                new Book
                {
                    Id = 3, Title = "A Brief History of Time", Isbn = "9780553380163", Price = 18.00m, StockQuantity = 5,
                    PublishedYear = 1988, AuthorId = 3, CategoryId = 2,
                    Description = "From the Big Bang to black holes."
                },
                new Book
                {
                    Id = 4, Title = "Clean Code", Isbn = "9780132350884", Price = 39.99m, StockQuantity = 10,
                    PublishedYear = 2008, AuthorId = 4, CategoryId = 3,
                    Description = "A handbook of agile software craftsmanship."
                });
        }
    }
}
