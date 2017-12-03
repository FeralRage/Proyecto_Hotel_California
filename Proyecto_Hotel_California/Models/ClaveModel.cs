using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Hotel_California.Models
{
    public class ClaveModel
    {
        [Display(Name = "Contraseña Anterior")]
        public string claveAnterior { get; set; }

        [Display(Name = "Contraseña Nueva")]
        public string claveNueva { get; set; }
    }
}