using BibliotecaAPP.Models;
using System.Net.Http;

namespace BibliotecaAPP.Controllers
{
    public class HomeService
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public HomeService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<bool> ValidarUsuarioAsync(LoginViewModel credenciales)
        {
        // Construir la URL con los parametros de consulta
        var url = $"{_baseUrl}/Usuario/validar?correo={Uri.EscapeDataString(credenciales.Correo)}&clave={Uri.EscapeDataString(credenciales.Clave)}";
        var response = await _httpClient.PostAsync(url, null);

        // Si la respuesta no es exitosa, loguear el mensaje de error
        if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }

            // Retornar si la validacion fue exitosa
            return response.IsSuccessStatusCode;
        }
    }
}


