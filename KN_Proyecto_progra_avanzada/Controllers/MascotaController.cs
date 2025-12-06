using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace KN_Proyecto_progra_avanzada.Controllers
{
    public class MascotaController : Controller
    {
        // ---------------------------------------------------------
        // LISTAR MASCOTAS
        // ---------------------------------------------------------
        public ActionResult VerMascotas()
        {
            var resultado = ConsultarMascotas();
            return View(resultado);
        }


        // ---------------------------------------------------------
        // GET: AGREGAR MASCOTA
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult AgregarMascota()
        {
            CargarClientes();
            return View(new Mascota());
        }


        // ---------------------------------------------------------
        // POST: AGREGAR MASCOTA
        // ---------------------------------------------------------
        [HttpPost]
        public ActionResult AgregarMascota(Mascota mascota)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                CargarClientes();
                return View(mascota);
            }

            try
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    var nuevaMascota = new tbMascotas
                    {
                        Nombre = mascota.Nombre,
                        Especie = mascota.Especie,
                        Raza = mascota.Raza,
                        Sexo = mascota.Sexo,
                        FechaNacimiento = mascota.FechaNacimiento,
                        IdCliente = mascota.IdCliente,
                        Estado = true,                // siempre activa al crear
                        FechaRegistro = DateTime.Now
                    };

                    context.tbMascotas.Add(nuevaMascota);
                    var resultadoInsercion = context.SaveChanges();

                    if (resultadoInsercion > 0)
                    {
                        return RedirectToAction("VerMascotas", "Mascota");
                    }
                }

                // Algo salió mal
                CargarClientes();
                ViewBag.Mensaje = "La información no se pudo registrar.";
                return View(mascota);
            }
            catch (Exception ex)
            {
                CargarClientes();
                ViewBag.Mensaje = "Ocurrió un error: " + ex.Message;
                return View(mascota);
            }
        }



        [HttpGet]


        public ActionResult ActualizarMascota(int  q)
        {

            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbMascotas
                    .Where(m => m.IdMascota == q)
                    .ToList();



                var datos = resultado.Select(p => new Mascota
                {
                    IdMascota = p.IdMascota,
                    Nombre = p.Nombre,
                    Especie = p.Especie,
                    Raza = p.Raza,
                    Sexo = p.Sexo,
                    FechaNacimiento = p.FechaNacimiento,
                    IdCliente = p.IdCliente
                }).FirstOrDefault();


                CargarClientes();
                return View(datos);
            }


            

        }



        [HttpPost]

        public ActionResult ActualizarMascota(Mascota mascota)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                CargarClientes();
                return View(mascota);
            }
            try
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    var mascotaExistente = context.tbMascotas
                        .FirstOrDefault(m => m.IdMascota == mascota.IdMascota);
                    if (mascotaExistente != null)
                    {
                        mascotaExistente.Nombre = mascota.Nombre;
                        mascotaExistente.Especie = mascota.Especie;
                        mascotaExistente.Raza = mascota.Raza;
                        mascotaExistente.Sexo = mascota.Sexo;
                        mascotaExistente.FechaNacimiento = mascota.FechaNacimiento;
                        mascotaExistente.IdCliente = mascota.IdCliente;
                        var resultadoActualizacion = context.SaveChanges();
                        if (resultadoActualizacion > 0)
                        {
                            return RedirectToAction("VerMascotas", "Mascota");
                        }
                    }
                    // Algo salió mal
                    CargarClientes();
                    ViewBag.Mensaje = "La información no se pudo actualizar.";
                    return View(mascota);
                }
            }
            catch (Exception ex)
            {
                CargarClientes();
                ViewBag.Mensaje = "Ocurrió un error: " + ex.Message;
                return View(mascota);
            }
        }


        // ---------------------------------------------------------
        // Eliminar(Inactivar estado)
        // ---------------------------------------------------------

        [HttpGet]

        public ActionResult CambiarEstadoMascota(int q)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultadoConsulta = context.tbMascotas
                    .Where(x => x.IdMascota == q).FirstOrDefault();




                if (resultadoConsulta != null)
                {
                    resultadoConsulta.Estado = resultadoConsulta.Estado ? false : true;


                    var resultadoActualizacion = context.SaveChanges();


                    if (resultadoActualizacion > 0)
                        return RedirectToAction("VerMascotas", "Mascota");
                }


                var resultado = ConsultarMascotas();
                ViewBag.Mensaje = "El estado no se puede actualizar, intente de nuevo";

                return View("VerMascotas", resultado);



            }
        }


        // ---------------------------------------------------------
        // CARGAR CLIENTES PARA EL DROPDOWN
        // ---------------------------------------------------------
        private void CargarClientes()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var clientes = context.tbClientes
                    .Where(c => c.Estado == true)
                    .Select(c => new
                    {
                        c.IdCliente,
                        NombreCompleto = c.Nombre + " " + c.Apellidos
                    })
                    .ToList();

                ViewBag.Clientes = new SelectList(clientes, "IdCliente", "NombreCompleto");
            }
        }


        // ---------------------------------------------------------
        // CONSULTAR TODAS LAS MASCOTAS
        // ---------------------------------------------------------
        private List<Mascota> ConsultarMascotas()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbMascotas
                    .Include(m => m.tbClientes)
                    .ToList()
                    .Select(m => new Mascota
                    {
                        IdMascota = m.IdMascota,
                        Nombre = m.Nombre,
                        Especie = m.Especie,
                        Raza = m.Raza,
                        Sexo = m.Sexo,
                        FechaNacimiento = m.FechaNacimiento,
                        IdCliente = m.IdCliente,
                        Estado = m.Estado,
                        NombreCliente = m.tbClientes != null
                                            ? m.tbClientes.Nombre + " " + m.tbClientes.Apellidos
                                            : "Sin asignar"
                    })
                    .ToList();

                return resultado;
            }
        }
    }
}
