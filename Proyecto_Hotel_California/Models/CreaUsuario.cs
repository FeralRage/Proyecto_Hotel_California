using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Hotel_California.Models
{
    public class CreaUsuario
    {
        [Display(Name = "Nombre")]
        public string nomCliente { get; set; }

        [Display(Name = "Apellido")]
        public string apeCliente { get; set; }

        [Display(Name = "Tipo de Documento")]
        public int codTipoDoc { get; set; }

        [Display(Name = "Número de Documento")]
        public string numDoc { get; set; }

        [Display(Name = "Teléfono")]
        public string telefono { get; set; }

        [Display(Name = "Dirección")]
        public string direccion { get; set; }

        [Display(Name = "Nombre de Usuario")]
        public string nomUsuario { get; set; }

        [Display(Name = "Contraseña")]
        public string clave { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string email { get; set; }
    }
}