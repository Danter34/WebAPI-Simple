using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Models.DTO;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublisherRepository _publisherRepository;

        public PublisherController(AppDbContext dbContext, IPublisherRepository publisherRepository)
        {
            _dbContext = dbContext;
            _publisherRepository = publisherRepository;
        }

        [HttpGet("get-all-Publisher")]
        public IActionResult GetAll()
        {
            var allPublishers = _publisherRepository.GetAllPublishers();
            return Ok(allPublishers);
        }

        [HttpGet]
        [Route("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById([FromRoute] int id)
        {
            var PublisherWithIdDTO = _publisherRepository.GetPublisherById(id);
            return Ok(PublisherWithIdDTO);
        }
        [HttpPost("add-Publisher")]
        public IActionResult AddPublisher([FromBody] AddPublisherRequestDTO addpublisherRequestDTO)
        {
            var PublisherAdd = _publisherRepository.AddPublisher(addpublisherRequestDTO);
            return Ok(PublisherAdd);
        }

        [HttpPut("update-Publisher-by-id/{id}")]
        public IActionResult UpdatePublisherById(int id, [FromBody] PublisherNoIdDTO publisherNoIdDTO)
        {
            var updatepublisher = _publisherRepository.UpdatePublisherById(id, publisherNoIdDTO);
            return Ok(updatepublisher);
        }
        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            var deletepublisher = _publisherRepository.DeletePublisherById(id);
            return Ok(deletepublisher);
        }
    }
}
