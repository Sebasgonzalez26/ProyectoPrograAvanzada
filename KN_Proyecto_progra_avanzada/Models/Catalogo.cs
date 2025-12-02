using System;
using System.ComponentModel.DataAnnotations;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Catalogo
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public string Imagen { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; }



        public String CategoriaNombre { get; set; }
    }

}
