using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class PreciosHistoricos
    {
        protected int m_idPrecio;
        protected DateTime m_fecha;
        protected int m_idProducto;
        protected string m_Producto;
        protected double m_Precio;
        protected int m_idSucursal;
        protected string m_Sucursal;

        public int idPrecio { get { return m_idPrecio; } set { m_idPrecio = value; } }
        public int idProducto { get { return m_idProducto; } set { m_idProducto = value; } }
        public string Producto { get { return m_Producto; } set { m_Producto = value; } }
        public double Precio { get { return m_Precio; } set { m_Precio = value; } }
        public int idSucursal { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string Sucursal { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public string Fecha { get { return m_fecha.ToString("yyyy-MM-dd"); } set { m_fecha = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }

        public PreciosHistoricos() { }

        public PreciosHistoricos(int PidPrecio, int PidProducto, string PProducto, double PPrecio, int PidSucursal, string PSucursal, DateTime PFecha)
        {
            m_idPrecio = PidPrecio;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_Precio = PPrecio;
            m_idSucursal = PidSucursal;
            m_Sucursal = PSucursal;
            m_fecha = PFecha;
        }
    }
}
