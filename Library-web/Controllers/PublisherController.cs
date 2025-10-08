using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Library_web.Controllers
{
    public class PublisherController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public PublisherController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            List<publisherDTO> response = new List<publisherDTO>();
            try
            {
                // Lấy dữ liệu books from API
                var client = httpClientFactory.CreateClient("APIClient");
                var httpResponseMess = await client.GetAsync("https://localhost:7021/api/Publisher/get-all-publisher");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());
            }
            catch (Exception ex)
            {
                return View("Error");
            }
            return View(response);
        }
        [HttpGet]
        public IActionResult addPublisher()
        {
            return View(new publisherNoIdDTO());
        }
        [HttpPost]
        public async Task<IActionResult> addPublisher(publisherNoIdDTO model)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            try
            {
                var req = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7021/api/Publisher/add-Publisher"),
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                var res = await client.SendAsync(req);
                if (res.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, await res.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditPublisher(int id)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            try
            {
                var res = await client.GetAsync($"https://localhost:7021/api/Publisher/get-publisher-by-id/{id}");
                if (!res.IsSuccessStatusCode) return NotFound();
                var model = await res.Content.ReadFromJsonAsync<publisherDTO>();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditPublisher(int id, publisherDTO model)
        {
            if (id != model.Id) model.Id = id;

            var client = httpClientFactory.CreateClient("APIClient");
            try
            {
                var req = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"https://localhost:7021/api/Publisher/update-publisher-by-id/{model.Id}"),
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                var res = await client.SendAsync(req);
                if (res.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, await res.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }
        public async Task<IActionResult> delPublisher([FromRoute] int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient("APIClient");
                var httpResponseMess = await client.DeleteAsync("https://localhost:7021/api/Publisher/delete-Publisher-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Publisher");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View("Index");
        }
    }
}
