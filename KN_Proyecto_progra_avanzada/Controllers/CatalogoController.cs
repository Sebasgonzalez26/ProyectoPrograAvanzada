using KN_Proyecto_progra_avanzada.EF;
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
                                       .ToList();

                return View(resultado);
            }
        }


        // ---------------------------------------------------------
        // GET: AGREGAR PRODUCTO
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult AgregarProducto()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                ViewBag.Categorias = new SelectList(
                    context.tbCategoria.ToList(),
                    "IdCategoria",
                    "Nombre"
                );
            }

            return View(new tbCatalogo());
        }


        // ---------------------------------------------------------
        // POST: AGREGAR PRODUCTO
        // ---------------------------------------------------------
        [HttpPost]
        public ActionResult AgregarProducto(tbCatalogo modelo)
        {
            // Imagen por defecto si viene vacía
            if (string.IsNullOrWhiteSpace(modelo.Imagen))
            {
                modelo.Imagen = "https://via.placeholder.com/150";
            }

            if (!ModelState.IsValid)
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    ViewBag.Categorias = new SelectList(
                        context.tbCategoria.ToList(),
                        "IdCategoria",
                        "Nombre"
                    );
                }
                return View(modelo);
            }

            using (var context = new BDProyecto_KNEntities())
            {
                context.Database.ExecuteSqlCommand(
                    "EXEC AgregarCatalogo @p0, @p1, @p2, @p3, @p4, @p5, @p6",
                    modelo.Nombre,
                    modelo.Descripcion,
                    modelo.Precio,
                    modelo.Stock,
                    modelo.Imagen,
                    modelo.Estado,
                    modelo.IdCategoria
                );
            }

            return RedirectToAction("VerCatalogo");
        }


        // ---------------------------------------------------------
        // GET: EDITAR PRODUCTO
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult EditarProducto(int id)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var producto = context.tbCatalogo.FirstOrDefault(x => x.IdProducto == id);

                if (producto == null)
                    return HttpNotFound();

                ViewBag.Categorias = new SelectList(
                    context.tbCategoria.ToList(),
                    "IdCategoria",
                    "Nombre",
                    producto.IdCategoria
                );

                return View(producto);
            }
        }


        // ---------------------------------------------------------
        // POST: EDITAR PRODUCTO
        // ---------------------------------------------------------
        [HttpPost]
        public ActionResult EditarProducto(tbCatalogo modelo)
        {
            // Imagen por defecto si viene vacía
            if (string.IsNullOrWhiteSpace(modelo.Imagen))
            {
                modelo.Imagen = "https://via.placeholder.com/150";
            }

            if (!ModelState.IsValid)
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    ViewBag.Categorias = new SelectList(
                        context.tbCategoria.ToList(),
                        "IdCategoria",
                        "Nombre"
                    );
                }
                return View(modelo);
            }

            using (var context = new BDProyecto_KNEntities())
            {
                context.Database.ExecuteSqlCommand(
                    "EXEC EditarCatalogo @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                    modelo.IdProducto,
                    modelo.Nombre,
                    modelo.Descripcion,
                    modelo.Precio,
                    modelo.Stock,
                    modelo.Imagen,
                    modelo.Estado,
                    modelo.IdCategoria
                );
            }

            return RedirectToAction("VerCatalogo");
        }
    }
}
