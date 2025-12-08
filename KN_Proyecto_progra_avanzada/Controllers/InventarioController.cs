
using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using KN_Proyecto_progra_avanzada.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KN_Proyecto_progra_avanzada.Controllers
{
    public class InventarioController : Controller
    {

        [HttpGet]
        public ActionResult VerInventario()
        {
            var resultado = ConsultarInventario();
            return View(resultado);
        }



        [HttpGet]

        public ActionResult AgregarInventario()
        {


            return View(new Inventario());
        }



        [HttpPost]

        public ActionResult AgregarInventario(Inventario inventario, HttpPostedFileBase ImgInventario)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var nuevoInventario = new tbInventario
                {
                    Nombre = inventario.Nombre,
                    Descripcion = inventario.Descripcion,
                    Categoria = inventario.Categoria,
                    Stock = inventario.Stock,
                    PrecioVenta = inventario.PrecioVenta,
                    Unidad = inventario.Unidad,
                    Imagen = string.Empty,
                    FechaIngreso = DateTime.Now,
                    Estado = true

                };

                context.tbInventario.Add(nuevoInventario);
                var resultadoInsercion = context.SaveChanges();

                if (resultadoInsercion > 0)
                {

                    var ext = Path.GetExtension(ImgInventario.FileName);

                    var rutaImagen = AppDomain.CurrentDomain.BaseDirectory + "imgInventarios\\" + nuevoInventario.IdInventario + ext;

                    ImgInventario.SaveAs(rutaImagen);

                    //Actualizar la ruta de la imagen 
                    nuevoInventario.Imagen = "/imgInventarios/" + nuevoInventario.IdInventario + ext;
                    context.SaveChanges();


                    return RedirectToAction("VerInventario", "Inventario");
                }
            }




          


            ViewBag.Mensaje = "La informacion no se pudo insertar ";
            return View();

        }




        [HttpGet]

        public ActionResult ActualizarInventario(int q)
        {


            using(var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbInventario
                                .Where(x => x.IdInventario == q)
                                .ToList();



                var datos = resultado.Select(i => new Inventario
                {
                    IdInventario = i.IdInventario,
                    Nombre = i.Nombre,
                    Descripcion = i.Descripcion,
                    Categoria = i.Categoria,
                    Stock = i.Stock,
                    PrecioVenta = i.PrecioVenta,
                    Unidad = i.Unidad,
                    Imagen = i.Imagen


                }).FirstOrDefault();

                return View(datos);
            }
        }




        [HttpPost]
        public ActionResult ActualizarInventario(Inventario inventario, HttpPostedFileBase ImgInventario)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                // Buscar el registro en BD
                var resultadoConsulta = context.tbInventario
                                               .Where(x => x.IdInventario == inventario.IdInventario)
                                               .FirstOrDefault();

                if (resultadoConsulta == null)
                {
                    ViewBag.Mensaje = "No se encontró el inventario a actualizar.";
                    return View(inventario);
                }

                // --------------------------
                // ACTUALIZAR CAMPOS NORMALES
                // --------------------------
                resultadoConsulta.Nombre = inventario.Nombre;
                resultadoConsulta.Descripcion = inventario.Descripcion;
                resultadoConsulta.Categoria = inventario.Categoria;
                resultadoConsulta.Stock = inventario.Stock;
                resultadoConsulta.PrecioVenta = inventario.PrecioVenta;
                resultadoConsulta.Unidad = inventario.Unidad;

                // --------------------------
                // ACTUALIZAR IMAGEN (SI SE ENVÍA UNA NUEVA)
                // --------------------------
                if (ImgInventario != null && ImgInventario.ContentLength > 0)
                {
                    var ext = Path.GetExtension(ImgInventario.FileName);
                    var carpeta = AppDomain.CurrentDomain.BaseDirectory + "imgInventarios\\";

                    // Crear carpeta si no existe
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    // Ruta física nueva
                    var rutaNueva = Path.Combine(carpeta, resultadoConsulta.IdInventario + ext);

                    // --------------------------
                    // BORRAR IMAGEN ANTERIOR (OPCIONAL PERO RECOMENDADO)
                    // --------------------------
                    if (!string.IsNullOrEmpty(resultadoConsulta.Imagen))
                    {
                        var rutaAnterior = AppDomain.CurrentDomain.BaseDirectory + resultadoConsulta.Imagen.TrimStart('/');
                        if (System.IO.File.Exists(rutaAnterior))
                            System.IO.File.Delete(rutaAnterior);
                    }

                    // Guardar nueva imagen
                    ImgInventario.SaveAs(rutaNueva);

                    // Actualizar ruta relativa
                    resultadoConsulta.Imagen = "/imgInventarios/" + resultadoConsulta.IdInventario + ext;
                }

                // --------------------------
                // GUARDAR EN BD
                // --------------------------
                var resultadoActualizacion = context.SaveChanges();

                if (resultadoActualizacion > 0)
                {
                    return RedirectToAction("VerInventario", "Inventario");
                }

                ViewBag.Mensaje = "La información no se pudo actualizar.";
                return View(inventario);
            }
        }

        [HttpGet]

        public ActionResult CambiarEstadoInventario(int q)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultadoConsulta = context.tbInventario.Where(x => x.IdInventario == q).FirstOrDefault();


                if (resultadoConsulta != null)
                {
                    resultadoConsulta.Estado = resultadoConsulta.Estado ? false : true;


                    var resultadoActualizacion = context.SaveChanges();


                    if (resultadoActualizacion > 0)
                        return RedirectToAction("VerInventario", "Inventario");
                }

                var resultaddo = ConsultarInventario();


                ViewBag.Mensaje = "El estado no se puede actualizar, intente de nuevo";
                return View( "VerInventario",resultaddo);
            }
        }

        private List<Inventario> ConsultarInventario()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbInventario
                                        .ToList()
                                        .Select(i => new Inventario
                                        {
                                            IdInventario = i.IdInventario,
                                            Nombre = i.Nombre,
                                            Descripcion = i.Descripcion,
                                            Categoria = i.Categoria,
                                            Stock = i.Stock,
                                            PrecioVenta = i.PrecioVenta,
                                            Unidad = i.Unidad,
                                            Imagen = i.Imagen,
                                            FechaIngreso = i.FechaIngreso,
                                            Estado = i.Estado

                                        })
                                        .ToList();

                return resultado;

            }
        }



    }
}
