// Controllers/PrestamoController.cs
using BibliotecaAPP.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using iTextSharp.text.pdf;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BibliotecaAPP.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly PrestamoService _prestamoService;
        private readonly LibroService _libroService;

        public PrestamoController(PrestamoService prestamoService, LibroService libroService)
        {
            _prestamoService = prestamoService;
            _libroService = libroService;
        }

        // GET: /Prestamo
        // Controllers/PrestamoController.cs
        public async Task<IActionResult> Index(string titulo, string autor)
        {
            // 1. Trae todos los préstamos
            var prestamos = (await _prestamoService.GetAllPrestamosAsync()).ToList();

            // 2. Trae todos los usuarios y libros para poblar navegación
            var usuarios = (await _prestamoService.GetAllUsuariosAsync()).ToList();
            var libros = (await _libroService.GetAllLibrosAsync()).ToList();

            // 3. Asocia la navegación
            foreach (var p in prestamos)
            {
                p.Usuario = usuarios.FirstOrDefault(u => u.Id == p.IdUsuario);
                p.Libro = libros.FirstOrDefault(l => l.Id == p.IdLibro);
            }

            // 4. Aplica filtros opcionales
            if (!string.IsNullOrWhiteSpace(titulo))
                prestamos = prestamos
                    .Where(p => p.Libro.Titulo
                        .Contains(titulo, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrWhiteSpace(autor))
                prestamos = prestamos
                    .Where(p => p.Libro.Autor
                        .Contains(autor, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            // Guarda los valores del filtro para reusarlos en la vista
            ViewBag.TituloFilter = titulo;
            ViewBag.AutorFilter = autor;

            return View(prestamos);
        }


        // GET: /Prestamo/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
                return NotFound();

            // Carga navegación
            var usuarios = (await _prestamoService.GetAllUsuariosAsync()).ToList();
            var libros = (await _libroService.GetAllLibrosAsync()).ToList();

            prestamo.Usuario = usuarios.FirstOrDefault(u => u.Id == prestamo.IdUsuario);
            prestamo.Libro = libros.FirstOrDefault(l => l.Id == prestamo.IdLibro);

            return View(prestamo);
        }

        // GET: /Prestamo/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Usuarios = await _prestamoService.GetAllUsuariosAsync();
            ViewBag.Libros = await _libroService.GetAllLibrosAsync();
            return View();
        }

        // POST: /Prestamo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrestamoCreacionModel vm)
        {
            // Dump de errores para debug:
            foreach (var e in ModelState)
                foreach (var err in e.Value.Errors)
                    System.Diagnostics.Debug.WriteLine($"[CREATE ERROR] {e.Key}: {err.ErrorMessage}");

            if (!ModelState.IsValid)
            {
                ViewBag.Usuarios = await _prestamoService.GetAllUsuariosAsync();
                ViewBag.Libros = await _libroService.GetAllLibrosAsync();
                return View(vm);
            }

            // Mapeo al modelo completo
            var prestamo = new PrestamoModel
            {
                IdUsuario = vm.IdUsuario,
                IdLibro = vm.IdLibro,
                FechaPrestamo = vm.FechaPrestamo,
                FechaDevolucionEsperada = vm.FechaDevolucionEsperada,
                Estado = vm.Estado
            };

            await _prestamoService.CreatePrestamoAsync(prestamo);
            TempData["Success"] = "Préstamo creado con éxito";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Prestamo/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null) return NotFound();

            // Mapea sólo los campos que vamos a editar
            var vm = new PrestamoActualizacionModel
            {
                Id = prestamo.Id,
                FechaDevolucionReal = prestamo.FechaDevolucionReal,
                Estado = prestamo.Estado
            };

            // Guardamos el original para mostrar campos de sólo lectura en la vista
            ViewBag.PrestamoOriginal = prestamo;
            return View(vm);
        }

        // POST: /Prestamo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PrestamoActualizacionModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Si hay error, recargamos el original y devolvemos la misma vista
                ViewBag.PrestamoOriginal = await _prestamoService.GetPrestamoByIdAsync(vm.Id);
                return View(vm);
            }

            // Creamos el modelo completo que espera el servicio
            var toUpdate = new PrestamoModel
            {
                Id = vm.Id,
                FechaDevolucionReal = vm.FechaDevolucionReal,
                Estado = vm.Estado
            };

            var success = await _prestamoService.UpdatePrestamoAsync(vm.Id, toUpdate);
            if (!success)
            {
                ModelState.AddModelError("", "No se pudo guardar en la API.");
                ViewBag.PrestamoOriginal = await _prestamoService.GetPrestamoByIdAsync(vm.Id);
                return View(vm);
            }

            TempData["Success"] = "Préstamo actualizado con éxito";
            return RedirectToAction(nameof(Index));
        }



        // GET: /Prestamo/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        // POST: /Prestamo/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prestamoService.DeletePrestamoAsync(id);
            TempData["Success"] = "Préstamo eliminado con éxito";
            return RedirectToAction(nameof(Index));
        }

        // Exportaciones 
        public async Task<IActionResult> ExportToPdf()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            using var stream = new MemoryStream();
            var doc = new iTextSharp.text.Document();
            PdfWriter.GetInstance(doc, stream).CloseStream = false;
            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph("Lista de Préstamos"));
            doc.Add(new iTextSharp.text.Paragraph(" "));
            var table = new PdfPTable(6);
            table.AddCell("Usuario");
            table.AddCell("Libro");
            table.AddCell("Fecha Préstamo");
            table.AddCell("Fecha Devolución Esperada");
            table.AddCell("Fecha Devolución Real");
            table.AddCell("Estado");
            foreach (var p in prestamos)
            {
                table.AddCell(p.Usuario?.Nombre ?? p.IdUsuario.ToString());
                table.AddCell(p.Libro?.Titulo ?? p.IdLibro.ToString());
                table.AddCell(p.FechaPrestamo.ToString("yyyy-MM-dd"));
                table.AddCell(p.FechaDevolucionEsperada.ToString("yyyy-MM-dd"));
                table.AddCell(p.FechaDevolucionReal?.ToString("yyyy-MM-dd") ?? "-");
                table.AddCell(p.Estado);
            }
            doc.Add(table);
            doc.Close();
            stream.Position = 0;
            return File(stream.ToArray(), "application/pdf", "Prestamos.pdf");
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Prestamos");
            worksheet.Cells[1, 1].Value = "Usuario";
            worksheet.Cells[1, 2].Value = "Libro";
            worksheet.Cells[1, 3].Value = "Fecha Préstamo";
            worksheet.Cells[1, 4].Value = "Fecha Devolución Esperada";
            worksheet.Cells[1, 5].Value = "Fecha Devolución Real";
            worksheet.Cells[1, 6].Value = "Estado";
            int row = 2;
            foreach (var p in prestamos)
            {
                worksheet.Cells[row, 1].Value = p.Usuario?.Nombre ?? p.IdUsuario.ToString();
                worksheet.Cells[row, 2].Value = p.Libro?.Titulo ?? p.IdLibro.ToString();
                worksheet.Cells[row, 3].Value = p.FechaPrestamo.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 4].Value = p.FechaDevolucionEsperada.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 5].Value = p.FechaDevolucionReal?.ToString("yyyy-MM-dd") ?? "-";
                worksheet.Cells[row, 6].Value = p.Estado;
                row++;
            }
            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Prestamos.xlsx");
        }
    }
}