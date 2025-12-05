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
    public class ClienteController : Controller
    {
        public ActionResult VerClientes()
        {
            var resultado = ConsultarCliente();
            return View(resultado);

        }


        [HttpGet]
        public ActionResult AgregarCliente()
        {


            CargarClientes();
               return View(new Cliente());



        }

        [HttpPost]
        public ActionResult AgregarCliente(Cliente cliente)
        {
            // Validación del lado del servidor
            if (!ModelState.IsValid)
            {
                CargarClientes();
                return View(cliente);
            }

            try
            {
                using (var context = new BDProyecto_KNEntities())
                {
                    // Verificar si el correo ya existe
                    bool correoExiste = context.tbClientes.Any(c => c.Correo == cliente.Correo);

                    if (correoExiste)
                    {
                        ViewBag.Mensaje = "El correo ingresado ya está registrado.";
                        CargarClientes();
                        return View(cliente);
                    }

                    var nuevoCliente = new tbClientes
                    {
                        Nombre = cliente.Nombre,
                        Apellidos = cliente.Apellidos,
                        Correo = cliente.Correo,
                        Telefono = cliente.Telefono,
                        Estado = true,              // siempre activo al crear
                        FechaRegistro = DateTime.Now       // o dejar que SQL lo genere
                    };

                    context.tbClientes.Add(nuevoCliente);
                    var resultadoInsercion = context.SaveChanges();

                    if (resultadoInsercion > 0)
                    {
                        return RedirectToAction("VerClientes", "Cliente");
                    }
                }

                // Si no entró al if anterior
                ViewBag.Mensaje = "La información no se pudo registrar.";
                return View(cliente);
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Ocurrió un error: " + ex.Message;
                return View(cliente);
            }
        }




        [HttpGet]
        public ActionResult ActualizarCliente(int? q)
        {
            // Si no vino el id, redireccionamos a la lista
            if (!q.HasValue)
            {
                return RedirectToAction("VerClientes", "Cliente");
            }
            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbClientes
                    .Where(x => x.IdCliente == q)
                    .ToList();

                var datos = resultado.Select(cliente => new Cliente
                {
                    IdCliente = cliente.IdCliente,
                    Nombre = cliente.Nombre,
                    Apellidos = cliente.Apellidos,
                    Correo = cliente.Correo,
                    Telefono = cliente.Telefono

                }).FirstOrDefault();


                CargarClientes();
                return View(datos);
            }
        }

        [HttpPost]
        public ActionResult ActualizarCliente(Cliente cliente)
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var resultadoConsulta = context.tbClientes
                     .Where(x => x.IdCliente == cliente.IdCliente)
                     .FirstOrDefault();



                if(resultadoConsulta != null)
                {
                    resultadoConsulta.Nombre = cliente.Nombre;
                    resultadoConsulta.Apellidos = cliente.Apellidos;
                    resultadoConsulta.Correo = cliente.Correo;
                    resultadoConsulta.Telefono = cliente.Telefono;


                    var resultadoActualizacion = context.SaveChanges();


                    if (resultadoActualizacion > 0)
                    {
                        return RedirectToAction("VerClientes", "Cliente");
                    }




                    CargarClientes();
                    ViewBag.Mensaje = "La informacion no se ha podido actualizar, revise la informacion de nuevo.";
                    return View(cliente);
                }


               




            }


            //si no encontro el cliente 
            CargarClientes();
            ViewBag.Mensaje = "La informacion no se ha podido actualizar, revise la informacion de nuevo.";
            return View();
        }







        private void CargarClientes()
        {
            using (var context = new BDProyecto_KNEntities())
            {
                var clientes = context.tbClientes
                    .Where(cli => cli.Estado == true)
                    .Select(cli => new
                    {

                        cli.IdCliente,
                        cli.Nombre

                    }).ToList();
                ViewBag.Cliente = new SelectList(
                    clientes,
                    "IdCliente",
                    "Nombre");
            }
        }






        private List<Cliente> ConsultarCliente()
        {

            using (var context = new BDProyecto_KNEntities())
            {
                var resultado = context.tbClientes
                                       .ToList()
                                       .Select(c => new Cliente
                                       {
                                           IdCliente = c.IdCliente,
                                           Nombre = c.Nombre,
                                           Apellidos = c.Apellidos,
                                           Correo = c.Correo,
                                           Telefono = c.Telefono,
                                           // Si en la BD es nullable, cambia el tipo en el modelo a DateTime?
                                           Estado = c.Estado
                                       

                                       })
                                       .ToList();

                return resultado;
            }

        }
    }
}