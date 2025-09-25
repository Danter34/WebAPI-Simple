using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CustomActionFilter;
using WebAPI.Data;
using WebAPI.Models.Domain;
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
        [ValidateModel]
        public IActionResult AddAuthor([FromBody] AddAuthorRequestDTO addauthorRequestDTO)
        {
            if (ValidateAddAuthor(addauthorRequestDTO))
            {
                var authorAdd = _authorRepository.AddAuthor(addauthorRequestDTO);
                return Ok(authorAdd);
            }
            else return BadRequest(ModelState);
           
        }

        [HttpPut("update-Author-by-id/{id}")]
        public IActionResult UpdateAuthorById(int id, [FromBody] AuthorNoIdDTO authorNoIdDTO)
        {
            var updateauthor = _authorRepository.UpdateAuthorById(id, authorNoIdDTO);
            return Ok(updateauthor);
        }
        [HttpDelete("delete-author-by-id/{id}")]
        [ServiceFilter(typeof(ValidateAuthorCanDeleteAttribute))]
        public IActionResult DeleteAuthorById(int id)
        {
            if (ValidateDeleteAuthor(id))
            {
                var deleteauthor = _authorRepository.DeleteAuthorById(id);
                return Ok(deleteauthor);
            }
            else return BadRequest(ModelState);
        }
        private bool ValidateAddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            if (string.IsNullOrWhiteSpace(addAuthorRequestDTO.FullName))
            {
                ModelState.AddModelError(nameof(addAuthorRequestDTO.FullName), $"{nameof(addAuthorRequestDTO.FullName)} cannot be empty");
            }
            else if (addAuthorRequestDTO.FullName.Length < 3)
            {
                ModelState.AddModelError(nameof(addAuthorRequestDTO.FullName), $"{nameof(addAuthorRequestDTO.FullName)} must be at least 3 characters long");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
        private bool ValidateDeleteAuthor(int id)
        {
            var author = _dbContext.Authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
            {
                ModelState.AddModelError("AuthorId", $"Author with ID {id} does not exist.");
            }

            // Kiểm tra liên kết qua bảng Book_Authors
            var hasLinks = _dbContext.Books_Authors.Any(ba => ba.AuthorId == id);
            if (hasLinks)
            {
                ModelState.AddModelError("AuthorId", $"Cannot delete Author with ID {id} because it is linked to one or more Books via Book_Authors.");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
    }
}