using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;

namespace WebAPI.Repositories
{
    public class SQLBook_Author:IBook_AuthorRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLBook_Author(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AddBook_AuthorRequestDTO AddBook_Author(AddBook_AuthorRequestDTO addBook_AuthorRequestDTO)
        {
            var bookAuthorDomain = new Book_Authors
            {
                BookId = addBook_AuthorRequestDTO.BookId,
                AuthorId = addBook_AuthorRequestDTO.AuthorId,
            };
            _dbContext.Books_Authors.Add(bookAuthorDomain);
            _dbContext.SaveChanges();
            return addBook_AuthorRequestDTO;
        }
        public bool ExistsByBookId(int bookId)
        {
            return _dbContext.Books.Any(b => b.Id == bookId);
        }
        public bool ExistsByAuthorId(int authorId)
        {
            return _dbContext.Authors.Any(a => a.Id == authorId);
        }

    }
}
