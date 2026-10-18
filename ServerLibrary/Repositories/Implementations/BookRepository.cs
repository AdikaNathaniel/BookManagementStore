using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class BookRepository(AppDbContext appDbContext) : IBookRepository
    {
        public async Task<List<Book>> GetAll() => await Search(null, null, null);

        public async Task<Book?> GetById(int id) =>
            await BooksWithDetails().FirstOrDefaultAsync(_ => _.Id == id);

        public async Task<List<Book>> Search(string? text, int? authorId, int? categoryId)
        {
            var query = BooksWithDetails();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var term = text.Trim();
                query = query.Where(_ => _.Title!.Contains(term) || _.Isbn!.Contains(term) || _.Author!.Name!.Contains(term));
            }
            if (authorId is not null) query = query.Where(_ => _.AuthorId == authorId);
            if (categoryId is not null) query = query.Where(_ => _.CategoryId == categoryId);

            return await query.OrderBy(_ => _.Title).ToListAsync();
        }

        public async Task<GeneralResponse> Insert(Book item)
        {
            var problem = await Validate(item);
            if (problem is not null) return new GeneralResponse(false, problem);

            item.Id = 0;
            //Only the foreign keys are used; don't let EF try to insert the related objects
            item.Author = null;
            item.Category = null;
            appDbContext.Books.Add(item);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Book added");
        }

        public async Task<GeneralResponse> Update(Book item)
        {
            var book = await appDbContext.Books.FindAsync(item.Id);
            if (book is null) return new GeneralResponse(false, "Book not found");

            var problem = await Validate(item);
            if (problem is not null) return new GeneralResponse(false, problem);

            book.Title = item.Title;
            book.Isbn = item.Isbn;
            book.Description = item.Description;
            book.Price = item.Price;
            book.StockQuantity = item.StockQuantity;
            book.PublishedYear = item.PublishedYear;
            book.AuthorId = item.AuthorId;
            book.CategoryId = item.CategoryId;
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Book updated");
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var book = await appDbContext.Books.FindAsync(id);
            if (book is null) return new GeneralResponse(false, "Book not found");

            appDbContext.Books.Remove(book);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Book deleted");
        }

        private IQueryable<Book> BooksWithDetails() =>
            appDbContext.Books.AsNoTracking().Include(_ => _.Author).Include(_ => _.Category);

        //Returns an error message, or null when the book is valid
        private async Task<string?> Validate(Book item)
        {
            if (!await appDbContext.Authors.AnyAsync(_ => _.Id == item.AuthorId)) return "Selected author does not exist";
            if (!await appDbContext.Categories.AnyAsync(_ => _.Id == item.CategoryId)) return "Selected category does not exist";
            if (await appDbContext.Books.AnyAsync(_ => _.Isbn == item.Isbn && _.Id != item.Id)) return "Another book already has this ISBN";
            return null;
        }
    }
}
