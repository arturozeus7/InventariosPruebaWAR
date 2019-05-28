using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class ListaProductos
    {
        protected int m_idListaProducto;
        protected int m_idProducto;
        protected string m_Producto;
        protected string m_Codigo;
        protected int m_idPresentacion;
        protected string m_Presentacion;
        protected int m_Cantidad;
        protected double m_CostoPrecio;
        protected double m_ImporteTotal;
        protected double m_IEPS;
        protected int m_idOrdenCVP;
        protected int m_idTransferencia;
        protected string m_Color;//ASR 24-05-2018
        protected int m_CantidadPZ; //ASR 27-11-2018

        public int idListaProductos { get { return m_idListaProducto; } set { m_idListaProducto = value; } }
        public int idProducto { get { return m_idProducto; } set { m_idProducto = value; } }
        public string Producto { get { return m_Producto; } set { m_Producto = value; } }
        public string Codigo { get { return m_Codigo; } set { m_Codigo = value; } }
        public int idPresentacion { get { return m_idPresentacion; } set { m_idPresentacion = value; } }
        public string Presentacion { get { return m_Presentacion; } set { m_Presentacion = value; } }
        public int Cantidad { get { return m_Cantidad; } set { m_Cantidad = value; } }
        public double CostoPrecio { get { return m_CostoPrecio; } set { m_CostoPrecio = value; } }
        public double ImporteTotal { get { return m_ImporteTotal; } set { m_ImporteTotal = value; } }
        public double IEPS { get { return m_IEPS; } set { m_IEPS = value; } }
        public int idOrdenCVP { get { return m_idOrdenCVP; } set { m_idOrdenCVP = value; } }
        public int idTransferencia { get { return m_idTransferencia; } set { m_idTransferencia = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }//ASR 24-05-2018
        public int CantidadPZ { get { return m_CantidadPZ; } set { m_CantidadPZ = value; } }//ASR 27-11-2018

        public ListaProductos() { }

        public ListaProductos(int PidListaProducto, int PidProducto, string PProducto, string PCodigo, int PidPresentacion, string PPresentacion, int PCantidad, double PCostoVenta,
            double PImporteTotal, double PIEPS, int PidOrdenCVP, int PidTransferencia)
        {
            m_idListaProducto = PidListaProducto;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_Codigo = PCodigo;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
            m_Cantidad = PCantidad;
            m_CostoPrecio = PCostoVenta;
            m_ImporteTotal = PImporteTotal;
            m_IEPS = PIEPS;
            m_idOrdenCVP = PidOrdenCVP;
            m_idTransferencia = PidTransferencia;
        }

        public ListaProductos(int PidListaProducto, int PidProducto, string PProducto, string PCodigo, int PidPresentacion, string PPresentacion, int PCantidad, double PCostoVenta,
            double PImporteTotal, double PIEPS, int PidOrdenCVP, int PidTransferencia, string PColor, int PCantidadPZ = 0)//ASR 24-05-2018
        {
            m_idListaProducto = PidListaProducto;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_Codigo = PCodigo;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
            m_Cantidad = PCantidad;
            m_CostoPrecio = PCostoVenta;
            m_ImporteTotal = PImporteTotal;
            m_IEPS = PIEPS;
            m_idOrdenCVP = PidOrdenCVP;
            m_idTransferencia = PidTransferencia;
            m_Color = PColor;//ASR 24-05-2018
            m_CantidadPZ = PCantidadPZ;
        }
    }
}
