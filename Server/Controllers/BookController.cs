using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class BookController(IBookRepository bookRepository) : GenericController<Book>(bookRepository)
    {
        //GET api/book/search?text=achebe&categoryId=1
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? text, int? authorId, int? categoryId) =>
            Ok(await bookRepository.Search(text, authorId, categoryId));
    }
}
