using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using System.Linq;
using System.Web.Mvc;

public class TiendaController : Controller
{
    [HttpGet]
    public ActionResult VerTienda(int? idCategoria, string q)
    {
        using (var context = new BDProyecto_KNEntities())
        {
            var query = context.tbCatalogo.Where(p => p.Estado == true && p.Stock > 0);

            if (idCategoria.HasValue)
                query = query.Where(p => p.IdCategoria == idCategoria.Value);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));

            var productos = query
                .OrderBy(p => p.Nombre)
                .Select(p => new Catalogo
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Imagen = p.Imagen,
                    IdCategoria = p.IdCategoria
                })
                .ToList();

            // 👇 Esto es lo que tu vista ocupa para el combo
            var categorias = context.tbCategoria
                .Where(c => c.Estado == true)
                .ToList();

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre", idCategoria);

            return View(productos);
        }
    }
}
