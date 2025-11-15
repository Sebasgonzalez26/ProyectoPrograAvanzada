using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using KN_Proyecto_progra_avanzada.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KN_Proyecto_progra_avanzada.Controllers
{
    [Seguridad]


    public class UsuarioController : Controller
    {
        [HttpGet]
        public ActionResult VerPerfil()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var consecutivo = int.Parse(Session["IdUsuario"].ToString());

                var resultado = context.tbUsuario
                    .Where(x => x.IdUsuario == consecutivo);

                var datos = resultado.Select(p => new Usuario
                {
                    IdUsuario = p.IdUsuario,
                    Identificacion = p.Identificacion,
                    Nombre = p.Nombre,
                    CorreoElectronico = p.CorreoElectronico,
                    NombrePerfil = p.tbPerfil.Nombre
                }).FirstOrDefault();

                // Pasar la URL de la API a la vista
                ViewBag.UrlConsultarCedula = Url.Action("ConsultarCedula", "Home");

                return View(datos);
            }
        }
    }
}