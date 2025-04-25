using BibliotecaAPP.Models;
using Newtonsoft.Json;
using System.Text;

namespace BibliotecaAPP.Controllers
{
    public class PrestamoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public PrestamoService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<PrestamoModel>> GetAllPrestamosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Prestamo");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<PrestamoModel>>(content);
        }
        public async Task<PrestamoModel> GetPrestamoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Prestamo/" + id);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PrestamoModel>(content);
        }

        public async Task<PrestamoModel> CreatePrestamoAsync(PrestamoModel prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Prestamo", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task<PrestamoModel> UpdatePrestamoAsync(int id, PrestamoModel prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl + "/Prestamo"}/{id}", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task DeletePrestamoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl + "/Prestamo"}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}