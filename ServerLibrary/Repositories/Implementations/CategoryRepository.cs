using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class CategoryRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Category>
    {
        public async Task<List<Category>> GetAll() =>
            await appDbContext.Categories.AsNoTracking().OrderBy(_ => _.Name).ToListAsync();

        public async Task<Category?> GetById(int id) =>
            await appDbContext.Categories.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id);

        public async Task<GeneralResponse> Insert(Category item)
        {
            if (await NameTaken(item.Name!, item.Id)) return new GeneralResponse(false, "Category already exists");

            item.Id = 0;
            appDbContext.Categories.Add(item);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Category added");
        }

        public async Task<GeneralResponse> Update(Category item)
        {
            var category = await appDbContext.Categories.FindAsync(item.Id);
            if (category is null) return new GeneralResponse(false, "Category not found");
            if (await NameTaken(item.Name!, item.Id)) return new GeneralResponse(false, "Another category already has this name");

            category.Name = item.Name;
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Category updated");
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var category = await appDbContext.Categories.FindAsync(id);
            if (category is null) return new GeneralResponse(false, "Category not found");
            if (await appDbContext.Books.AnyAsync(_ => _.CategoryId == id))
                return new GeneralResponse(false, "This category still has books in it. Move or delete them first.");

            appDbContext.Categories.Remove(category);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Category deleted");
        }

        private async Task<bool> NameTaken(string name, int excludeId) =>
            await appDbContext.Categories.AnyAsync(_ => _.Name!.ToLower() == name.Trim().ToLower() && _.Id != excludeId);
    }
}
