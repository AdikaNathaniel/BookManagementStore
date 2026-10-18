using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts
{
    public interface IGenericService<T>
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<GeneralResponse> InsertAsync(T item);

        Task<GeneralResponse> UpdateAsync(T item);

        Task<GeneralResponse> DeleteAsync(int id);
    }
}
