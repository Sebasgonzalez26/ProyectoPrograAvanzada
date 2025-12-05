using System;
using System.ComponentModel.DataAnnotations;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }




       
        public string Nombre { get; set; }

        
        public string Apellidos { get; set; }


        public string Correo { get; set; }


        public string Telefono { get; set; }


          public DateTime FechaRegistro { get; set; }

      
        public bool Estado { get; set; }
    }
}
