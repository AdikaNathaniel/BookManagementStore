using BaseLibrary.Entities;

namespace ClientLibrary.Services.Contracts
{
    public interface IBookService : IGenericService<Book>
    {
        Task<List<Book>> SearchAsync(string? text, int? categoryId);
    }
}
