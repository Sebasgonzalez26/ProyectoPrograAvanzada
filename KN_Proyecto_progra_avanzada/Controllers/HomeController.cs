using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using KN_Proyecto_progra_avanzada.Services;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace KN_Proyecto_progra_avanzada.Controllers
{


    [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true, VaryByParam = "*")]
    public class HomeController : Controller
    {
        // ----------------- LOGIN -----------------
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Usuario usuario)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                // Llamamos al procedimiento almacenado para validar el usuario
                var resultado = context.ValidarUsuarios(usuario.CorreoElectronico, usuario.Contrasena).FirstOrDefault();

                if (resultado != null)
                {

                    // Redirige al home principal

                    Session["IdUsuario"] = resultado.IdUsuario;

                    Session["NombreUsuario"] = resultado.Nombre;

                    Session["IdPerfil"] = resultado.IdPerfil;
                    return RedirectToAction("Principal", "Home");
                }
                else
                {
                    // Si no existe o está inactivo
                    ViewBag.Mensaje = "Correo o contraseña incorrectos, o el usuario está inactivo.";
                    return View(usuario);
                }
            }
        }

        // ----------------- REGISTRO -----------------
        [HttpGet]
        public ActionResult Registro()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public ActionResult Registro(Usuario usuario)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.CrearUsuarios(
                    usuario.Identificacion,
                    usuario.Nombre,
                    usuario.CorreoElectronico,
                    usuario.Contrasena
                );

                if (resultado > 0)
                    return RedirectToAction("Index", "Home");

                ViewBag.Mensaje = "Error al registrar el usuario.";
                return View(usuario);
            }
        }

        // ----------------- CONSULTA CÉDULA (Gometa) -----------------
        [HttpGet]
        public async Task<ActionResult> ConsultarCedula(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json(new { ok = false, mensaje = "Cédula vacía" }, JsonRequestBehavior.AllowGet);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var http = new HttpClient())
            {
                var resp = await http.GetAsync("https://apis.gometa.org/cedulas/" + id);
                if (!resp.IsSuccessStatusCode)
                    return Json(new { ok = false, mensaje = "No se pudo consultar la cédula" }, JsonRequestBehavior.AllowGet);

                var json = await resp.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
        }

        // ----------------- RECUPERAR ACCESO -----------------
        [HttpGet]
        public ActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarAcceso(Usuario usuario)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                // Buscar usuario por correo
                var resultadoConsulta = context.tbUsuario
                    .Where(m => m.CorreoElectronico == usuario.CorreoElectronico)
                    .FirstOrDefault();

                if (resultadoConsulta != null)
                {
                    // 1. Generar contraseña con el service
                    var contrasennaGenerada = RecuperacionService.GenerarContrasenna();

                    // 2. Actualizar la contraseña al usuario en la BD
                    // OJO: esto es en texto plano; idealmente la encriptas/hasheas
                    resultadoConsulta.Contrasenna = contrasennaGenerada;
                    var resultadoActualizacion = context.SaveChanges();

                    // 3. Enviar el correo solo si se guardó en la BD
                    if (resultadoActualizacion > 0)
                    {
                        RecuperacionService.EnviarCorreoRecuperacion(
                            "Contraseña de acceso",
                            contrasennaGenerada,
                            resultadoConsulta.CorreoElectronico
                        );

                        // podrías mostrar un mensaje en la misma vista, pero tú estabas redirigiendo
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.Mensaje = "La información no se ha podido restablecer.";
                return View(usuario);
            }
        }

        // ----------------- PRINCIPAL -----------------

        [Seguridad]
        [HttpGet]
        public ActionResult Principal()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var hoy = DateTime.Today;
                var ahora = DateTime.Now;

                // 🔹 1) Citas de HOY (solo para el KPI)
                var citasHoyDb = context.tbCitas
                    .Where(c => DbFunctions.TruncateTime(c.FechaCita) == hoy)
                    .ToList();

                ViewBag.CitasHoy = citasHoyDb.Count;

                // 🔹 2) Próximas citas (desde ahora hacia adelante, máximo 3)
                var proximasCitasDb = context.tbCitas
                    .Include(c => c.tbMascotas)
                    .Include(c => c.tbMascotas.tbClientes)
                    .Where(c => c.FechaCita >= ahora)            // 👈 ya no solo hoy, sino futuras
                    .OrderBy(c => c.FechaCita)
                    .Take(3)
                    .ToList();

                var proximasCitas = proximasCitasDb
                    .Select(c => new Cita
                    {
                        IdCita = c.IdCita,
                        FechaCita = c.FechaCita,
                        Motivo = c.Motivo,
                        Estado = c.Estado,
                        NombreMascota = c.tbMascotas.Nombre,
                        NombreCliente = c.tbMascotas.tbClientes.Nombre + " " +
                                        c.tbMascotas.tbClientes.Apellidos
                    })
                    .ToList();

                ViewBag.ProximasCitas = proximasCitas;

                // 🔹 3) Productos para el módulo de tienda
                var productosTienda = context.tbCatalogo
                    .Where(p => p.Estado == true || p.Estado == null)
                    .OrderByDescending(p => p.FechaRegistro)
                    .Take(3)
                    .ToList();

                ViewBag.ProductosTienda = productosTienda;
            }

            return View();
        }



        [Seguridad]
        [HttpGet]
        public ActionResult CerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");

        }

    }
}
