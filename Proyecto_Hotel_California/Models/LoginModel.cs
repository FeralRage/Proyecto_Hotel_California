using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Hotel_California.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string nomUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string clave { get; set; }
    }
}