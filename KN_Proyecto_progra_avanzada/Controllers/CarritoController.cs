using KN_Proyecto_progra_avanzada.Models; 
using System.Collections.Generic;         
using System.Data.Entity;                  

using KN_Proyecto_progra_avanzada.EF;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace KN_Proyecto_progra_avanzada.Controllers
{
    public class CarritoController : Controller
    {
        
        [HttpGet]
        public ActionResult VerCarrito()
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Index", "Home");

            int idUsuario = int.Parse(Session["IdUsuario"].ToString());

            using (var context = new BDProyecto_KNEntities())
            {
                // Traemos carrito + info del producto (por navegación tbCatalogo)
                var lista = context.tbCarrito
                    .Include(x => x.tbCatalogo)
                    .Where(x => x.IdUsuario == idUsuario)
                    .OrderByDescending(x => x.Fecha)
                    .Select(x => new Carrito
                    {
                        IdCarrito = x.IdCarrito,
                        IdUsuario = x.IdUsuario,
                        IdProducto = x.IdProducto,
                        Cantidad = x.Cantidad,
                        Fecha = x.Fecha,

                        // info para mostrar en la vista
                        Nombre = x.tbCatalogo.Nombre,
                        Imagen = x.tbCatalogo.Imagen,
                        Precio = x.tbCatalogo.Precio
                    })
                    .ToList();

                return View(lista);
            }
        }

        [HttpPost]
        public ActionResult AgregarAlCarrito(int id)
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Index", "Home");

            int idUsuario = int.Parse(Session["IdUsuario"].ToString());

            using (var context = new BDProyecto_KNEntities())
            {
                try
                {
                    var item = context.tbCarrito
                        .FirstOrDefault(x => x.IdUsuario == idUsuario && x.IdProducto == id);

                    if (item == null)
                    {
                        context.tbCarrito.Add(new tbCarrito
                        {
                            IdUsuario = idUsuario,
                            IdProducto = id,
                            Cantidad = 1,
                            Fecha = DateTime.Now
                        });
                    }
                    else
                    {
                        item.Cantidad += 1;
                    }

                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var msg = ex.InnerException?.InnerException?.Message ?? ex.Message;
                    throw new Exception("Error al guardar carrito: " + msg, ex);
                }
            }

            return RedirectToAction("VerTienda", "Tienda");
        }
    }
}
