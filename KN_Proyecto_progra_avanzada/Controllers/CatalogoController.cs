using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using KN_Proyecto_progra_avanzada.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace KN_Proyecto_progra_avanzada.Controllers
{
    [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true, VaryByParam = "*")]
    [Seguridad]
    public class CatalogoController : Controller
    {
        // ---------------------------------------------------------
        // VER CATALOGO
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult VerCatalogo()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbCatalogo
                                       .Include(c => c.tbCategoria)
                                       .ToList()
                                       .Select(c => new Catalogo
                                       {
                                           IdProducto = c.IdProducto,
                                           Nombre = c.Nombre,
                                           Descripcion = c.Descripcion,
                                           Precio = c.Precio,
                                           Stock = c.Stock,
                                           Imagen = c.Imagen,
                                           Estado = c.Estado,
                                           CategoriaNombre = c.tbCategoria != null
                                                               ? c.tbCategoria.Nombre
                                                               : "Sin categoría"
                                       })
                                       .ToList();

                return View(resultado);
            }
        }


        [HttpGet]
        public ActionResult AgregarCatalogo()
        {
            CargarCategorias();                 // 🔹 Llenamos ViewBag.Categorias
            return View(new Catalogo());        // 🔹 Mandamos un modelo vacío
        }

        [HttpPost]
        public ActionResult AgregarCatalogo(Catalogo catalogo)
        {
            if (!ModelState.IsValid)
            {
                CargarCategorias();       // 🔹 Volvemos a poblar el combo
                return View(catalogo);
            }

            // Aquí luego vas a guardar en la BD
            // ...

            return RedirectToAction("VerCatalogo");
        }


        private void CargarCategorias()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var categorias = context.tbCategoria
                    .Where(cat => cat.Estado == true)  // mejor explícito por si es nullable
                    .Select(cat => new
                    {
                        cat.IdCategoria,
                        cat.Nombre
                    })
                    .ToList();

                ViewBag.Categorias = new SelectList(
                    categorias,
                    "IdCategoria",
                    "Nombre"
                );
            }
        }


    }
}
