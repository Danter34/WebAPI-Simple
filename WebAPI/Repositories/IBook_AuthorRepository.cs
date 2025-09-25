using WebAPI.Models.DTO;

namespace WebAPI.Repositories
{
    public interface IBook_AuthorRepository
    {
        AddBook_AuthorRequestDTO AddBook_Author(AddBook_AuthorRequestDTO addBook_AuthorRequestDTO);
        public bool ExistsByBookId(int bookId);
        public bool ExistsByAuthorId(int authorId);
        bool Exists(int bookId, int authorId);
    }
}
