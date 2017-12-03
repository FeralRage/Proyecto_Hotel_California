using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Hotel_California.Models
{
    public class Reserva
    {
        [Display(Name = "Código de Reserva")]
        public string codReserva { get; set; }

        [Display(Name = "Cliente")]
        public string codCliente { get; set; }

        [Display(Name = "Fecha de Creación")]
        public string fecCreacion { get; set; }

        [Display(Name = "Fecha de Reserva")]
        public string fecReserva { get; set; }

        [Display(Name = "Días de Reserva")]
        public int diasReserva { get; set; }

        [Display(Name = "Código de Habitación")]
        public string codHabitacion { get; set; }

        [Display(Name = "Número de Habitación")]
        public string numHabitacion { get; set; }

        [Display(Name = "Tipo de Habitación")]
        public string tipoHabitacion { get; set; }

        [Display(Name = "Sede")]
        public string sede { get; set; }

        [Display(Name = "Monto Total")]
        public double montoTotal { get; set; }

        [Display(Name = "Fecha de Cancelación")]
        public string fecCancelacion { get; set; }

        [Display(Name = "Días hasta la Reserva")]
        public int diasFaltantes { get; set; }

        [Display(Name = "Días de Penalidad")]
        public int diasPenalidad { get; set; }
        
        [Display(Name = "Penalidad al Cancelar")]
        public double montoCancelacion { get; set; }

        [Display(Name = "Monto Cobrado")]
        public double montoCobrado { get; set; }

        [Display(Name = "Inicio de Estadía")]
        public string fecInicio { get; set; }

        [Display(Name = "Fin de Estadía")]
        public string fecFin { get; set; }

        [Display(Name = "Estado")]
        public string estado { get; set; }
    }
}