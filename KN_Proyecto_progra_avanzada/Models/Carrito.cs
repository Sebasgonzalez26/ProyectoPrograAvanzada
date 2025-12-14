using System;

namespace KN_Proyecto_progra_avanzada.Models
{
    public class Carrito
    {
        public int IdCarrito { get; set; }

        public int IdUsuario { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }


        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public decimal Precio { get; set; }


    }
}
