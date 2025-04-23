namespace BibliotecaAPP.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Tipo_usuario { get; set; }
        public string Clave { get; set; }
    }
}
