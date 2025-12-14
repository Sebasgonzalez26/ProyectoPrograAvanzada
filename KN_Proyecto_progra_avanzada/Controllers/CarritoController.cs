using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace KN_Proyecto_progra_avanzada.Controllers
{
    public class CarritoController : Controller
    {
        // ---------------------------
        // VER CARRITO
        // ---------------------------
        [HttpGet]
        public ActionResult VerCarrito()
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Index", "Home");

            int idUsuario = int.Parse(Session["IdUsuario"].ToString());

            using (var context = new BDProyecto_KNEntities())
            {
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

                        // Para la vista
                        Nombre = x.tbCatalogo.Nombre,
                        Imagen = x.tbCatalogo.Imagen,
                        Precio = x.tbCatalogo.Precio
                    })
                    .ToList();

                return View(lista);
            }
        }

        // ---------------------------
        // AGREGAR AL CARRITO (1 a la vez)
        // ---------------------------
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
                        item.Fecha = DateTime.Now;
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

        // ---------------------------
        // ACTUALIZAR CANTIDAD
        // (si cantidad <= 0 -> elimina)
        // ---------------------------
        [HttpPost]
        public ActionResult ActualizarCantidad(int idCarrito, int cantidad)
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Index", "Home");

            int idUsuario = int.Parse(Session["IdUsuario"].ToString());

            using (var context = new BDProyecto_KNEntities())
            {
                var item = context.tbCarrito
                    .FirstOrDefault(x => x.IdCarrito == idCarrito && x.IdUsuario == idUsuario);

                if (item != null)
                {
                    if (cantidad <= 0)
                    {
                        context.tbCarrito.Remove(item);
                    }
                    else
                    {
                        item.Cantidad = cantidad;
                        item.Fecha = DateTime.Now;
                    }

                    context.SaveChanges();
                }
            }

            return RedirectToAction("VerCarrito");
        }

        // ---------------------------
        // QUITAR ITEM (por IdCarrito)
        // ---------------------------
        [HttpPost]
        public ActionResult Quitar(int idCarrito)
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Index", "Home");

            int idUsuario = int.Parse(Session["IdUsuario"].ToString());

            using (var context = new BDProyecto_KNEntities())
            {
                var item = context.tbCarrito
                    .FirstOrDefault(x => x.IdCarrito == idCarrito && x.IdUsuario == idUsuario);

                if (item != null)
                {
                    context.tbCarrito.Remove(item);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("VerCarrito");
        }

        // ---------------------------
        // VACIAR CARRITO COMPLETO
        // ---------------------------
        [HttpPost]
        public ActionResult Vaciar()
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Index", "Home");

            int idUsuario = int.Parse(Session["IdUsuario"].ToString());

            using (var context = new BDProyecto_KNEntities())
            {
                var items = context.tbCarrito
                    .Where(x => x.IdUsuario == idUsuario)
                    .ToList();

                if (items.Any())
                {
                    context.tbCarrito.RemoveRange(items);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("VerCarrito");
        }
    }
}
