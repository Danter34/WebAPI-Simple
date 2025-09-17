using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;
namespace WebAPI.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<AuthorDTO> GellAllAuthors()
        {
            var allAuthors = _dbContext.Authors.Select(author => new AuthorDTO
            {
                Id = author.Id,
                FullName = author.FullName
            }).ToList();
            return allAuthors;
        }
        public AuthorNoIdDTO GetAuthorById(int id)
        {
            var AuthorWithDomain = _dbContext.Authors.Where(n => n.Id == id);
            var AuthorWithIdDTO = AuthorWithDomain.Select(author => new AuthorNoIdDTO
            {
                FullName = author.FullName
            }).FirstOrDefault();
            return AuthorWithIdDTO;
        }
        public AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var AuthorDomain = new Authors
            {
                FullName = addAuthorRequestDTO.FullName
            };
            _dbContext.Authors.Add(AuthorDomain);
            _dbContext.SaveChanges();
            return addAuthorRequestDTO;
        }
        public AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var AuthorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (AuthorDomain != null)
            {
                AuthorDomain.FullName = authorNoIdDTO.FullName;
                _dbContext.SaveChanges();
            }
            return authorNoIdDTO;
        }
        public Authors? DeleteAuthorById(int id)
        {
            var AuthorDoamin = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (AuthorDoamin != null)
            {
                _dbContext.Authors.Remove(AuthorDoamin);
                _dbContext.SaveChanges();
            }
            return null;
        }

    }
}
