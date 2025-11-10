using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KN_Proyecto_progra_avanzada.Controllers
{
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
                    // 1. Generar contraseña
                    var contrasennaGenerada = GenerarContrasenna();

                    // 2. Actualizar la contraseña al usuario en la BD
                    // OJO: esto es en texto plano; idealmente la encriptas/hasheas
                    resultadoConsulta.Contrasenna = contrasennaGenerada;
                    var resultadoActualizacion = context.SaveChanges();

                    // 3. Enviar el correo solo si se guardó en la BD
                    if (resultadoActualizacion > 0)
                    {
                        EnviarCorreoRecuperacion(
                            "Contraseña de acceso",
                            contrasennaGenerada,
                            resultadoConsulta.CorreoElectronico
                        );

                        // podrías mostrar un mensaje en la misma vista, pero tú estabas redirigiendo
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.Mensaje = "La información no se ha podido restablecer.";
                return View();
            }
        }

        // ----------------- PRINCIPAL -----------------
        [HttpGet]
        public ActionResult Principal()
        {
            return View();
        }

        // =====================================================================
        // MÉTODOS PRIVADOS
        // =====================================================================

        // Generar contraseña aleatoria
        private string GenerarContrasenna()
        {
            int longitud = 8;
            const string caracteres = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789";
            char[] resultado = new char[longitud];

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[sizeof(uint)];

                for (int i = 0; i < longitud; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    resultado[i] = caracteres[(int)(num % (uint)caracteres.Length)];
                }
            }

            return new string(resultado);
        }

        // Enviar correo de recuperación con HTML bonito
        private void EnviarCorreoRecuperacion(string asunto, string contrasenna, string destinatario)
        {
            var correoSMTP = ConfigurationManager.AppSettings["CorreoSMTP"];
            var contrasennaSMTP = ConfigurationManager.AppSettings["ContrasennaSMTP"];

            var smtp = new SmtpClient("smtp.office365.com")
            {
                Port = 587, // 587 = TLS
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(correoSMTP, contrasennaSMTP)
            };

            // Armamos el HTML y sustituimos la contraseña
            string cuerpoHtml = GetPlantillaRecuperacion()
                                    .Replace("{{PASSWORD}}", contrasenna);

            var mensaje = new MailMessage
            {
                From = new MailAddress(correoSMTP, "Mundo Animal Vet"),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            mensaje.To.Add(destinatario);

            smtp.Send(mensaje);
        }


        private string GetPlantillaRecuperacion()
        {
            // Ruta física del archivo dentro del proyecto
            var ruta = Server.MapPath("~/Templates/templateRecuperacion.html");

            // Leer el contenido completo del archivo
            return System.IO.File.ReadAllText(ruta);
        }



       
    }
}
