using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts
{
    public interface IUserAccountService
    {
        Task<GeneralResponse> RegisterAsync(Register user);

        Task<LoginResponse> LoginAsync(Login user);

        Task LogoutAsync();
    }
}
