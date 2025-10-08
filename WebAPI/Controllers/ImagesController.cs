using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }
        //Phương thức Upload trong Controller
        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload([FromForm] ImageUploadRequestDTO request)
        {
            ValidateFileUpload(request);

            if (ModelState.IsValid)
            {
                // convert DTO to Domain model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.File.FileName,
                    FileDescription = request.FileDescription
                };

                // Use repository to upload image
                _imageRepository.Upload(imageDomainModel);

                return Ok(imageDomainModel);
            }

            return BadRequest(ModelState);
        }
        //hàm kiểm tra kích thước
        private void ValidateFileUpload(ImageUploadRequestDTO request)
        {
            var allowExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            if (request.File.Length > 10 * 1024 * 1024) // 10MB
            {
                ModelState.AddModelError("file", "File size too big, please upload a file smaller than 10MB");
            }
        }
        //GetInfoAllImages
        [HttpGet]
        public IActionResult GetAllInfoImages()
        {
            var allImages = _imageRepository.GetAllInfoImages();
            return Ok(allImages);
        }
        //DownloadImage
        [HttpGet]
        [Route("Download")]
        public IActionResult DownloadImage(int id)
        {
            var result = _imageRepository.DownloadFile(id);
            return File(result.Item1, result.Item2, result.Item3);
        }

    }
}
