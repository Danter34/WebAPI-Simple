using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.CustomActionFilter;
using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;
using WebAPI.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly IPublisherRepository _publisherRepository;
        private const int MAX_BOOKS_PER_AUTHOR = 20;
        private const int MAX_BOOKS_PER_PUBLISHER_PER_YEAR = 100;

        public BooksController(AppDbContext dbContext, IBookRepository bookRepository, IPublisherRepository publisherRepository)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
            _publisherRepository = publisherRepository;
        }

        [HttpGet("get-all-books")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            // su dung reposity pattern  
            var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery, sortBy,isAscending, pageNumber, pageSize);
            return Ok(allBooks);
        }

        [HttpGet]
        [Route("get-book-by-id/{id}")]
        [Authorize(Roles ="Read")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            return Ok(bookWithIdDTO);
        }
        [HttpPost("add-book")]
        [Authorize(Roles ="Read,Write")]
        [ValidateModel]
        [ServiceFilter(typeof(ValidatePublisherExistsAttribute))]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            if (ValidateAddBook(addBookRequestDTO))
            {
                var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
                return Ok(bookAdd);
            }
            else return BadRequest(ModelState);
        }

        [HttpPut("update-book-by-id/{id}")]
        [Authorize(Roles ="Read,Write")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            try
            {
                var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
                return Ok(updateBook);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("delete-book-by-id/{id}")]
        [Authorize(Roles ="Read,Write")]
        public IActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            return Ok(deleteBook);
        }
        private bool ValidateAddBook(AddBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO), $"Please add book data"); 
                return false;
            }
            // Kiểm tra Book.Title không được rỗng, không chứa ký tự đặc biệt.
            if (string.IsNullOrWhiteSpace(addBookRequestDTO.Title))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Title), $"{nameof(addBookRequestDTO.Title)} cannot be empty");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(addBookRequestDTO.Title, @"^[a-zA-Z0-9\s]+$"))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Title), $"{nameof(addBookRequestDTO.Title)} cannot contain special characters");
            }
            // kiem tra Description NotNull 
            if (string.IsNullOrEmpty(addBookRequestDTO.Description))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Description),$"{nameof(addBookRequestDTO.Description)} cannot be null");
            }
            // kiem tra rating (0,5) 
            if (addBookRequestDTO.Rate < 0 || addBookRequestDTO.Rate > 5)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Rate),$"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5");
            }
            //Kiểm tra PublisherID trong bảng Books phải tồn tại trong bảng Publishers. 
            if (!_publisherRepository.ExistsById(addBookRequestDTO.PublisherID))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherID), $"{nameof(addBookRequestDTO.PublisherID)} does not exist in Publishers table");
            }
            //Kiểm tra duplicate AuthorIds
            if (addBookRequestDTO.AuthorIds != null && addBookRequestDTO.AuthorIds.Count > 0)
            {
                var duplicates = addBookRequestDTO.AuthorIds
                    .GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicates.Any())
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"AuthorIds contains duplicate values: {string.Join(",", duplicates)}");
                }
            }
            // Check AuthorIds phải có ít nhất 1
            if (addBookRequestDTO.AuthorIds == null || !addBookRequestDTO.AuthorIds.Any())
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), "A book must have at least one author.");
            }
            else
            {
                // Check AuthorIds tồn tại
                foreach (var authorId in addBookRequestDTO.AuthorIds)
                {
                    if (!_dbContext.Authors.Any(a => a.Id == authorId))
                    {
                        ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"Author with ID {authorId} does not exist.");
                    }
                }
            }
            //check số lượng tối đa một tác giả viết sách
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                int currentCount = _dbContext.Books_Authors.Count(ba => ba.AuthorId == authorId);

                if (currentCount >= MAX_BOOKS_PER_AUTHOR)
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds),
                        $"Author with ID {authorId} already has {currentCount} books. " +
                        $"Maximum allowed is {MAX_BOOKS_PER_AUTHOR}.");
                }
            }
            //check số lượng tối đa một năm nhà xuất bản có thể xuất
            int year = addBookRequestDTO.DateAdded.Year;
            int publishedCount = _dbContext.Books.Count(b =>
                b.PublisherID == addBookRequestDTO.PublisherID &&
                b.DateAdded.Year == year);

            if (publishedCount >= MAX_BOOKS_PER_PUBLISHER_PER_YEAR)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherID),
                    $"Publisher with ID {addBookRequestDTO.PublisherID} already published {publishedCount} books in {year}. " +
                    $"Maximum allowed is {MAX_BOOKS_PER_PUBLISHER_PER_YEAR}.");
            }
            //Title không trùng trong cùng 1 Publisher
            bool duplicateTitle = _dbContext.Books.Any(b => b.PublisherID == addBookRequestDTO.PublisherID && b.Title.ToLower().Trim() == addBookRequestDTO.Title.ToLower().Trim());
            if (duplicateTitle)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Title),
                    $"The title '{addBookRequestDTO.Title}' already exists for this Publisher");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
    }
}