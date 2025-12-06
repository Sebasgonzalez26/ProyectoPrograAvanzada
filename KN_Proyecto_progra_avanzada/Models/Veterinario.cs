using System;
using System.ComponentModel.DataAnnotations;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Veterinario
    {
        public int IdVeterinario { get; set; }

        public string Nombre { get; set; }

        
        public string Apellidos { get; set; }

      
        public string Correo { get; set; }


        public string Telefono { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        // OPCIONAL: Nombre completo para mostrar en tablas o desplegables
        public string NombreCompleto
        {
            get { return $"{Nombre} {Apellidos}"; }
        }
    }
}
