using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Precios
    {
        protected int m_idPrecio;
        protected int m_idProducto;
        protected string m_Producto;
        protected string m_EtiquetaPrecio;
        protected int m_Minimo;
        protected int m_Maximo;
        protected double m_Precio;
        protected int m_idSucursal;
        protected string m_Sucursal;
        protected string m_Color;
        protected string m_Presentacion;

        public int idPrecio { get { return m_idPrecio; } set { m_idPrecio = value; } }
        public int idProducto { get { return m_idProducto; } set { m_idProducto = value; } }
        public string Producto { get { return m_Producto; } set { m_Producto = value; } }
        public string EtiquetaPrecio { get { return m_EtiquetaPrecio; } set { m_EtiquetaPrecio = value; } }
        public int Minimo { get { return m_Minimo; } set { m_Minimo = value; } }
        public int Maxima { get { return m_Maximo; } set { m_Maximo = value; } }
        public double Precio { get { return m_Precio; } set { m_Precio = value; } }
        public int idSucursal { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string Sucursal { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }
        public string Presentacion { get { return m_Presentacion; } set { m_Presentacion = value; } }

        public Precios() { }

        public Precios(int PidPrecio, int PidProducto, string PProducto, string PEtiqueta, int PMinimo, int PMaximo, double PPrecio, int PidSucursal, string PSucursal, string PColor, string PPresentacion)
        {
            m_idPrecio = PidPrecio;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_EtiquetaPrecio = PEtiqueta;
            m_Minimo = PMinimo;
            m_Maximo = PMaximo;
            m_Precio = PPrecio;
            m_idSucursal = PidSucursal;
            m_Sucursal = PSucursal;
            m_Color = PColor;
            m_Presentacion = PPresentacion;
        }
    }
}
