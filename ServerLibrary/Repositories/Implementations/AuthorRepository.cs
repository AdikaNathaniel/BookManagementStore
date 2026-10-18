using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class AuthorRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Author>
    {
        public async Task<List<Author>> GetAll() =>
            await appDbContext.Authors.AsNoTracking().OrderBy(_ => _.Name).ToListAsync();

        public async Task<Author?> GetById(int id) =>
            await appDbContext.Authors.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id);

        public async Task<GeneralResponse> Insert(Author item)
        {
            if (await NameTaken(item.Name!, item.Id)) return new GeneralResponse(false, "Author already exists");

            item.Id = 0;
            appDbContext.Authors.Add(item);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Author added");
        }

        public async Task<GeneralResponse> Update(Author item)
        {
            var author = await appDbContext.Authors.FindAsync(item.Id);
            if (author is null) return new GeneralResponse(false, "Author not found");
            if (await NameTaken(item.Name!, item.Id)) return new GeneralResponse(false, "Another author already has this name");

            author.Name = item.Name;
            author.Biography = item.Biography;
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Author updated");
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var author = await appDbContext.Authors.FindAsync(id);
            if (author is null) return new GeneralResponse(false, "Author not found");
            if (await appDbContext.Books.AnyAsync(_ => _.AuthorId == id))
                return new GeneralResponse(false, "This author still has books in the store. Delete or reassign them first.");

            appDbContext.Authors.Remove(author);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Author deleted");
        }

        private async Task<bool> NameTaken(string name, int excludeId) =>
            await appDbContext.Authors.AnyAsync(_ => _.Name!.ToLower() == name.Trim().ToLower() && _.Id != excludeId);
    }
}
