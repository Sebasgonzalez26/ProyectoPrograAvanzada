using System;
using System.ComponentModel.DataAnnotations;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Inventario
    {
        public int IdInventario { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string Categoria { get; set; }

        public int Stock { get; set; }

        public decimal? PrecioVenta { get; set; }

        public string Unidad { get; set; }

        public string Imagen { get; set; }

        public DateTime FechaIngreso { get; set; }

        public bool Estado { get; set; }
    }
}
