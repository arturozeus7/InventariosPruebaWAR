using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class InventarioMov
    {
        protected int m_idInventarioMov;
        protected int m_idInventario;
        protected int m_idProducto;
        protected string m_Producto;
        protected int m_idPresentacion;
        protected string m_Presentacion;
        protected string m_Color;
        protected int m_idSucursal;
        protected string m_Sucursal;
        protected int m_Cantidad;
        protected int m_CantidadActual;
        protected int m_CantidadNueva;
        protected string m_Movimiento;
        protected DateTime m_Fecha;

        public int idInventarioMov { get { return m_idInventarioMov; } set { m_idInventarioMov = value; } }
        public int idInventario { get { return m_idInventario; } set { m_idInventario = value; } }
        public int idProducto { get { return m_idProducto; } set { m_idProducto = value; } }
        public string Producto { get { return m_Producto; } set { m_Producto = value; } }
        public int idPresentacion { get { return m_idPresentacion; } set { m_idPresentacion = value; } }
        public string Presentacion { get { return m_Presentacion; } set { m_Presentacion = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }
        public int idSucursal { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string Sucursal { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public int Cantidad { get { return m_Cantidad; } set { m_Cantidad = value; } }
        public int CantidadActual { get { return m_CantidadActual; } set { m_CantidadActual = value; } }
        public int CantidadNueva { get { return m_CantidadNueva; } set { m_CantidadNueva = value; } }
        public string Movimiento { get { return m_Movimiento; } set { m_Movimiento = value; } }
        public string Fecha
        {
            get { return m_Fecha.ToString("yyyy-MM-dd HH:mm"); }
            set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture); }
        }

        public InventarioMov() { }

        public InventarioMov(int PidInventarioMov, int PidInventario, int PidProducto, string PProducto, int PidSucursal, string PSucursal, string PColor, int PCantidad, int PCantidadActual, int PCantidadNueva, string PMovimiento, DateTime PFecha, int PidPresentacion, string PPresentacion)
        {
            m_idInventarioMov = PidInventarioMov;
            m_idInventario = PidInventario;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_idSucursal = PidSucursal;
            m_Sucursal = PSucursal;
            m_Color = PColor;
            m_Cantidad = PCantidad;
            m_CantidadActual = PCantidadActual;
            m_CantidadNueva = PCantidadNueva;
            m_Movimiento = PMovimiento;
            m_Fecha = PFecha;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
        }
    }
}
