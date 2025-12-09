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

using System.IO;


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

            var resultado = ConsultarCatalogo();
            return View(resultado);

        }


        [HttpGet]
        public ActionResult AgregarCatalogo()
        {
            CargarCategorias();                 // 🔹 Llenamos ViewBag.Categorias
            return View(new Catalogo());        // 🔹 Mandamos un modelo vacío
        }

        [HttpPost]
        public ActionResult AgregarCatalogo(Catalogo catalogo, HttpPostedFileBase ImgCatalogo)
        {

            using (var context = new BDProyecto_KNEntities())
            {
                var nuevoCatalogo = new tbCatalogo
                {
                    // IdProducto NO lo toques si es IDENTITY
                    Nombre = catalogo.Nombre,
                    Descripcion = catalogo.Descripcion,
                    Precio = catalogo.Precio,
                    Stock = catalogo.Stock,
                    Imagen = string.Empty,
                    FechaRegistro = DateTime.Now,
                    Estado = true,
                    IdCategoria = catalogo.IdCategoria
                };

                context.tbCatalogo.Add(nuevoCatalogo);
                var resultadoInsercion = context.SaveChanges();

                if (resultadoInsercion > 0)
                {

                    //guardar la imagen 
                    var ext = Path.GetExtension(ImgCatalogo.FileName);

                    var rutaImagen = AppDomain.CurrentDomain.BaseDirectory + "imgCatalogos\\" + nuevoCatalogo.IdProducto + ext;
                    ImgCatalogo.SaveAs(rutaImagen);



                    //Actualizar la ruta de la imagen 
                    nuevoCatalogo.Imagen = "/imgCatalogos/" + nuevoCatalogo.IdProducto + ext;
                    context.SaveChanges();





                    return RedirectToAction("VerCatalogo", "Catalogo");
                }


            }

            CargarCategorias();
            ViewBag.Mensaje = "La informacion no se pudo regisrar";
            return View();



        }



        //--------------------------------------
        //------Actualizar----------------------
        //----------------------------



        [HttpGet]
        public ActionResult ActualizarCatalogo(int q)
        {
            using (var context = new BDProyecto_KNEntities())
            {


                //Tomar el objeto de la BD
                var resultado = context.tbCatalogo

                                       .Where(x => x.IdProducto == q)
                                       .ToList();

                //Convertirlo en un objeto Propio
                var datos = resultado.Select(p => new Catalogo
                {

                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Imagen = p.Imagen,
                    IdCategoria = p.IdCategoria,


                }).FirstOrDefault();



                CargarCategorias();
                return View(datos);


            }
        }




        // 🔹 Llenamos ViewBag.Categorias

        [HttpPost]
        public ActionResult ActualizarCatalogo(Catalogo catalogo, HttpPostedFileBase ImgCatalogo)
        {

            using (var context = new BDProyecto_KNEntities())
            {

                //Tomar el objeto de la BD
                var resultadoConsulta = context.tbCatalogo
                                               .Where(x => x.IdProducto == catalogo.IdProducto)
                                               .FirstOrDefault();

                //Si existe se manda a actualizar
                if (resultadoConsulta != null)
                {
                    //Actualizar los campos del formulario
                    resultadoConsulta.Nombre = catalogo.Nombre;
                    resultadoConsulta.Descripcion = catalogo.Descripcion;
                    resultadoConsulta.Precio = catalogo.Precio;
                    resultadoConsulta.Stock = catalogo.Stock;
                    resultadoConsulta.IdCategoria = catalogo.IdCategoria;

                    var resultadoActualizacion = context.SaveChanges();

                    if (resultadoActualizacion > 0)
                    {
                        return RedirectToAction("VerCatalogo", "Catalogo");
                    }

                    // Si llegó aquí, no se actualizó nada
                    CargarCategorias();
                    ViewBag.Mensaje = "La informacion no se ha podido actualizar";
                    return View(catalogo);
                }
            }

            // Si no encontró el producto (resultadoConsulta == null)
            CargarCategorias();
            ViewBag.Mensaje = "La informacion no se ha podido actualizar";
            return View();
        }













        //--------------------------------------
        //------Actualizar----------------------
        //----------------------------



        //--------------------------------------
        //------Ddelete----------------------
        //----------------------------
        [HttpGet]


        public ActionResult CambiarEstadoCatalogo(int q)
        {


            using (var context = new BDProyecto_KNEntities())
            {
                var resultadoConsulta = context.tbCatalogo.Where(x => x.IdProducto == q).FirstOrDefault();


                if (resultadoConsulta != null)
                {
                    resultadoConsulta.Estado = resultadoConsulta.Estado ? false : true;


                    var resultadoActualizacion = context.SaveChanges();


                    if (resultadoActualizacion > 0)
                        return RedirectToAction("VerCatalogo", "Catalogo");

                }

                var resultado = ConsultarCatalogo();

                ViewBag.Mensaje = "El estado no se puede actualizar, intente de nuevo";
                return View("VerCatalogo", resultado);

            }



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



        private List<Catalogo> ConsultarCatalogo()
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

                return resultado;
            }

        }






    }
}
