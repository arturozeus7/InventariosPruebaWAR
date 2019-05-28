using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Costos
    {
        protected int m_idCosto;
        protected int m_idProducto;
        protected string m_Producto;
        protected int m_idProveedor;
        protected string m_Proveedor;
        protected double m_Costo;

        public int idCosto { get { return m_idCosto; } set { m_idCosto = value; } }
        public int idProducto { get { return m_idProducto; } set { m_idProducto = value; } }
        public string Producto { get { return m_Producto; } set { m_Producto = value; } }
        public int idProveedor { get { return m_idProveedor; } set { m_idProveedor = value; } }
        public string Proveedor { get { return m_Proveedor; } set { m_Proveedor = value; } }
        public double Costo { get { return m_Costo; } set { m_Costo = value; } }

        public Costos() { }

        public Costos(int PidCosto, int PidProducto, string PProducto, int PidProveedor, string PProveedor, double PCosto)
        {
            m_idCosto = PidCosto;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_idProveedor = PidProveedor;
            m_Proveedor = PProveedor;
            m_Costo = PCosto;
        }
    }
}
