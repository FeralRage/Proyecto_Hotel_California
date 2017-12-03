using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Hotel_California.Models
{
    public class Cliente
    {
        [Display(Name = "Código de Cliente")]
        public string codCliente { get; set; }

        [Display(Name = "Nombre de Cliente")]
        public string nomCliente { get; set; }

        [Display(Name = "Apellido de Cliente")]
        public string apeCliente { get; set; }

        [Display(Name = "Tipo de Documento")]
        public string tipoDoc { get; set; }

        [Display(Name = "Número de Documento")]
        public string numDoc { get; set; }

        [Display(Name = "Teléfono")]
        public string telefono { get; set; }

        [Display(Name = "Dirección")]
        public string direccion { get; set; }

        public int tipoTC { get; set; }

        public string numTC { get; set; }

        public string venc { get; set; }

        public string codSeg { get; set; }
    }
}