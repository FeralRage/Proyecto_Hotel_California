using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Hotel_California.Models
{
    public class CreaReserva
    {
        [Display(Name = "Código de Cliente")]
        public string codCliente { get; set; }

        [Display(Name = "Fecha de Reserva")]
        public string fecReserva { get; set; }

        [Display(Name = "Días a reservar")]
        public int diasReserva { get; set; }

        [Display(Name = "Código de Habitación")]
        public string codHabitacion { get; set; }

        [Display(Name = "Número de Habitación")]
        public string numHabitacion { get; set; }

        [Display(Name = "Costo Total")]
        public double total { get; set; }

        [Display(Name = "Tipo de Tarjeta")]
        public int tipoTarjeta { get; set; }

        [Display(Name = "Número de Tarjeta EX")]
        public string numTarjetaSecure { get; set; }

        [Display(Name = "Número de Tarjeta")]
        public string numTarjeta { get; set; }

        [Display(Name = "Fecha de Vencimiento")]
        public string fecVencimiento { get; set; }

        [Display(Name = "Código de Seguridad")]
        public string codSeguridad { get; set; }
    }
}