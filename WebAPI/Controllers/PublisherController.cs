using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Models.Domain;
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
            if (ValidateAddPublisher(addpublisherRequestDTO))
            {
                var PublisherAdd = _publisherRepository.AddPublisher(addpublisherRequestDTO);
                return Ok(PublisherAdd);
            }
            else return BadRequest(ModelState);
           
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
            if (ValidateDeletePublisher(id))
            {
                var deletepublisher = _publisherRepository.DeletePublisherById(id);
                return Ok(deletepublisher);
            }
            else return BadRequest(ModelState);
            
        }
        private bool ValidateAddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            if (!string.IsNullOrWhiteSpace(addPublisherRequestDTO.Name) && _publisherRepository.ExistsByName(addPublisherRequestDTO.Name))
            {
                ModelState.AddModelError(nameof(addPublisherRequestDTO.Name), $"{nameof(addPublisherRequestDTO.Name)} already exists");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
        private bool ValidateDeletePublisher(int id) 
        {
            var publisher = _dbContext.Publishers.FirstOrDefault(p => p.Id == id);
            if (publisher == null)
            {
                ModelState.AddModelError(nameof(id), $"Publisher with ID {id} does not exist.");
            }

            var hasBooks = _dbContext.Books.Any(b => b.PublisherID == id);
            if (hasBooks)
            {
                ModelState.AddModelError(nameof(id), $"Cannot delete Publisher with ID {id} because it has related Books.");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
    }
}
