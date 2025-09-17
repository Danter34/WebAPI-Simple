using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Models.DTO;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorRepository;

        public AuthorController(AppDbContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }

        [HttpGet("get-all-author")]
        public IActionResult GetAll()
        {
            var allAuthors = _authorRepository.GellAllAuthors();
            return Ok(allAuthors);
        }

        [HttpGet]
        [Route("get-author-by-id/{id}")]
        public IActionResult GetAuthorById([FromRoute] int id)
        {
            var AuthorWithIdDTO = _authorRepository.GetAuthorById(id);
            return Ok(AuthorWithIdDTO);
        }
        [HttpPost("add-Author")]
        public IActionResult AddAuthor([FromBody] AddAuthorRequestDTO addauthorRequestDTO)
        {
            var authorAdd = _authorRepository.AddAuthor(addauthorRequestDTO);
            return Ok(authorAdd);
        }

        [HttpPut("update-Author-by-id/{id}")]
        public IActionResult UpdateAuthorById(int id, [FromBody] AuthorNoIdDTO authorNoIdDTO)
        {
            var updateauthor = _authorRepository.UpdateAuthorById(id, authorNoIdDTO);
            return Ok(updateauthor);
        }
        [HttpDelete("delete-author-by-id/{id}")]
        public IActionResult DeleteAuthorById(int id)
        {
            var deleteauthor = _authorRepository.DeleteAuthorById(id);
            return Ok(deleteauthor);
        }
    }
}