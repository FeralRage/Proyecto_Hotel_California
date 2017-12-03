using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_Hotel_California.Models;
using System.Data.Sql;
using System.Data.SqlClient;

using System.Data.Entity;
using System.Data.SqlTypes;
using System.Windows.Forms;
using System.Threading.Tasks;


namespace Proyecto_Hotel_California.Controllers
{
    public class HotelCaliforniaController : Controller
    {
        HOTEL_CALIFORNIAEntities4 conexion = new HOTEL_CALIFORNIAEntities4();


        public ActionResult PaginaPrincipal(string mensaje = null)
        {
            string url = Request.UrlReferrer == null ? "" : HttpContext.Request.UrlReferrer.PathAndQuery;

            if (!url.Contains("mensaje"))
                ViewBag.mensaje = mensaje;

            return View();
        }
        
        public ActionResult PermisoDenegado()
        {
            return View();
        }

        public ActionResult EditarDatos()
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || !logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            var usuario = conexion.TB_USUARIO.Where(c => c.NOM_USU.Equals(logueado.nomUsuario));

            TB_USUARIO usu = new TB_USUARIO();

            foreach (var item in usuario)
            {
                if (item.COD_CLI == null)
                {
                    return View("PermisoDenegado");
                }
                else
                {
                    usu.CLAVE = item.CLAVE;
                    usu.COD_CLI = item.COD_CLI;
                    usu.COD_EMP = item.COD_EMP;
                    usu.COD_USU = item.COD_USU;
                    usu.EMAIL = item.EMAIL;
                    usu.NOM_USU = item.NOM_USU;

                }
            }

            return View(usu);
        }

        [HttpPost]
        public ActionResult EditarDatos(TB_USUARIO usuario)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || !logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            SqlConnection cn = new SqlConnection("server=(local);Initial Catalog=HOTEL_CALIFORNIA; user = sa; password = sql");

            cn.Open();

            String sqlUpdate = "Update TB_USUARIO set NOM_USU='" + usuario.NOM_USU + "', CLAVE='" + usuario.CLAVE + "', EMAIL='" + usuario.EMAIL + "' WHERE COD_USU = '" + usuario.COD_USU + "'";

            SqlCommand cmd = new SqlCommand(sqlUpdate, cn);

            cmd.ExecuteNonQuery();

            // MessageBox.Show("Registro Actualizado");

