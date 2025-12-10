using System;
using System.ComponentModel.DataAnnotations;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Cita
    {
        public int IdCita { get; set; }

        // ----------- Relaciones ----------
        public int IdMascota { get; set; }
        //----------------------------------
        public int IdVeterinario { get; set; }

        public string NombreMascota { get; set; }
        public string NombreCliente { get; set; }
        public string NombreVeterinario { get; set; }

        public DateTime FechaCita { get; set; }

     
        public string Motivo { get; set; }

        public string Observaciones { get; set; }

        // Estado: Programada / Completada / Cancelada
        public string Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
