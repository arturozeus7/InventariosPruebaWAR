using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class OrdenesCVP
    {
        protected int m_idOrdenCVP;
        protected int m_idProveedor;
        protected int m_idCliente;
        protected string m_Empresa;
        protected int m_idSucursal;
        protected string m_Sucursal;
        protected int m_idSerie;
        protected string m_Prefijo;
        protected double m_NumeroSerie;
        protected int m_idMoneda;
        protected string m_Moneda;
        protected int m_DiasCredito;
        protected DateTime m_Fecha;
        protected string m_Comentarios;
        protected int m_idEmpleado;
        protected string m_Empleado;
        protected double m_Descuento;
        protected int m_idOperacion;
        protected string m_Operacion;
        protected int m_idIVA;
        protected double m_IVA;
        protected int m_idEstatus;
        protected string m_Estatus;
        protected int m_idEmpleadoAprobar;
        protected string m_EmpleadoAprobar;
        protected double m_ValorDescuento;
        protected double m_ValorIVA;
        protected double m_Subtotal;
        protected double m_Total;
        protected string m_folio;
        protected string m_Dirigido;
        protected List<ListaProductos> m_Productos;//ASR 27-11-2018

        public int idOrdenCVP { get { return m_idOrdenCVP; } set { m_idOrdenCVP = value; } }
        public int idProveedor { get { return m_idProveedor; } set { m_idProveedor = value; } }
        public int idCliente { get { return m_idCliente; } set { m_idCliente = value; } }
        public string Empresa { get { return m_Empresa; } set { m_Empresa = value; } }
        public int idSucursal { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string Sucursal { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public int idSerie { get { return m_idSerie; } set { m_idSerie = value; } }
        public string Prefijo { get { return m_Prefijo; } set { m_Prefijo = value; } }
        public double NumeroSerie { get { return m_NumeroSerie; } set { m_NumeroSerie = value; } }
        public int idMoneda { get { return m_idMoneda; } set { m_idMoneda = value; } }
        public string Moneda { get { return m_Moneda; } set { m_Moneda = value; } }
        public int DiasCredito { get { return m_DiasCredito; } set { m_DiasCredito = value; } }
        public string Fecha
        {
            get { return m_Fecha.ToString("yyyy-MM-dd HH:mm"); }
            set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture); }
        }
        public string Comentarios { get { return m_Comentarios; } set { m_Comentarios = value; } }
        public int idEmpleado { get { return m_idEmpleado; } set { m_idEmpleado = value; } }
        public string Empleado { get { return m_Empleado; } set { m_Empleado = value; } }
        public double Descuento { get { return m_Descuento; } set { m_Descuento = value; } }
        public int idOperacion { get { return m_idOperacion; } set { m_idOperacion = value; } }
        public string Operacion { get { return m_Operacion; } set { m_Operacion = value; } }
        public int idIVA { get { return m_idIVA; } set { m_idIVA = value; } }
        public double IVA { get { return m_IVA; } set { m_IVA = value; } }
        public int idEstatus { get { return m_idEstatus; } set { m_idEstatus = value; } }
        public string Estatus { get { return m_Estatus; } set { m_Estatus = value; } }
        public int idEmpleadoAprobar { get { return m_idEmpleadoAprobar; } set { m_idEmpleadoAprobar = value; } }
        public string EmpleadoAprobar { get { return m_EmpleadoAprobar; } set { m_EmpleadoAprobar = value; } }
        public double ValorDescuento { get { return m_ValorDescuento; } set { m_ValorDescuento = value; } }
        public double ValorIVA { get { return m_ValorIVA; } set { m_ValorIVA = value; } }
        public double Subtotal { get { return m_Subtotal; } set { m_Subtotal = value; } }
        public double Total { get { return m_Total; } set { m_Total = value; } }
        public string Folio { get { return m_folio; } set { m_folio = value; } }
        public string Dirigido { get { return m_Dirigido; } set { m_Dirigido = value; } }
        public List<ListaProductos> Productos { get { return m_Productos; } set { m_Productos = value; } }//ASR 27-11-2018

        public OrdenesCVP() { }

        public OrdenesCVP(int PidOrdenCVP, int PidProveedor, int PidCliente, string PEmpresa, int PidSucursal, string PSucursal, int PidSerie, string PPrefijo, double PNumeroSerie,
            int PidMoneda, string PMoneda, int PDiasCredito, DateTime PFecha, string PComentarios, int PidEmpleado, string PEmpleado, double PDescuento, int PidOperacion,
            string POperacion, int PidIVA, double PIVA, int PidEstatus, string PEstatus, int PidEmpleado2, string PEmpleado2, double PValorDescuento, double PValorIVA, double PSubtotal, double PTotal, string PFolio, string PDirigido, List<ListaProductos> PProductos = null)
        {
            m_idOrdenCVP = PidOrdenCVP;
            m_idProveedor = PidProveedor;
            m_idCliente = PidCliente;
            m_Empresa = PEmpresa;
            m_idSucursal = PidSucursal;
            m_Sucursal = PSucursal;
            m_idSerie = PidSerie;
            m_Prefijo = PPrefijo;
            m_NumeroSerie = PNumeroSerie;
            m_idMoneda = PidMoneda;
            m_Moneda = PMoneda;
            m_DiasCredito = PDiasCredito;
            m_Fecha = PFecha;
            m_Comentarios = PComentarios;
            m_idEmpleado = PidEmpleado;
            m_Empleado = PEmpleado;
            m_Descuento = PDescuento;
            m_idOperacion = PidOperacion;
            m_Operacion = POperacion;
            m_idIVA = PidIVA;
            m_IVA = PIVA;
            m_idEstatus = PidEstatus;
            m_Estatus = PEstatus;
            m_idEmpleadoAprobar = PidEmpleado2;
            m_EmpleadoAprobar = PEmpleado2;
            m_ValorDescuento = PValorDescuento;
            m_ValorIVA = PValorIVA;
            m_Subtotal = PSubtotal;
            m_Total = PTotal;
            m_folio = PFolio;
            m_Dirigido = PDirigido;
            m_Productos = PProductos;//ASR 27-11-2018
        }

    }
}
