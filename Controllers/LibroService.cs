using BibliotecaAPP.Models;
using Newtonsoft.Json;
using System.Text;
namespace BibliotecaAPP.Controllers
{
    public class LibroService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public LibroService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<LibroModel>> GetAllLibrosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Libro");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<LibroModel>>(content);
        }

        public async Task<LibroModel> GetLibroByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Libro/" + id);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LibroModel>(content);
        }

        public async Task<LibroModel> CreateLibroAsync(LibroModel libro)
        {
            var content = new StringContent(JsonConvert.SerializeObject(libro), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Libro", content);
            response.EnsureSuccessStatusCode();
            return libro;
        }

        public async Task<LibroModel> UpdateLibroAsync(int id, LibroModel libro)
        {
            var content = new StringContent(JsonConvert.SerializeObject(libro), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl + "/Libro"}/{id}", content);
            response.EnsureSuccessStatusCode();
            return libro;
        }

        public async Task DeleteLibroAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl + "/Libro"}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
