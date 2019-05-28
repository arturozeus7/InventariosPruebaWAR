using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Pedidos
    {
        protected int m_idPedidos;
        protected string m_Folio;
        protected int m_idCliente;
        protected string m_Cliente;
        protected int m_idProcesoFinal;
        protected string m_ProcesoFinal;
        protected DateTime m_FechaEntrega;
        protected double m_PorcentajeProcesos;
        protected double m_PorcentajeEntrega;
        protected DateTime m_FechaEntregado;
        protected string m_Descripcion;
        protected int m_idEmpleado;
        protected string m_Empleado;
        protected DateTime m_FechaPedido;

        public int idPedidos { get { return m_idPedidos; } set { m_idPedidos = value; } }
        public string Folio { get { return m_Folio; } set { m_Folio = value; } }
        public int idCliente { get { return m_idCliente; } set { m_idCliente = value; } }
        public string Cliente { get { return m_Cliente; } set { m_Cliente = value; } }
        public int idProcesoFinal { get { return m_idProcesoFinal; } set { m_idProcesoFinal = value; } }
        public string ProcesoFinal { get { return m_ProcesoFinal; } set { m_ProcesoFinal = value; } }
        public string FechaEntrega { get { return m_FechaEntrega.ToString("yyyy-MM-dd"); } set { m_FechaEntrega = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        public double PorcentajeProcesos { get { return m_PorcentajeProcesos; } set { m_PorcentajeProcesos = value; } }
        public double PorcentajeEntrega { get { return m_PorcentajeEntrega; } set { m_PorcentajeEntrega = value; } }
        public string FechaEntregado { get { return m_FechaEntregado.ToString("yyyy-MM-dd"); } set { m_FechaEntregado = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }
        public int idEmpleado { get { return m_idEmpleado; } set { m_idEmpleado = value; } }
        public string Empleado { get { return m_Empleado; } set { m_Empleado = value; } }
        public string FechaPedido { get { return m_FechaPedido.ToString("yyyy-MM-dd"); } set { m_FechaPedido = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }

        public Pedidos() { }

        public Pedidos(int PidPedidos, string PFolio, int PidCliente, string PCliente, int PidProcesoFinal, string PProcesoFinal, DateTime PFechaEntrega,
            double PPorcentajeProcesos, double PPorcentajeEntrega, DateTime PFechaEntregado, string PDescripcion, int PidEmpleado, string PEmpleado, DateTime PFechaPedido)
        {
            m_idPedidos = PidPedidos;
            m_Folio = PFolio;
            m_idCliente = PidCliente;
            m_Cliente = PCliente;
            m_idProcesoFinal = PidProcesoFinal;
            m_ProcesoFinal = PProcesoFinal;
            m_FechaEntrega = PFechaEntrega;
            m_PorcentajeProcesos = PPorcentajeProcesos;
            m_PorcentajeEntrega = PPorcentajeEntrega;
            m_FechaEntregado = PFechaEntregado;
            m_Descripcion = PDescripcion;
            m_idEmpleado = PidEmpleado;
            m_Empleado = PEmpleado;
            m_FechaPedido = PFechaPedido;
        }

    }
}