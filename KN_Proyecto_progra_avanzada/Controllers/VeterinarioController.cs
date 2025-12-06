using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KN_Proyecto_progra_avanzada.Controllers
{
    public class VeterinarioController : Controller
    {
        // ---------------------------------------------------------
        // LISTAR VETERINARIOS
        // ---------------------------------------------------------
        public ActionResult VerVeterinarios()
        {
            var resultado = ConsultarVeterinarios();
            return View(resultado);
        }


        // ---------------------------------------------------------
        // GET: AGREGAR VETERINARIO
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult AgregarVeterinario()
        {
            return View(new Veterinario());
        }


        // ---------------------------------------------------------
        // POST: AGREGAR VETERINARIO
        // ---------------------------------------------------------
        [HttpPost]
        public ActionResult AgregarVeterinario(Veterinario veterinario)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                return View(veterinario);
            }

            try
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    // Verificar si el correo ya existe
                    bool correoExiste = context.tbVeterinarios
                        .Any(v => v.Correo == veterinario.Correo);

                    if (correoExiste)
                    {
                        ViewBag.Mensaje = "El correo ingresado ya está registrado para otro veterinario.";
                        return View(veterinario);
                    }

                    var nuevoVeterinario = new tbVeterinarios
                    {
                        Nombre = veterinario.Nombre,
                        Apellidos = veterinario.Apellidos,
                        Correo = veterinario.Correo,
                        Telefono = veterinario.Telefono,
                        Estado = true,              // siempre activo al crear
                        FechaRegistro = DateTime.Now       
                    };

                    context.tbVeterinarios.Add(nuevoVeterinario);
                    var resultadoInsercion = context.SaveChanges();

                    if (resultadoInsercion > 0)
                    {
                        return RedirectToAction("VerVeterinarios", "Veterinario");
                    }
                }

                ViewBag.Mensaje = "La información no se pudo registrar.";
                return View(veterinario);
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Ocurrió un error: " + ex.Message;
                return View(veterinario);
            }
        }


        // ---------------------------------------------------------
        // GET: ACTUALIZAR VETERINARIO
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult ActualizarVeterinario(int? q)
        {

            if (!q.HasValue)
            {
                return RedirectToAction("VerVeterinarios", "Veterinario");
            }
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbVeterinarios
                    .Where(v => v.IdVeterinario == q)
                    .ToList();

                var datos = resultado.Select(v => new Veterinario
                {
                    IdVeterinario = v.IdVeterinario,
                    Nombre = v.Nombre,
                    Apellidos = v.Apellidos,
                    Correo = v.Correo,
                    Telefono = v.Telefono
                    
                }).FirstOrDefault();

                if (datos == null)
                {
                    TempData["Mensaje"] = "El veterinario seleccionado no existe.";
                    return RedirectToAction("VerVeterinarios", "Veterinario");
                }

                return View(datos);
            }
        }


        // ---------------------------------------------------------
        // POST: ACTUALIZAR VETERINARIO
        // ---------------------------------------------------------
        [HttpPost]
        public ActionResult ActualizarVeterinario(Veterinario veterinario)
        {
            if (!ModelState.IsValid)
            {
                return View(veterinario);
            }

            try
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    var veterinarioExistente = context.tbVeterinarios
                        .FirstOrDefault(v => v.IdVeterinario == veterinario.IdVeterinario);

                    if (veterinarioExistente == null)
                    {
                        ViewBag.Mensaje = "El veterinario no fue encontrado.";
                        return View(veterinario);
                    }

                    // Validar correo único (que no choque con otro veterinario)
                    bool correoDuplicado = context.tbVeterinarios
                        .Any(v => v.Correo == veterinario.Correo &&
                                  v.IdVeterinario != veterinario.IdVeterinario);

                    if (correoDuplicado)
                    {
                        ViewBag.Mensaje = "El correo ingresado ya está asignado a otro veterinario.";
                        return View(veterinario);
                    }

                    veterinarioExistente.Nombre = veterinario.Nombre;
                    veterinarioExistente.Apellidos = veterinario.Apellidos;
                    veterinarioExistente.Correo = veterinario.Correo;
                    veterinarioExistente.Telefono = veterinario.Telefono;
                    // Estado y FechaRegistro no se tocan aquí

                    var resultadoActualizacion = context.SaveChanges();

                    if (resultadoActualizacion > 0)
                    {
                        return RedirectToAction("VerVeterinarios", "Veterinario");
                    }

                    ViewBag.Mensaje = "La información no se pudo actualizar.";
                    return View(veterinario);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Ocurrió un error: " + ex.Message;
                return View(veterinario);
            }
        }


        // ---------------------------------------------------------
        // CAMBIAR ESTADO (ACTIVAR / INACTIVAR)
        // ---------------------------------------------------------
        public ActionResult CambiarEstadoVeterinario(int q)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var veterinario = context.tbVeterinarios
                    .FirstOrDefault(v => v.IdVeterinario == q);

                if (veterinario != null)
                {
                    veterinario.Estado = !veterinario.Estado;
                    context.SaveChanges();
                }
            }

            return RedirectToAction("VerVeterinarios", "Veterinario");
        }


        // ---------------------------------------------------------
        // CONSULTAR TODOS LOS VETERINARIOS
        // ---------------------------------------------------------
        private List<Veterinario> ConsultarVeterinarios()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbVeterinarios
                    .ToList()
                    .Select(v => new Veterinario
                    {
                        IdVeterinario = v.IdVeterinario,
                        Nombre = v.Nombre,
                        Apellidos = v.Apellidos,
                        Correo = v.Correo,
                        Telefono = v.Telefono,
                        Estado = v.Estado,

                    })
                    .ToList();

                return resultado;
            }
        }
    }
}
