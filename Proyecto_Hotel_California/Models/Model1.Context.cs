﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_Hotel_California.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HOTEL_CALIFORNIAEntities4 : DbContext
    {
        public HOTEL_CALIFORNIAEntities4()
            : base("name=HOTEL_CALIFORNIAEntities4")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TB_CLIENTE> TB_CLIENTE { get; set; }
        public virtual DbSet<TB_EMPLEADO> TB_EMPLEADO { get; set; }
        public virtual DbSet<TB_ESTADO_EMP> TB_ESTADO_EMP { get; set; }
        public virtual DbSet<TB_ESTADO_HAB> TB_ESTADO_HAB { get; set; }
        public virtual DbSet<TB_ESTADO_RES> TB_ESTADO_RES { get; set; }
        public virtual DbSet<TB_HABITACION> TB_HABITACION { get; set; }
        public virtual DbSet<TB_PENALIDAD> TB_PENALIDAD { get; set; }
        public virtual DbSet<TB_RESERVA> TB_RESERVA { get; set; }
        public virtual DbSet<TB_SEDE> TB_SEDE { get; set; }
        public virtual DbSet<TB_TIPO_DOC> TB_TIPO_DOC { get; set; }
        public virtual DbSet<TB_TIPO_EMP> TB_TIPO_EMP { get; set; }
        public virtual DbSet<TB_TIPO_HAB> TB_TIPO_HAB { get; set; }
        public virtual DbSet<TB_TIPO_TARJ> TB_TIPO_TARJ { get; set; }
        public virtual DbSet<TB_USUARIO> TB_USUARIO { get; set; }
    }
}
