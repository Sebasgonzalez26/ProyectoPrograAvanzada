using System;
using System.ComponentModel.DataAnnotations;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Mascota
    {
        public int IdMascota { get; set; }

        public string Nombre { get; set; }

        public string Especie { get; set; }

        public string Raza { get; set; }

        public string Sexo { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public int IdCliente { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Estado { get; set; }

        // opcional: para mostrar el nombre del dueño sin hacer join en la vista
        public string NombreCliente { get; set; }
    }
}
