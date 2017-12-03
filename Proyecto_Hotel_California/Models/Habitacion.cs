using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Hotel_California.Models
{
    public class Habitacion
    {
        [Display(Name = "Código de Habitación")]
        public string codHab { get; set; }

        [Display(Name = "Número de Habitación")]
        public string numHab { get; set; }

        [Display(Name = "Tipo de Habitación")]
        public string tipoHab { get; set; }

        [Display(Name = "Sede")]
        public string sedeHab { get; set; }

        [Display(Name = "Costo por Día")]
        public double costoHab { get; set; }

        [Display(Name = "Descripcion")]
        public string descHab { get; set; }

        [Display(Name = "Estado de Habitación")]
        public string estadoHab { get; set; }
    }
}