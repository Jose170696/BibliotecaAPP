using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPP.Models
{
    public class PrestamoModel
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdLibro { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string Estado { get; set; }

        //Navigation properties
        public virtual UsuarioModel Usuario { get; set; }
        public virtual LibroModel Libro { get; set; }
    }
    public class PrestamoActualizacionModel
    {
        [Required]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Devolución Real")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaDevolucionReal { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public string Estado { get; set; }
    }

    public class PrestamoCreacionModel
    {
        [Required(ErrorMessage = "Debe seleccionar un usuario")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un libro")]
        public int IdLibro { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Préstamo")]
        public DateTime FechaPrestamo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Devolución Esperada")]
        public DateTime FechaDevolucionEsperada { get; set; }

        [Required(ErrorMessage = "Debe elegir un estado")]
        public string Estado { get; set; }
    }
}
