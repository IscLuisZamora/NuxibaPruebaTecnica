using Microsoft.AspNetCore.Mvc;
using AppTestNuxiba.Data;
using AppTestNuxiba.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;
using System.Data;
using System.Text;
using ClosedXML.Excel;

namespace AppTestNuxiba.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDBContext _appDbContext;
        public AccesoController(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> ContenidoTabla() 
        {
            List<Usuario> lista = await _appDbContext.GetAllUsuariosAsync();
            return View(lista);
        }

        [HttpGet]
        public IActionResult NuevoUsuario()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevoUsuarioInsert(Usuario usuario)
        {
            usuario.FechaIngreso = DateTime.Now;
            await _appDbContext.InsertAllData(usuario);
            return RedirectToAction(nameof(ContenidoTabla));
        }

        [HttpGet]
        public async Task<IActionResult> EditarGetUsuario(int id)
        {
            Usuario usuario = await _appDbContext.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsuarioInsert(Usuario usuario, double sueldo)
        {
            await _appDbContext.EditSueldoAsync(usuario.UserId, sueldo);
            return RedirectToAction(nameof(ContenidoTabla));
        }

        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var usuarios = await _appDbContext.GetAllUsuariosAsync();
            var dataTable = new DataTable("Usuarios");

            dataTable.Columns.AddRange(new DataColumn[4] {
            new DataColumn("Login", typeof(string)),
            new DataColumn("Nombre Completo", typeof(string)),
            new DataColumn("Sueldo", typeof(double)),
            new DataColumn("FechaIngreso", typeof(DateTime))
        });

            foreach (var usuario in usuarios)
            {
                dataTable.Rows.Add(
                    usuario.Login,
                    usuario.Nombre + " " + usuario.Paterno + " " + usuario.Materno,
                    usuario.Sueldo,
                    usuario.FechaIngreso
                );
            }

            var csvContent = DataTableToCSV(dataTable);
            var byteArray = Encoding.UTF8.GetBytes(csvContent);
            var stream = new MemoryStream(byteArray);

            var nombreCSV = string.Concat("Reporte_Datos_", DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".csv");
            return File(stream, "text/csv", nombreCSV);
        }

        private string DataTableToCSV(DataTable dataTable)
        {
            var builder = new StringBuilder();

            // header
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                builder.Append(dataTable.Columns[i].ColumnName);
                if (i < dataTable.Columns.Count - 1)
                    builder.Append(",");
            }
            builder.AppendLine();

            // rows
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    builder.Append(row[i].ToString());
                    if (i < dataTable.Columns.Count - 1)
                        builder.Append(",");
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }

    }
}
