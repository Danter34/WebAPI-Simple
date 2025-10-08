using Microsoft.AspNetCore.Mvc;
using System.Text;
using Library_web.Models.DTO;
using Newtonsoft.Json;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
namespace Library_web.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }    
                

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Gọi API login
            var response = await client.PostAsync("https://localhost:7021/api/User/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LoginResponseDTO>(json);

                // Lưu token vào session
                HttpContext.Session.SetString("JwtToken", result.JwtToken);

                //giai ma de lay role
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(result.JwtToken);

                var roles = jwt.Claims
                   .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                   .Select(c => c.Value)
                   .ToList();

                HttpContext.Session.SetString("UserRoles", string.Join(",", roles));
                return RedirectToAction("Index", "Books");
            }

            ViewBag.Error = "Username hoặc password không đúng!";
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login");
        }
    }
}
