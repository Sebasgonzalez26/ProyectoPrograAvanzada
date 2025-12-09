using System;
using System.ComponentModel.DataAnnotations;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Cita
    {
        public int IdCita { get; set; }

        // ----------- Relaciones ----------
        public int IdMascota { get; set; }

        public int IdVeterinario { get; set; }

        // Estos nombres son solo para mostrar en el VerCitas (no se guardan en BD)
        public string NombreMascota { get; set; }
        public string NombreCliente { get; set; }
        public string NombreVeterinario { get; set; }

        // ----------- Datos de la cita ----------
        public DateTime FechaCita { get; set; }

     
        public string Motivo { get; set; }

        public string Observaciones { get; set; }

        // Estado: Programada / Completada / Cancelada
        public string Estado { get; set; }

        // Fecha en que se registró la cita
        public DateTime FechaRegistro { get; set; }
    }
}
