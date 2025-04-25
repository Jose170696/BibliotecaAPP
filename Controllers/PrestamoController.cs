using BibliotecaAPP.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BibliotecaAPP.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly PrestamoService _prestamoService;
        public PrestamoController(PrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        //GET: /Prestamo
        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            return View(prestamos);
        }
        //GET: /Prestamo/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        //GET: /Prestamo/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: /Prestamo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrestamoModel prestamo)
        {
            if (ModelState.IsValid)
            {
                await _prestamoService.CreatePrestamoAsync(prestamo);
                return RedirectToAction(nameof(Index));
            }
            return View(prestamo);
        }

        //GET: /Prestamo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        //POST: /Prestamo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrestamoModel prestamo)
        {
            if (id != prestamo.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                await _prestamoService.UpdatePrestamoAsync(id, prestamo);
                return RedirectToAction(nameof(Index));
            }
            return View(prestamo);
        }

        //GET: /Prestamo/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        //POST: /Prestamo/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prestamoService.DeletePrestamoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();

            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("Lista de Prestamos"));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var table = new PdfPTable(6);
                table.AddCell("Id Ususario");
                table.AddCell("Id Libro");
                table.AddCell("Fecha Prestamo");
                table.AddCell("Fecha Devolucion Esperada");
                table.AddCell("Fecha Devolucion Real");
                table.AddCell("Estado");

                foreach (var prestamo in prestamos)
                {
                    table.AddCell(prestamo.IdUsuario.ToString());
                    table.AddCell(prestamo.IdLibro.ToString());
                    table.AddCell(prestamo.FechaPrestamo.ToString());
                    table.AddCell(prestamo.FechaDevolucionEsperada.ToString());
                    table.AddCell(prestamo.FechaDevolucionReal.ToString());
                    table.AddCell(prestamo.Estado);
                }

                doc.Add(table);
                doc.Close();

                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", "Prestamos.pdf");
            }
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Prestamos");
                //Encabezados
                worksheet.Cells[1, 1].Value = "Id Usuario";
                worksheet.Cells[1, 2].Value = "Id Libro";
                worksheet.Cells[1, 3].Value = "Fecha Prestamo";
                worksheet.Cells[1, 4].Value = "Fecha Devolucion Esperada";
                worksheet.Cells[1, 5].Value = "Fecha Devolucion Real";
                worksheet.Cells[1, 6].Value = "Estado";

                //Datos
                int row = 2;
                foreach (var prestamo in prestamos)
                {
                    worksheet.Cells[row, 5].Value = prestamo.IdUsuario.ToString();
                    worksheet.Cells[row, 5].Value = prestamo.IdLibro.ToString();
                    worksheet.Cells[row, 5].Value = prestamo.FechaPrestamo.ToString();
                    worksheet.Cells[row, 5].Value = prestamo.FechaDevolucionEsperada.ToString();
                    worksheet.Cells[row, 5].Value = prestamo.FechaDevolucionReal.ToString();
                    worksheet.Cells[row, 6].Value = prestamo.Estado;
                    row++;
                }
                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Prestamos.xlsx");
            }
        }
    }
}
