using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    //Shared CRUD endpoints: anyone can browse, only administrators can change the catalogue
    [ApiController]
    [Authorize(Roles = Constants.Admin)]
    public abstract class GenericController<T>(IGenericRepositoryInterface<T> repository) : ControllerBase where T : class
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await repository.GetAll());

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await repository.GetById(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add(T model)
        {
            var result = await repository.Insert(model);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(T model)
        {
            var result = await repository.Update(model);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await repository.DeleteById(id);
            return result.Flag ? Ok(result) : BadRequest(result);
        }
    }
}
