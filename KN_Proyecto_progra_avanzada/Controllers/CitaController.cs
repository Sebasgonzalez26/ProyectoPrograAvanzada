using KN_Proyecto_progra_avanzada.EF;
using KN_Proyecto_progra_avanzada.Models;
using KN_Proyecto_progra_avanzada.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.UI.WebControls.WebParts;


namespace KN_Proyecto_progra_avanzada.Controllers
{
    public class CitaController : Controller
    {

        [HttpGet]
        public ActionResult VerCitas()
        {

            var resultado = ConsultarCita();
            return View(resultado);
        }




        [HttpGet]

        public ActionResult AgregarCita()
        {


            CargarCombosCita();
            return View(new Cita());


        }





        [HttpPost]

        public ActionResult AgregarCita(Cita cita)
        {


            if (!ModelState.IsValid)
            {
                CargarCombosCita();
                return View(cita);
            }



            using(var context = new BDProyecto_KNEntities())
            {
                var nuevaCita = new tbCitas
                {
                    IdMascota = cita.IdMascota,
                    IdVeterinario = cita.IdVeterinario,
                    FechaCita = cita.FechaCita,
                    Motivo = cita.Motivo,
                    Observaciones = cita.Observaciones,
                    Estado = string.IsNullOrEmpty(cita.Estado) ? "Programada" : cita.Estado,
                    FechaRegistro = DateTime.Now

                };

                context.tbCitas.Add(nuevaCita);
                context.SaveChanges();
            }

            return RedirectToAction("VerCitas", "Cita");

        }



        [HttpGet]

        public ActionResult ActualizarCita(int q)
        {

            using (var context = new BDProyecto_KNEntities())
            {

                var resultado = context.tbCitas
                    .Where(x => x.IdCita == q)
                    .ToList();

                var datos = resultado.Select(c => new Cita
                {
                    IdMascota = c.IdMascota,
                    IdVeterinario = c.IdVeterinario,
                    IdCita = c.IdCita,
                    FechaCita = c.FechaCita,
                    Motivo = c.Motivo,
                    Observaciones = c.Observaciones,
                    Estado = c.Estado
                }).FirstOrDefault();

                CargarCombosCita();
                return View(datos);


            }


        }


        [HttpPost]
        public ActionResult ActualizarCita(Cita cita)
        {
           
            using (var context = new BDProyecto_KNEntities())
            {
                // Tomar el objeto de la BD
                var resultadoConsulta = context.tbCitas
                                               .FirstOrDefault(x => x.IdCita == cita.IdCita);

                // Si existe se manda a actualizar
                if (resultadoConsulta != null)
                {
                    resultadoConsulta.IdMascota = cita.IdMascota;
                    resultadoConsulta.IdVeterinario = cita.IdVeterinario;
                    resultadoConsulta.FechaCita = cita.FechaCita;
                    resultadoConsulta.Motivo = cita.Motivo;
                    resultadoConsulta.Observaciones = cita.Observaciones;
                    resultadoConsulta.Estado = cita.Estado;



                    var resultadoActualizacion = context.SaveChanges();

                    if (resultadoActualizacion > 0)
                    {
                        return RedirectToAction("VerCitas", "Cita");
                    }

                    // Si llegó aquí, no se actualizó nada
                    CargarCombosCita();
                    ViewBag.Mensaje = "La información no se ha podido actualizar, intente de nuevo.";
                    return View(cita);
                }
            }

            // Si no encontró la cita 
            CargarCombosCita();
            ViewBag.Mensaje = "No se encontró la cita a actualizar.";
            return View(cita);
        }






        private List<Cita> ConsultarCita()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbCitas
                    .Include(c => c.tbMascotas)              // relación con mascota
                    .Include(c => c.tbMascotas.tbClientes)   // dueño de la mascota
                    .Include(c => c.tbVeterinarios)          // relación con veterinario
                    .ToList()
                    .Select(c => new Cita
                    {
                        IdCita = c.IdCita,
                        IdMascota = c.IdMascota,
                        IdVeterinario = c.IdVeterinario,

                        FechaCita = c.FechaCita,
                        Motivo = c.Motivo,
                        Observaciones = c.Observaciones,
                        Estado = c.Estado,
                        FechaRegistro = c.FechaRegistro,

                        NombreMascota = c.tbMascotas.Nombre,
                        NombreCliente = c.tbMascotas.tbClientes.Nombre + " " +
                                        c.tbMascotas.tbClientes.Apellidos,
                        NombreVeterinario = c.tbVeterinarios.Nombre
                    })
                    .ToList();

                return resultado;
            }
        }


        private void CargarCombosCita()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                //  Mascotas activas (con nombre del dueño)
                var mascotas = context.tbMascotas
                    .Where(m => m.Estado == true)       // si tenés campo Estado
                    .Select(m => new
                    {
                        m.IdMascota,
                        NombreMostrar = m.Nombre + " - " +
                                        m.tbClientes.Nombre + " " + m.tbClientes.Apellidos
                    })
                    .ToList();

                ViewBag.Mascotas = new SelectList(
                    mascotas,
                    "IdMascota",
                    "NombreMostrar"
                );

                //  Veterinarios activos
                var veterinarios = context.tbVeterinarios
                    .Where(v => v.Estado == true)      
                    .Select(v => new
                    {
                        v.IdVeterinario,
                        v.Nombre
                    })
                    .ToList();

                ViewBag.Veterinarios = new SelectList(
                    veterinarios,
                    "IdVeterinario",
                    "Nombre"
                );
            }
        }

    }
}
