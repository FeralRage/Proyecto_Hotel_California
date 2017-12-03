using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Hotel_California.Models
{
    public class UsuarioLog
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string tipoDoc { get; set; }
        public string numDoc { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string tipoUsuario { get; set; }
        public string nomUsuario { get; set; }
    }
}