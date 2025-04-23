using System.Diagnostics;
using BibliotecaAPP.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeService _usuarioService;

        public HomeController(ILogger<HomeController> logger, HomeService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            //return View();
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View();
            }

            bool esValido = await _usuarioService.ValidarUsuarioAsync(loginViewModel);
            if (esValido)
            {
                // Aqu� puedes redirigir a la p�gina de inicio o a otra p�gina despu�s de un inicio de sesi�n exitoso
                return RedirectToAction("Index", "Libro");
            }
                ModelState.AddModelError(string.Empty, "Credenciales inv�lidas.");
                return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