            return View("PaginaPrincipal");
        }

        public ActionResult ListarHabitaciones(int? sede = 0, DateTime? fecha = null, int? dias = 0, string mensaje = null)
        {
            string url = Request.UrlReferrer == null ? "" : HttpContext.Request.UrlReferrer.PathAndQuery;

            if (!url.Contains("mensaje"))
                ViewBag.mensaje = mensaje;

            ViewBag.sedes = new SelectList(conexion.TB_SEDE.ToList(), "COD_SEDE", "DESC_SEDE");

            if (!fecha.HasValue || dias == 0 || sede == 0)
                return View(new List<Habitacion>());

            ViewBag.fecha = fecha.Value.ToShortDateString();
            ViewBag.dias = dias.Value;
            ViewBag.sede = sede.Value;

            return View(new List<Habitacion>());

        }

        [HttpPost]
        public ActionResult ListarHabitaciones(int? sede = 0, DateTime? fecha = null, int? dias = 0)
        {
            ViewBag.fecha = fecha.Value.ToShortDateString();
            ViewBag.dias = dias.Value;
            ViewBag.sede = sede.Value;

            ViewBag.sedes = new SelectList(conexion.TB_SEDE.ToList(), "COD_SEDE", "DESC_SEDE", sede);

            List<Habitacion> habitaciones = new List<Habitacion>();

            if (ModelState.IsValid)
            {
                habitaciones = (from h in conexion.TB_HABITACION
                                where h.COD_SEDE == sede &&
                                    !(from r in conexion.TB_RESERVA
                                      where DbFunctions.AddDays(fecha, dias) > r.FEC_RESERVA
                                          && fecha < DbFunctions.AddDays(r.FEC_RESERVA, r.DIAS_RESERVA)
                                      select r.COD_HAB).Contains(h.COD_HAB)
                                select new Habitacion
                                {
                                    codHab = h.COD_HAB,
                                    numHab = h.NUM_HAB,
                                    tipoHab = h.TB_TIPO_HAB.DESC_TIPO,
                                    sedeHab = h.TB_SEDE.DESC_SEDE,
                                    costoHab = (double)h.COSTO_HAB,
                                    descHab = h.DESC_HAB,
                                    estadoHab = h.TB_ESTADO_HAB.DESC_EST
                                }).ToList();
            }

            return View(habitaciones);
        }

        public ActionResult DetallesHabitacion(string id = null, int? sed = 0, int? dia = 0, DateTime? fec = null)
        {
            Habitacion habitacion = new Habitacion();

            if (dia == 0 || !fec.HasValue)
                return View(habitacion);

            ViewBag.fecha = fec.Value.ToShortDateString();
            ViewBag.dias = dia.Value;
            ViewBag.sede = sed.Value;

            var reg = conexion.TB_HABITACION.Where(h => id.Equals(h.COD_HAB)).FirstOrDefault();

            habitacion.codHab = reg.COD_HAB;
            habitacion.numHab = reg.NUM_HAB;
            habitacion.tipoHab = reg.TB_TIPO_HAB.DESC_TIPO;
            habitacion.sedeHab = reg.TB_SEDE.DESC_SEDE + " (" + reg.TB_SEDE.DIRECCION + ") ";
            habitacion.costoHab = (double)reg.COSTO_HAB;
            habitacion.descHab = reg.DESC_HAB;
            habitacion.estadoHab = reg.TB_ESTADO_HAB.DESC_EST;

            return View(habitacion);
        }

        public ActionResult CrearReserva(string id = null, int? dias = 0, string fecha = null, double? costo = 0.0, string numHab = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];
            CreaReserva creaRes = new CreaReserva();

            if (logueado == null)
                return RedirectToAction("Login", "HotelCalifornia", new { mensaje = "[ERROR] Debe iniciar sesion antes de reservar" });
            else
            {
                TB_CLIENTE cliente = new TB_CLIENTE();

                ViewBag.tipos = new SelectList(conexion.TB_TIPO_TARJ.ToList(), "COD_TIPO_TARJ", "DESC_TIPO");

                creaRes.codHabitacion = id;
                creaRes.diasReserva = (int)dias;
                creaRes.fecReserva = fecha;
                creaRes.numHabitacion = numHab;
                creaRes.total = costo.Value * dias.Value;

                if (logueado.tipoUsuario.Equals("Cliente"))
                {
                    cliente = conexion.TB_CLIENTE.Where(c => c.COD_CLI.Equals(logueado.codigo)).First();

                    if (cliente != null && !string.IsNullOrEmpty(cliente.NUMERO_TARJETA))
                    {
                        creaRes.numTarjeta = cliente.NUMERO_TARJETA;
                        creaRes.tipoTarjeta = cliente.TB_TIPO_TARJ.COD_TIPO_TARJ;
                        creaRes.fecVencimiento = cliente.FEC_VENC;
                        creaRes.codSeguridad = cliente.CVV;
                    }
                }
            }

            return View(creaRes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearReserva(CreaReserva creaReserva)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null)
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            TB_RESERVA reserva = new TB_RESERVA();

            try
            {
                reserva.COD_RES = "";
                reserva.COD_CLI = creaReserva.codCliente;
                reserva.FEC_CREACION = DateTime.Now;
                reserva.FEC_RESERVA = DateTime.Parse(creaReserva.fecReserva).Date;
                reserva.DIAS_RESERVA = creaReserva.diasReserva;
                reserva.COD_HAB = creaReserva.codHabitacion;
                reserva.COD_EST_RES = 1;

                conexion.TB_RESERVA.Add(reserva);

                conexion.SaveChanges();

                string codRes = conexion.TB_RESERVA.Where(r1 => r1.COD_CLI.Equals(reserva.COD_CLI)).OrderByDescending(r2 => r2.COD_RES).Select(r3 => r3.COD_RES).First().ToString();

                msg = "Reserva Guardada con codigo: " + codRes;
            }
            catch (Exception ex1)
            {
                MessageBox.Show("exe1");
                msg = ex1.Message;
            }

            TB_CLIENTE cliente = conexion.TB_CLIENTE.Where(c => c.COD_CLI.Equals(reserva.COD_CLI)).FirstOrDefault();

            if (cliente != null) // && string.IsNullOrEmpty(cliente.NUMERO_TARJETA)) {
            {
                try
                {
                    cliente.COD_TIPO_TARJ = creaReserva.tipoTarjeta;
                    cliente.NUMERO_TARJETA = creaReserva.numTarjeta;
                    cliente.FEC_VENC = creaReserva.fecVencimiento;
                    cliente.CVV = creaReserva.codSeguridad;

                    conexion.SaveChanges();
                }
                catch (Exception ex2)
                {
                    msg = ex2.Message;
                }
            }

            return RedirectToAction("ListarHabitaciones", "HotelCalifornia", new { mensaje = msg });
        }

        public ActionResult CrearUsuario(string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado != null)
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            ViewBag.mensaje = mensaje;

            ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO");

            return View(new CreaUsuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuario(CreaUsuario creaUsuario)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado != null)
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            TB_CLIENTE cliente = new TB_CLIENTE();

            if (conexion.TB_CLIENTE.Where(c => c.NUM_DOC.Equals(creaUsuario.numDoc)).FirstOrDefault() != null)
            {
                ViewBag.mensaje = "[ERROR] Documento ya esta registrado en nuestra lista de usuarios/clientes (si usted ya ha usado nuestros servicios anteriormente contactenos para ayudarlo) ";
                ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO", creaUsuario.codTipoDoc);

                return View(creaUsuario);
            }

            if (conexion.TB_USUARIO.Where(u => u.NOM_USU.Equals(creaUsuario.nomUsuario)).FirstOrDefault() != null)
            {
                ViewBag.mensaje = "[ERROR] El Nombre de Usuario ya ha sido registrado, elija otro";
                ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO", creaUsuario.codTipoDoc);

                return View(creaUsuario);
            }

            try
            {
                cliente.COD_CLI = "";
                cliente.NOM_CLI = creaUsuario.nomCliente.Trim();
                cliente.APE_CLI = creaUsuario.apeCliente.Trim();
                cliente.COD_TIPO_DOC = creaUsuario.codTipoDoc;
                cliente.NUM_DOC = creaUsuario.numDoc;
                cliente.TELEFONO = creaUsuario.telefono;
                cliente.DIRECCION = creaUsuario.direccion.Trim();

                conexion.TB_CLIENTE.Add(cliente);
                conexion.SaveChanges();
                msg = "Cliente Registrado";
            }
            catch (Exception ex1)
            {
                msg = ex1.Message;
            }

            TB_USUARIO usuario = new TB_USUARIO();

            if (msg.Equals("Cliente Registrado"))
            {
                try
                {
                    usuario.COD_USU = "";
                    usuario.NOM_USU = creaUsuario.nomUsuario;
                    usuario.CLAVE = creaUsuario.clave;
                    usuario.EMAIL = creaUsuario.email;
                    usuario.COD_CLI = conexion.TB_CLIENTE.Where(c => c.NUM_DOC.Equals(creaUsuario.numDoc)).First().COD_CLI;

                    conexion.TB_USUARIO.Add(usuario);
                    conexion.SaveChanges();
                    msg = "Usuario Registrado";
                }
                catch (Exception ex2)
                {
                    msg = ex2.Message;
                }
            }

            return RedirectToAction("Login", "HotelCalifornia", new { mensaje = msg });
        }

        [AllowAnonymous]
        public ActionResult Login(string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado != null)
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string url = Request.UrlReferrer == null ? "" : HttpContext.Request.UrlReferrer.PathAndQuery;
            ViewBag.mensaje = mensaje;

            if (string.IsNullOrEmpty((Session["ReturnUrl"] ?? "").ToString()) && url.Contains("HotelCalifornia"))
            {
                if (!url.Contains("CrearUsuario"))
                    Session["ReturnUrl"] = url;
                else
                    Session["ReturnUrl"] = "";
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel, string returnUrl)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado != null)
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            TB_USUARIO usuario = (TB_USUARIO)conexion.TB_USUARIO.Where(u => u.NOM_USU.Equals(loginModel.nomUsuario) && u.CLAVE.Equals(loginModel.clave)).FirstOrDefault();
            logueado = new UsuarioLog();

            if (usuario == null)
                return RedirectToAction("Login", "HotelCalifornia", new { mensaje = "[ERROR] Nombre de Usuario o Clave incorrectos" });

            if (usuario.COD_CLI != null)
            {
                var cliente = conexion.TB_CLIENTE.Where(c => c.COD_CLI.Equals(usuario.COD_CLI)).First();

                logueado.codigo = cliente.COD_CLI;
                logueado.nombre = cliente.NOM_CLI;
                logueado.apellido = cliente.APE_CLI;
                logueado.tipoDoc = cliente.TB_TIPO_DOC.DESC_TIPO;
                logueado.numDoc = cliente.NUM_DOC;
                logueado.telefono = cliente.TELEFONO;
                logueado.direccion = cliente.DIRECCION;
                logueado.email = usuario.EMAIL;
                logueado.tipoUsuario = "Cliente";
                logueado.nomUsuario = usuario.NOM_USU;
            }
            else
            {
                var empleado = conexion.TB_EMPLEADO.Where(e => e.COD_EMP.Equals(usuario.COD_EMP)).First();

                if (empleado.COD_EST_EMP == 2)
                {
                    return RedirectToAction("Login", "HotelCalifornia", new { mensaje = "[ERROR] Nombre de Usuario actualmente inactivo" });
                }

                logueado.codigo = empleado.COD_EMP;
                logueado.nombre = empleado.NOM_EMP;
                logueado.apellido = empleado.APE_EMP;
                logueado.tipoDoc = empleado.TB_TIPO_DOC.DESC_TIPO;
                logueado.numDoc = empleado.NUM_DOC;
                logueado.telefono = empleado.TELEFONO;
                logueado.direccion = empleado.DIRECCION;
                logueado.email = usuario.EMAIL;
                logueado.tipoUsuario = empleado.TB_TIPO_EMP.DESC_TIPO;
                logueado.nomUsuario = usuario.NOM_USU;
            }
            Session.Remove("ReturnUrl");
            Session["logueado"] = logueado;

            return Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("PaginaPrincipal", "HotelCalifornia");
        }

        public ActionResult Index()
        {
            return View();
        }

        //----------ACTION : VER LISTA RESERVAS REALIZADAS, DESDE EL CLIENTE 
        public ActionResult ListarReservasRealizadas()
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || !logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            List<Reserva> reservas = new List<Reserva>();
            List<TB_RESERVA> listaRes = new List<TB_RESERVA>();

            ViewBag.Tipo = "cliente";

            if (ModelState.IsValid)
            {
                listaRes = (from r in conexion.TB_RESERVA
                            where r.COD_CLI.Equals(logueado.codigo)
                            select r).ToList();
            }

            foreach (TB_RESERVA r in listaRes)
            {
                Reserva reserva = new Reserva();

                reserva.codReserva = r.COD_RES;
                reserva.fecReserva = r.FEC_RESERVA.ToLongDateString();
                reserva.diasReserva = r.DIAS_RESERVA;
                reserva.numHabitacion = r.TB_HABITACION.NUM_HAB;
                reserva.estado = r.TB_ESTADO_RES.DESC_EST;

                reservas.Add(reserva);
            }

            return View(reservas);
        }

        public ActionResult EditarReserva(string id)
        {
            TB_RESERVA reser = new TB_RESERVA();
            var reserva = conexion.TB_RESERVA;
            foreach (var item in reserva)
            {
                if (item.COD_RES == id)
                {
                    reser = item;
                    break;
                }
            }
            return View(reser);
        }
        
        public ActionResult GuardarEditarReserva(String id, String dias, String fecha)
        {
            TB_RESERVA reser = new TB_RESERVA();
            var reserva = conexion.TB_RESERVA;
            foreach (var item in reserva)
            {
                if (item.COD_RES == id)
                {

                    DateTime fecha_guarda = DateTime.Parse(fecha);
                    string strDate = fecha_guarda.ToString("yyyy/MM/dd");
                    DateTime st = DateTime.Parse(strDate);



                    SqlConnection cn = new SqlConnection("server=(local);Initial Catalog=HOTEL_CALIFORNIA; user = sa; password = Admin123");

                    cn.Open();

                    

                    String sqlUpdate = "Update TB_RESERVA set FEC_RESERVA='" + st.ToString("yyyy/MM/dd") + "', DIAS_RESERVA='" + int.Parse(dias) + "' WHERE COD_RES = '" + id + "'";

                    SqlCommand cmd = new SqlCommand(sqlUpdate, cn);

                    cmd.ExecuteNonQuery();
                    break;
                }
            }

            return RedirectToAction("ListarReservasRealizadas", "HotelCalifornia");
        }
        public ActionResult DetallesReserva(string id, string tipo = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null)
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            Reserva reserva = new Reserva();
            DateTime nullDate = DateTime.Parse("1900-01-01 00:00:00");
            tipo = tipo ?? "";

            ViewBag.NullDate = nullDate.ToString();
            ViewBag.Tipo = tipo;

            var res = conexion.TB_RESERVA.Where(r => r.COD_RES.Equals(id)).FirstOrDefault();

            int dias_falta = res.FEC_RESERVA.Date.Subtract(DateTime.Today.Date).Days;
            int dias_penal = dias_falta > 14 ? 15 : dias_falta;
            int dias_penal_real = res.DIAS_PENAL ?? (res.FEC_RESERVA.Date.Subtract(res.FEC_CREACION.Date).Days > 14 ? dias_penal : dias_penal + 15 - res.FEC_RESERVA.Date.Subtract(res.FEC_CREACION.Date).Days);
            double proporcion = res.DIAS_PENAL != null ? (double)res.TB_PENALIDAD.PROPORCION : (double)conexion.TB_PENALIDAD.Where(p => p.DIAS_PENAL == dias_penal_real).First().PROPORCION;

            reserva.codReserva = res.COD_RES;
            reserva.fecCreacion = res.FEC_CREACION.ToString();
            reserva.fecReserva = res.FEC_RESERVA.ToShortDateString();
            reserva.diasReserva = res.DIAS_RESERVA;
            reserva.numHabitacion = res.TB_HABITACION.NUM_HAB;
            reserva.tipoHabitacion = res.TB_HABITACION.TB_TIPO_HAB.DESC_TIPO;
            reserva.montoTotal = res.DIAS_RESERVA * (double)res.TB_HABITACION.COSTO_HAB;
            reserva.fecCancelacion = (res.FEC_CANCEL ?? nullDate).ToString();
            reserva.montoCobrado = (double)(res.MONTO_COBRADO ?? 0);
            reserva.fecInicio = (res.FEC_INI ?? nullDate).ToString();
            reserva.fecFin = (res.FEC_FIN ?? nullDate).ToString();
            reserva.estado = res.TB_ESTADO_RES.DESC_EST;

            if (!tipo.Equals("checkout"))
            {
                reserva.diasFaltantes = dias_falta;
                reserva.diasPenalidad = dias_penal_real;
                reserva.montoCancelacion = proporcion * reserva.montoTotal;
            }

            return View(reserva);
        }

        public ActionResult ListarClientes(string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string url = Request.UrlReferrer == null ? "" : HttpContext.Request.UrlReferrer.PathAndQuery;

            if (!url.Contains("mensaje"))
                ViewBag.mensaje = mensaje;

            return View(new List<Cliente>());
        }

        [HttpPost]
        public ActionResult ListarClientes(string cod = null, string ape = null, string doc = null, string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            List<Cliente> clientes = new List<Cliente>();
            List<TB_CLIENTE> lista = new List<TB_CLIENTE>();

            if(mensaje.Contains("buscar"))
                ViewBag.mensaje = mensaje;

            ViewBag.cod = cod;
            ViewBag.ape = ape;
            ViewBag.doc = doc;

            if (ModelState.IsValid)
            {
                if (mensaje.Equals("buscarSinUsu"))
                {
                    lista = (from c in conexion.TB_CLIENTE
                             where (c.COD_CLI.Equals(cod) || c.APE_CLI.StartsWith(string.IsNullOrEmpty(ape.Trim()) ? "xxx" : ape) || c.NUM_DOC.Equals(doc))
                                   && !(from u in conexion.TB_USUARIO
                                        where !string.IsNullOrEmpty(u.COD_CLI)
                                        select u.COD_CLI).Contains(c.COD_CLI)
                             select c).ToList();
                }
                else
                {
                    lista = (from c in conexion.TB_CLIENTE
                             where c.COD_CLI.Equals(cod) || c.APE_CLI.StartsWith(string.IsNullOrEmpty(ape) ? "xxx" : ape) || c.NUM_DOC.Equals(doc)
                             select c).ToList();
                }

                foreach (var c in lista)
                {
                    Cliente cliente = new Cliente();

                    cliente.codCliente = c.COD_CLI;
                    cliente.nomCliente = c.NOM_CLI;
                    cliente.apeCliente = c.APE_CLI;
                    cliente.tipoDoc = c.TB_TIPO_DOC.DESC_TIPO;
                    cliente.numDoc = c.NUM_DOC;
                    cliente.telefono = c.TELEFONO;
                    cliente.direccion = c.DIRECCION;

                    if (!string.IsNullOrEmpty(c.NUMERO_TARJETA))
                    {
                        cliente.tipoTC = (int)c.COD_TIPO_TARJ;
                        cliente.numTC = c.NUMERO_TARJETA;
                        cliente.venc = c.FEC_VENC;
                        cliente.codSeg = c.CVV;
                    }

                    clientes.Add(cliente);
                }
            }

            return View(clientes);
        }

        public ActionResult ListarReservasxFiltro(string cod = null, string doc = null, string ape = null, DateTime? fecha = null, string tipo = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            List<Reserva> reservas = new List<Reserva>();
            List<TB_RESERVA> listaRes = new List<TB_RESERVA>();

            ViewBag.Tipo = tipo;

            cod = cod ?? "";
            doc = doc ?? "";
            ape = ape ?? "";

            ViewBag.Cod = cod;
            ViewBag.Doc = doc;
            ViewBag.Ape = ape;

            if (fecha.HasValue)
                ViewBag.Fecha = fecha.Value.ToShortDateString();

            DateTime nullDate = DateTime.Parse("1900-01-01 00:00:00");
            fecha = fecha ?? nullDate;

            if (ModelState.IsValid)
            {
                if (tipo.Equals("checkin"))
                {
                    if (string.IsNullOrEmpty(cod) && string.IsNullOrEmpty(doc) && string.IsNullOrEmpty(ape))
                    {
                        listaRes = (from r in conexion.TB_RESERVA
                                    where DateTime.Compare(r.FEC_RESERVA, DateTime.Today.Date) == 0
                                    select r).ToList();
                    }
                    else
                    {
                        listaRes = (from r in conexion.TB_RESERVA
                                    where (r.COD_RES.Equals(cod) || r.TB_CLIENTE.NUM_DOC.Equals(doc) || r.TB_CLIENTE.APE_CLI.StartsWith(string.IsNullOrEmpty(ape) ? "xxx" : ape))
                                        && DateTime.Compare(r.FEC_RESERVA, DateTime.Today.Date) == 0
                                    select r).ToList();
                    }
                }

                if (tipo.Equals("checkout"))
                {
                    if (string.IsNullOrEmpty(cod) && string.IsNullOrEmpty(doc) && string.IsNullOrEmpty(ape))
                    {
                        listaRes = (from r in conexion.TB_RESERVA
                                    where DbFunctions.AddDays(r.FEC_RESERVA, r.DIAS_RESERVA) == DateTime.Today.Date
                                    select r).ToList();
                    }
                    else
                    {
                        listaRes = (from r in conexion.TB_RESERVA
                                    where (r.COD_RES.Equals(cod) || r.TB_CLIENTE.NUM_DOC.Equals(doc) || r.TB_CLIENTE.APE_CLI.StartsWith(string.IsNullOrEmpty(ape) ? "xxx" : ape))
                                        && DbFunctions.AddDays(r.FEC_RESERVA, r.DIAS_RESERVA) == DateTime.Today.Date
                                    select r).ToList();
                    }
                }

                if (tipo.Equals("cancel"))
                {
                    listaRes = (from r in conexion.TB_RESERVA
                                where (r.COD_RES.Equals(cod) || r.TB_CLIENTE.NUM_DOC.Equals(doc) || r.TB_CLIENTE.APE_CLI.StartsWith(string.IsNullOrEmpty(ape) ? "xxx" : ape)
                                    || (fecha >= r.FEC_RESERVA && fecha < DbFunctions.AddDays(r.FEC_RESERVA, r.DIAS_RESERVA))) && r.FEC_RESERVA >= DateTime.Today.Date
                                select r).ToList();
                }

                foreach (TB_RESERVA r in listaRes)
                {
                    Reserva reserva = new Reserva();

                    reserva.codReserva = r.COD_RES;
                    reserva.codCliente = r.TB_CLIENTE.NOM_CLI + " " + r.TB_CLIENTE.APE_CLI;
                    reserva.fecReserva = r.FEC_RESERVA.ToShortDateString();
                    reserva.diasReserva = r.DIAS_RESERVA;
                    reserva.numHabitacion = r.TB_HABITACION.NUM_HAB;
                    reserva.sede = r.TB_HABITACION.TB_SEDE.DESC_SEDE;
                    reserva.estado = r.TB_ESTADO_RES.DESC_EST;

                    reservas.Add(reserva);
                }
            }

            return View(reservas);
        }

        public ActionResult ProcesarReserva(string id, string tipo = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            TB_RESERVA reserva = conexion.TB_RESERVA.Where(r => r.COD_RES.Equals(id)).First();

            if (!string.IsNullOrEmpty(tipo))
            {
                try
                {
                    if (tipo.Equals("checkin"))
                    {
                        reserva.FEC_INI = DateTime.Now;
                        reserva.COD_EST_RES = 2;

                        conexion.SaveChanges();

                        msg = "Check-In exitoso";
                    }

                    if (tipo.Equals("checkout"))
                    {
                        reserva.MONTO_COBRADO = reserva.DIAS_RESERVA * reserva.TB_HABITACION.COSTO_HAB;
                        reserva.FEC_FIN = DateTime.Now;
                        reserva.COD_EST_RES = 3;

                        conexion.SaveChanges();

                        msg = "Check-Out exitoso";
                    }

                    if (tipo.Equals("cancel"))
                    {
                        int dias_falta = reserva.FEC_RESERVA.Date.Subtract(DateTime.Today.Date).Days;
                        int dias_penal = dias_falta > 14 ? 15 : dias_falta;
                        int dias_penal_real = reserva.FEC_RESERVA.Date.Subtract(reserva.FEC_CREACION.Date).Days > 14 ? dias_penal : dias_penal + 15 - reserva.FEC_RESERVA.Date.Subtract(reserva.FEC_CREACION.Date).Days;
                        decimal montoCobrado = (reserva.DIAS_PENAL != null ? reserva.TB_PENALIDAD.PROPORCION : conexion.TB_PENALIDAD.Where(p => p.DIAS_PENAL == dias_penal_real).First().PROPORCION) * reserva.DIAS_RESERVA * reserva.TB_HABITACION.COSTO_HAB;

                        reserva.FEC_CANCEL = DateTime.Now;
                        reserva.DIAS_PENAL = dias_penal_real;
                        //reserva.MONTO_COBRADO = reserva.TB_PENALIDAD.PROPORCION * reserva.DIAS_RESERVA * reserva.TB_HABITACION.COSTO_HAB;
                        reserva.MONTO_COBRADO = montoCobrado;
                        reserva.COD_EST_RES = 4;

                        conexion.SaveChanges();

                        msg = "Reserva Cancelada con exito ";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }

            return RedirectToAction("PaginaPrincipal", "HotelCalifornia", new { mensaje = msg });
        }

        public ActionResult CrearCliente(string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            ViewBag.mensaje = mensaje;

            ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO");

            return View(new TB_CLIENTE());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCliente(TB_CLIENTE cliente)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";
            ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO", cliente.COD_TIPO_DOC);

            if (conexion.TB_CLIENTE.Where(c => c.NUM_DOC.Equals(cliente.NUM_DOC)).FirstOrDefault() != null)
            {
                ViewBag.mensaje = "[ERROR] Numero de Documento ya esta registrado a un cliente";
                return View(cliente);
            }

            try
            {
                cliente.COD_CLI = "";

                cliente.NOM_CLI = cliente.NOM_CLI.Trim();
                cliente.APE_CLI = cliente.APE_CLI.Trim();
                cliente.DIRECCION = cliente.DIRECCION.Trim();

                conexion.TB_CLIENTE.Add(cliente);
                conexion.SaveChanges();
                msg = "Cliente Registrado";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return RedirectToAction("CrearCliente", "HotelCalifornia", new { mensaje = msg });
        }

        public ActionResult CrearEmpleado(string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || !logueado.tipoUsuario.Equals("Administrador"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            ViewBag.mensaje = mensaje;

            ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO");
            ViewBag.TiposEmp = new SelectList(conexion.TB_TIPO_EMP.ToList(), "COD_TIPO_EMP", "DESC_TIPO");
            ViewBag.Sedes = new SelectList(conexion.TB_SEDE.ToList(), "COD_SEDE", "DESC_SEDE");

            return View(new TB_EMPLEADO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEmpleado(TB_EMPLEADO empleado)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || !logueado.tipoUsuario.Equals("Administrador"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO", empleado.COD_TIPO_DOC);
            ViewBag.TiposEmp = new SelectList(conexion.TB_TIPO_EMP.ToList(), "COD_TIPO_EMP", "DESC_TIPO", empleado.COD_TIPO_EMP);
            ViewBag.Sedes = new SelectList(conexion.TB_SEDE.ToList(), "COD_SEDE", "DESC_SEDE", empleado.COD_SEDE);

            if (conexion.TB_EMPLEADO.Where(c => c.NUM_DOC.Equals(empleado.NUM_DOC)).FirstOrDefault() != null)
            {
                ViewBag.mensaje = "[ERROR] Documento ya esta registrado a otro empleado";
                return View(empleado);
            }

            try
            {
                empleado.COD_EMP = "";
                empleado.COD_EST_EMP = 1;

                empleado.NOM_EMP = empleado.NOM_EMP.Trim();
                empleado.APE_EMP = empleado.APE_EMP.Trim();
                empleado.DIRECCION = empleado.DIRECCION.Trim();

                conexion.TB_EMPLEADO.Add(empleado);
                conexion.SaveChanges();
                msg = "Empleado Registrado";
            }
            catch (Exception ex1)
            {
                msg = ex1.Message;
            }

            TB_USUARIO usuario = new TB_USUARIO();

            if (msg.Equals("Empleado Registrado"))
            {
                try
                {
                    string codEmp = conexion.TB_EMPLEADO.Where(e => e.NUM_DOC.Equals(empleado.NUM_DOC)).First().COD_EMP;
                    string nomBase = string.Concat(empleado.NOM_EMP.Substring(0, 1), empleado.APE_EMP).ToLower();
                    string nomUsu = nomBase;
                    int i = 1;

                    while (conexion.TB_USUARIO.Where(u => u.NOM_USU.Equals(nomUsu)).FirstOrDefault() != null)
                    {
                        nomUsu = string.Concat(nomBase, i.ToString());
                        i++;
                    }

                    usuario.COD_USU = "";
                    usuario.NOM_USU = nomUsu;
                    usuario.CLAVE = nomUsu;
                    usuario.EMAIL = string.Concat(nomUsu, "@hcalifornia.com");
                    usuario.COD_EMP = codEmp;

                    conexion.TB_USUARIO.Add(usuario);
                    conexion.SaveChanges();

                    msg = "Empleado registrado con codigo: " + codEmp + " y Nombre de Usuario: " + nomUsu;
                }
                catch (Exception ex2)
                {
                    msg = ex2.Message;
                }
            }

            return RedirectToAction("CrearEmpleado", "HotelCalifornia", new { mensaje = msg });
        }

        public ActionResult CrearHabitacion(string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || !logueado.tipoUsuario.Equals("Administrador"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            ViewBag.mensaje = mensaje;

            ViewBag.TiposHab = new SelectList(conexion.TB_TIPO_HAB.ToList(), "COD_TIPO_HAB", "DESC_TIPO");
            ViewBag.Sedes = new SelectList(conexion.TB_SEDE.ToList(), "COD_SEDE", "DESC_SEDE");

            return View(new TB_HABITACION());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearHabitacion(TB_HABITACION habitacion)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || !logueado.tipoUsuario.Equals("Administrador"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            ViewBag.TiposHab = new SelectList(conexion.TB_TIPO_HAB.ToList(), "COD_TIPO_HAB", "DESC_TIPO", habitacion.COD_TIPO_HAB);
            ViewBag.Sedes = new SelectList(conexion.TB_SEDE.ToList(), "COD_SEDE", "DESC_SEDE", habitacion.COD_SEDE);

            if (conexion.TB_HABITACION.Where(h => (h.NUM_HAB.Equals(habitacion.NUM_HAB)) && h.COD_SEDE == habitacion.COD_SEDE).FirstOrDefault() != null)
            {
                ViewBag.mensaje = "[ERROR] Numero de Habitacion ya esta asignado en esa Sede";
                return View(habitacion);
            }

            try
            {
                habitacion.COD_HAB = "";
                habitacion.COD_EST_HAB = 1;

                habitacion.DESC_HAB = habitacion.DESC_HAB.Trim();

                conexion.TB_HABITACION.Add(habitacion);
                conexion.SaveChanges();
                msg = "Habitacion Registrada";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return RedirectToAction("CrearHabitacion", "HotelCalifornia", new { mensaje = msg });
        }

        public ActionResult CrearUsuarioManual(string mensaje = null)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            ViewBag.mensaje = mensaje;

            return View(new TB_USUARIO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuarioManual(TB_USUARIO usuario)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            if (conexion.TB_USUARIO.Where(u => u.NOM_USU.Equals(usuario.NOM_USU)).FirstOrDefault() != null)
            {
                ViewBag.mensaje = "[ERROR] El Nombre de Usuario ya ha sido registrado, elija otro";
                usuario.COD_CLI = null;

                return View(usuario);
            }

            try
            {
                usuario.COD_USU = "";

                conexion.TB_USUARIO.Add(usuario);
                conexion.SaveChanges();
                msg = "Usuario Registrado";
            }
            catch (Exception ex2)
            {
                msg = ex2.Message;
            }

            return RedirectToAction("CrearUsuarioManual", "HotelCalifornia", new { mensaje = msg });
        }

        public ActionResult EditarCliente(string id)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            TB_CLIENTE cliente = conexion.TB_CLIENTE.Where(c => c.COD_CLI.Equals(id)).First();

            ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO", cliente.COD_TIPO_DOC);

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCliente(TB_CLIENTE registro)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            if (ModelState.IsValid)
            {
                if (conexion.TB_CLIENTE.Where(c => c.NUM_DOC.Equals(registro.NUM_DOC) && !c.COD_CLI.Equals(registro.COD_CLI)).FirstOrDefault() != null)
                {
                    ViewBag.mensaje = "[ERROR] Numero de Documento ya esta registrado a un cliente";
                    ViewBag.TiposDoc = new SelectList(conexion.TB_TIPO_DOC.ToList(), "COD_TIPO_DOC", "DESC_TIPO", registro.COD_TIPO_DOC);

                    return View(registro);
                }

                TB_CLIENTE cliente = conexion.TB_CLIENTE.Where(c => c.COD_CLI.Equals(registro.COD_CLI)).First();

                try
                {
                    cliente.NOM_CLI = registro.NOM_CLI.Trim();
                    cliente.APE_CLI = registro.APE_CLI.Trim();
                    cliente.COD_TIPO_DOC = registro.COD_TIPO_DOC;
                    cliente.NUM_DOC = registro.NUM_DOC;
                    cliente.TELEFONO = registro.TELEFONO;
                    cliente.DIRECCION = registro.DIRECCION.Trim();

                    conexion.SaveChanges();
                    msg = "Registro de Cliente Actualizado";
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }

            return RedirectToAction("ListarClientes", "HotelCalifornia", new { mensaje = msg });
        }

        public ActionResult EditarClave()
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || conexion.TB_USUARIO.Where(u => (u.COD_EMP??"").Equals(logueado.codigo)).FirstOrDefault() == null)
                RedirectToAction("Pagina Principal", "HotelCalifornia");

            return View(new ClaveModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarClave(ClaveModel registro)
        {
            UsuarioLog logueado = (UsuarioLog)Session["logueado"];

            if (logueado == null || logueado.tipoUsuario.Equals("Cliente"))
                return RedirectToAction("PaginaPrincipal", "HotelCalifornia");

            string msg = "";

            if (ModelState.IsValid)
            {
                TB_USUARIO usuario = conexion.TB_USUARIO.Where(u => u.COD_EMP.Equals(logueado.codigo)).First();

                if (!usuario.CLAVE.Equals(registro.claveAnterior))
                {
                    ViewBag.mensaje = "[ERROR] Clave Anterior es incorrecta";

                    return View(new ClaveModel());
                }

                try
                {
                    usuario.CLAVE = registro.claveNueva;

                    conexion.SaveChanges();
                    msg = "Clave Actualizada";
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }

            return RedirectToAction("PaginaPrincipal", "HotelCalifornia", new { mensaje = msg });
        }



        public ActionResult ListarEmpleados(string mensaje = null)
        {

            UsuarioLog logueado = (UsuarioLog)Session["logueado"];
            var tb_empledado = conexion.TB_EMPLEADO;
            TB_EMPLEADO empleado_logeado = new TB_EMPLEADO();

            foreach (var item in tb_empledado)
            {
                if (item.COD_EMP == logueado.codigo)
                {
                     empleado_logeado = item;
                }
            }

            List<TB_EMPLEADO> empleados = new List<TB_EMPLEADO>();
            TB_EMPLEADO empleado = new TB_EMPLEADO();

            foreach (var item in tb_empledado)
            {
                if (item.COD_TIPO_EMP == 1 && item.COD_SEDE == empleado_logeado.COD_SEDE)
                { 
                empleado = item;
                empleados.Add(empleado);
                }
            }


            return View(empleados);
        }

        public ActionResult GuardarEditarEmpleado(String id, String nombre, String apellido, String telefono, String direccion, String fecha)
        {
            
            TB_EMPLEADO emple = new TB_EMPLEADO();
            var empleado = conexion.TB_EMPLEADO;
            
            foreach (var item in empleado)
            {
                if (item.COD_EMP == id)
                {

                    DateTime fecha_guarda = DateTime.Parse(fecha);
                    string strDate = fecha_guarda.ToString("yyyy/MM/dd");
                    DateTime st = DateTime.Parse(strDate);



                    SqlConnection cn = new SqlConnection("server=(local);Initial Catalog=HOTEL_CALIFORNIA; user = sa; password = Admin123");

                    cn.Open();



                    String sqlUpdate = "Update TB_EMPLEADO set FEC_NAC='" + st.ToString("yyyy/MM/dd") 
                                                        + "', NOM_EMP='" + nombre 
                                                        + "', APE_EMP='" + apellido
                                                        + "', TELEFONO='" + telefono
                                                        + "', DIRECCION='" + direccion
                                                        + "' WHERE COD_EMP = '" + id + "'";

                    SqlCommand cmd = new SqlCommand(sqlUpdate, cn);

                    cmd.ExecuteNonQuery();
                    break;
                }
            }

            return RedirectToAction("ListarEmpleados", "HotelCalifornia");
        }

        public ActionResult EditarEmpleado(string id)
        {
            var tb_empleado = conexion.TB_EMPLEADO;
            TB_EMPLEADO empleado = new TB_EMPLEADO();

            foreach (var item in tb_empleado)
            {
                if (item.COD_EMP == id)
                {
                    empleado = item;
                }
            }

            return View(empleado);
        }
    }
}
