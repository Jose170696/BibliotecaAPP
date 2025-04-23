using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPP.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string Correo { get; set; }
        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Clave { get; set; }

    }
}
