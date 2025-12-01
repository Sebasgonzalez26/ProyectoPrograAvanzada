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


    }
}
