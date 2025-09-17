using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;

namespace WebAPI.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<PublisherDTO> GetAllPublishers()
        {
            var allPublisher = _dbContext.Publishers.Select(publisher => new PublisherDTO
            {
                Id = publisher.Id,
                Name = publisher.Name
            }).ToList();
            return allPublisher;
        }
        public PublisherNoIdDTO GetPublisherById(int id)
        {
            var PublisherWithDomain = _dbContext.Publishers.Where(n => n.Id == id);
            var PublisherWithIdDTO = PublisherWithDomain.Select(author => new PublisherNoIdDTO
            {
                Name = author.Name,
            }).FirstOrDefault();
            return PublisherWithIdDTO;
        }
        public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var PublisherDomain = new Publishers
            {
                Name = addPublisherRequestDTO.Name
            };
            _dbContext.Publishers.Add(PublisherDomain);
            _dbContext.SaveChanges();
            return addPublisherRequestDTO;
        }
        public PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var PublisherDoamin = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if(PublisherDoamin != null)
            {
                PublisherDoamin.Name = publisherNoIdDTO.Name;
                _dbContext.SaveChanges();
            }
            return publisherNoIdDTO;
        }
        public Publishers? DeletePublisherById(int id)
        {
            var PublisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if(PublisherDomain != null)
            {
                _dbContext.Publishers.Remove(PublisherDomain);
                _dbContext.SaveChanges();
            }
            return null;
        }
    }
}
