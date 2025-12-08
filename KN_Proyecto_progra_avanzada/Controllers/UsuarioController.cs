using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using KN_Proyecto_progra_avanzada.Services;
using System;
using System.Linq;
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

                // URL para API de cédula
                ViewBag.UrlConsultarCedula = Url.Action("ConsultarCedula", "Home");

                // Traer mensaje que dejó el POST
                ViewBag.Mensaje = TempData["Mensaje"];
                ViewBag.TipoMensaje = TempData["TipoMensaje"];

                return View(datos);
            }
        }

        [HttpPost]
        public ActionResult VerPerfil(Usuario usuario)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var consecutivo = int.Parse(Session["IdUsuario"].ToString());
                var resultado = context.tbUsuario
                    .FirstOrDefault(x => x.IdUsuario == consecutivo);

                if (resultado != null)
                {
                    resultado.Identificacion = usuario.Identificacion;
                    resultado.Nombre = usuario.Nombre;
                    resultado.CorreoElectronico = usuario.CorreoElectronico;

                    var filas = context.SaveChanges();

                    if (filas > 0)
                    {
                        TempData["Mensaje"] = "La información se actualizó correctamente.";
                        TempData["TipoMensaje"] = "success"; // Bootstrap: alert-success
                    }
                    else
                    {
                        TempData["Mensaje"] = "La información no se actualizó correctamente.";
                        TempData["TipoMensaje"] = "danger"; // alert-danger
                    }
                }
                else
                {
                    TempData["Mensaje"] = "No se encontró el usuario.";
                    TempData["TipoMensaje"] = "danger";
                }
            }

            // Muy importante: redirigir al GET para que lea TempData
            return RedirectToAction("VerPerfil");
        }



        [HttpGet]
        public ActionResult CambiarAcceso()
        {
            ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.TipoMensaje = TempData["TipoMensaje"];

            return View();
        }


        [HttpPost]
        public ActionResult CambiarAcceso(string ContrasenaActual, string NuevaContrasena)
        {
            if (string.IsNullOrWhiteSpace(ContrasenaActual) || string.IsNullOrWhiteSpace(NuevaContrasena))
            {
                TempData["Mensaje"] = "Debe completar ambos campos de contraseña.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction("CambiarAcceso");
            }

            try
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    // 1. Obtener el usuario en sesión
                    var idUsuario = int.Parse(Session["IdUsuario"].ToString());

                    var usuarioBD = context.tbUsuario
                                           .FirstOrDefault(u => u.IdUsuario == idUsuario);

                    if (usuarioBD == null)
                    {
                        TempData["Mensaje"] = "No se encontró el usuario.";
                        TempData["TipoMensaje"] = "danger";
                        return RedirectToAction("CambiarAcceso");
                    }

                    // 2. Validar que la contraseña actual coincida (TEXTO PLANO)
                    if (usuarioBD.Contrasenna != ContrasenaActual)
                    {
                        TempData["Mensaje"] = "La contraseña actual es incorrecta.";
                        TempData["TipoMensaje"] = "danger";
                        return RedirectToAction("CambiarAcceso");
                    }

                    // 3. Guardar la nueva contraseña (TEXTO PLANO)
                    usuarioBD.Contrasenna = NuevaContrasena;

                    var filas = context.SaveChanges();

                    if (filas > 0)
                    {
                        TempData["Mensaje"] = "La contraseña se actualizó correctamente.";
                        TempData["TipoMensaje"] = "success";
                    }
                    else
                    {
                        TempData["Mensaje"] = "No se pudo actualizar la contraseña.";
                        TempData["TipoMensaje"] = "danger";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Ocurrió un error al actualizar la contraseña: " + ex.Message;
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction("CambiarAcceso");
        }



    }
}
