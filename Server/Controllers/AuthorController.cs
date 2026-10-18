using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController(IGenericRepositoryInterface<Author> repository) : GenericController<Author>(repository)
    {
    }
}
