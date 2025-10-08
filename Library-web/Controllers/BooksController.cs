using Microsoft.AspNetCore.Mvc;
using Library_web.Models.DTO;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Text;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Library_web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public BooksController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index([FromQuery] string filterOn = null, string filterQuery = null, string sortBy = null, bool isAscending = true)
        {
            List<BookDTO> response = new List<BookDTO>();
            try
            {
                // Lấy dữ liệu books from API
                var client = httpClientFactory.CreateClient("APIClient");
                var httpResponseMess = await client.GetAsync($"https://localhost:7021/api/Books/get-all-books?filterOn={filterOn}&filterQuery={filterQuery}&sortBy={sortBy}&isAscending={isAscending}");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<BookDTO>>());
            }
            catch (Exception ex)
            {
                return View("Error");
            }
            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> addBook()
        {
            await LoadLookupsAsync();                     
            return View(new addBookDTO());               
        }

        [HttpPost]
        public async Task<IActionResult> addBook(addBookDTO model)
        {
            var client = httpClientFactory.CreateClient("APIClient");

            try
            {
                var req = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7021/api/Books/add-book"),
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                var resp = await client.SendAsync(req);
                if (resp.IsSuccessStatusCode)
                    return RedirectToAction("Index", "Books");

               
                ModelState.AddModelError(string.Empty, await resp.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

          
            await LoadLookupsAsync();
            return View(model);
        }
        private async Task LoadLookupsAsync()
        {
            var client = httpClientFactory.CreateClient("APIClient");

            var authors = await client.GetFromJsonAsync<IEnumerable<authorDTO>>(
                "https://localhost:7021/api/Author/get-all-author") ?? Enumerable.Empty<authorDTO>();

            var publishers = await client.GetFromJsonAsync<IEnumerable<publisherDTO>>(
                "https://localhost:7021/api/Publisher/get-all-publisher") ?? Enumerable.Empty<publisherDTO>();

            ViewBag.Authors = authors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList();
            ViewBag.Publishers = publishers.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
        }
        public async Task<IActionResult> ListBook(int id)
        {
            BookDTO response = new();
            try
            {
                var client = httpClientFactory.CreateClient("APIClient");
                var httpResponseMess = await client.GetAsync($"https://localhost:7021/api/Books/get-book-by-id/{id}");
                httpResponseMess.EnsureSuccessStatusCode();

                response = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>() ?? new BookDTO();
                response.AuthorNames ??= new List<string>();
                response.PublisherName ??= string.Empty;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> editBook(int id)
        {
            var client = httpClientFactory.CreateClient("APIClient");

            // 1) Lấy chi tiết sách
            var r1 = await client.GetAsync($"https://localhost:7021/api/Books/get-book-by-id/{id}");
            r1.EnsureSuccessStatusCode();
            var book = await r1.Content.ReadFromJsonAsync<BookDTO>();

            if (book == null) return NotFound();

            // 2) Map sang editBookDTO để bind ra form
            var model = new editBookDTO
            {
                Id = book.Id,
                title = book.Title ?? string.Empty,
                Description = book.Description,
                IsRead = book.IsRead ?? false,
                DateRead = book.DateRead,
                Rate = book.Rate ?? 0,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = book.DateAdded,
                // PublisherID & AuthorIds sẽ chọn lại trong form
            };

            
            await LoadLookupsAsync(client);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> editBook([FromRoute] int id, editBookDTO model)
        {
            if (id != model.Id) model.Id = id;

            var client = httpClientFactory.CreateClient("APIClient");

            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7021/api/Books/update-book-by-id/{model.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            var resp = await client.SendAsync(req);
            if (resp.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Books");
            }

           
            ViewBag.Error = await resp.Content.ReadAsStringAsync();
            await LoadLookupsAsync(client);
            return View(model);
        }

        private async Task LoadLookupsAsync(HttpClient client)
        {
            // authors
            var au = await client.GetFromJsonAsync<IEnumerable<authorDTO>>(
                "https://localhost:7021/api/Author/get-all-author");
            ViewBag.ListAuthor = au?.ToList() ?? new List<authorDTO>();

            // publishers
            var pu = await client.GetFromJsonAsync<IEnumerable<publisherDTO>>(
                "https://localhost:7021/api/Publisher/get-all-publisher");
            ViewBag.ListPublisher = pu?.ToList() ?? new List<publisherDTO>();
        }
        [HttpGet]
        public async Task<IActionResult> delBook([FromRoute] int id)
        {
            try
            {
                // lấy dữ liệu books from API
                var client = httpClientFactory.CreateClient("APIClient");
                var httpResponseMess = await client.DeleteAsync("https://localhost:7021/api/Books/delete-book-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View("Index");
        }

    }
}
