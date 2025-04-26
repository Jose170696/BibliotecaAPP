// Services/PrestamoService.cs
using BibliotecaAPP.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            var response = await _httpClient.GetAsync($"{_baseUrl}/Prestamo/{id}");
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

        public async Task<bool> UpdatePrestamoAsync(int id, PrestamoModel prestamo)
        {
            try
            {
                // Formatear la fecha exactamente como en el ejemplo exitoso
                string fechaFormateada = prestamo.FechaDevolucionReal?.ToString("yyyy-MM-ddTHH:mm:ss");

                // Crear el payload JSON exactamente como en el ejemplo exitoso
                var jsonPayload = $@"{{
            ""id"": {id},
            ""fechaDevolucionReal"": ""{fechaFormateada}"",
            ""estado"": ""{prestamo.Estado}""
        }}";

                System.Diagnostics.Debug.WriteLine($"[PUT] Payload: {jsonPayload}");

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var url = $"{_baseUrl}/Prestamo/{id}";
                System.Diagnostics.Debug.WriteLine($"[PUT] URL: {url}");

                var response = await _httpClient.PutAsync(url, content);

                System.Diagnostics.Debug.WriteLine($"[PUT] StatusCode: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[PUT] Response Body: {body}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PUT ERROR] {ex.Message}");
                return false;
            }
        }


        public async Task DeletePrestamoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Prestamo/{id}");
            response.EnsureSuccessStatusCode();
        }


        // Método para obtener usuarios
        public async Task<IEnumerable<UsuarioModel>> GetAllUsuariosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Usuario");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<UsuarioModel>>(json);
        }
    }
}