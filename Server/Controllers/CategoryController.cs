using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController(IGenericRepositoryInterface<Category> repository) : GenericController<Category>(repository)
    {
    }
}
