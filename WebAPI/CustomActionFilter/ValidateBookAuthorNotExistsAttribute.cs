using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.Models.DTO;
using WebAPI.Repositories;

namespace WebAPI.CustomActionFilter
{
    public class ValidateBookAuthorNotExistsAttribute:ActionFilterAttribute
    {
        private readonly IBook_AuthorRepository _repository;

        public ValidateBookAuthorNotExistsAttribute(IBook_AuthorRepository repository)
        {
            _repository = repository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("addBook_AuthorRequestDTO", out var value) && value is AddBook_AuthorRequestDTO dto)
            {
                if (_repository.Exists(dto.BookId, dto.AuthorId))
                {
                    context.Result = new ConflictObjectResult(new
                    {
                        message = $"The relationship BookID={dto.BookId} and AuthorID={dto.AuthorId} already exists."
                    });
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
