using BaseLibrary.Entities;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IBookRepository : IGenericRepositoryInterface<Book>
    {
        //Matches the search text against title, ISBN and author name; filters are optional
        Task<List<Book>> Search(string? text, int? authorId, int? categoryId);
    }
}
