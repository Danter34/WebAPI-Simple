using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Net.Mime;
using System.Text.Json;
using System.Text;

namespace Library_web.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public AuthorController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            List<authorDTO> response = new List<authorDTO>();
            try
            {
                // Lấy dữ liệu books from API
                var client = httpClientFactory.CreateClient("APIClient");
                var httpResponseMess = await client.GetAsync("https://localhost:7021/api/Author/get-all-author");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());
            }
            catch (Exception ex)
            {
                return View("Error");
            }
            return View(response);
        }
        [HttpGet]
        public IActionResult addAuthor()
        {
            return View(new addAuthorDTO()); 
        }
        [HttpPost]
        public async Task<IActionResult> addAuthor(addAuthorDTO model)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            try
            {
                var req = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7021/api/Author/add-Author"),
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
        public async Task<IActionResult> EditAuthor(int id)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            try
            {
                var res = await client.GetAsync($"https://localhost:7021/api/Author/get-author-by-id/{id}");
                if (!res.IsSuccessStatusCode) return NotFound();
                var model = await res.Content.ReadFromJsonAsync<authorDTO>();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAuthor(int id, authorDTO model)
        {
            if (id != model.Id) model.Id = id;

            var client = httpClientFactory.CreateClient("APIClient");
            try
            {
                var req = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"https://localhost:7021/api/Author/update-author-by-id/{model.Id}"),
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
        public async Task<IActionResult> delAuthor([FromRoute] int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient("APIClient");
                var httpResponseMess = await client.DeleteAsync("https://localhost:7021/api/Author/delete-author-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Author");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View("Index");
        }

    }
}
