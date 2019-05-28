using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Inventario
    {
        protected int m_idInventario;
        protected int m_idProducto;
        protected string m_Producto;
        protected string m_Color;//ASR 10-04-2018
        protected int m_idSucursal;
        protected string m_Sucursal;
        protected int m_Existencia;
        protected int m_ExistenciaMinima;
        protected string m_Descripcion;
        protected string m_Codigo;
        protected string m_CodigoBarras;
        protected int m_idPresentacion;
        protected string m_Presentacion;
        protected int m_Piezas;//ASR 17-05-2018
        protected int m_piezasTotales;
        protected int m_idProveedor; //ASR 24-05-2018
        protected string m_Proveedor;//ASR 24-05-2018

        public int idInventario { get { return m_idInventario; } set { m_idInventario = value; } }
        public int idProducto { get { return m_idProducto; } set { m_idProducto = value; } }
        public string Producto { get { return m_Producto; } set { m_Producto = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }//ASR 10-04-2018
        public int idSucursal { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string Sucursal { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public int Existencia { get { return m_Existencia; } set { m_Existencia = value; } }
        public int ExistenciaMinima { get { return m_ExistenciaMinima; } set { m_ExistenciaMinima = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }
        public string Codigo { get { return m_Codigo; } set { m_Codigo = value; } }
        public int idPresentacion { get { return m_idPresentacion; } set { m_idPresentacion = value; } }
        public string Presentacion { get { return m_Presentacion; } set { m_Presentacion = value; } }
        public int Piezas { get { return m_Piezas; } set { m_Piezas = value; } }//ASR 17-05-2018
        public int PiezasTotales { get { return m_piezasTotales; } set { m_piezasTotales = value; } }
        public int idProveedor { get { return m_idProveedor; } set { m_idProveedor = value; } }//ASR 24-05-2018
        public string Proveedor { get { return m_Proveedor; } set { m_Proveedor = value; } }//ASR 24-05-2018
        public string CodigoBarras { get { return m_CodigoBarras; } set { m_CodigoBarras = value; } }

        public Inventario() { }

        public Inventario(int PidInventario, int PidProducto, string PProducto, int PidSucursal, string PSucursal, int PExistencia, int PExistenciaMinima, string PDescripcion, string PCodigo, int PidPresentacion, string PPresentacion, string PColor, int PPiezas, int PPiezasTotales, string PCodigoBarras)//ASR 17-05-2018
        {
            m_idInventario = PidInventario;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_idSucursal = PidSucursal;
            m_Sucursal = PSucursal;
            m_Existencia = PExistencia;
            m_ExistenciaMinima = PExistenciaMinima;
            m_Descripcion = PDescripcion;
            m_Codigo = PCodigo;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
            m_Color = PColor;//ASR 10-04-2018
            m_Piezas = PPiezas;//ASR 17-05-2018
            m_piezasTotales = PPiezasTotales;
            m_CodigoBarras = PCodigoBarras;
        }

        public Inventario(int PidInventario, int PidProducto, string PProducto, int PidSucursal, string PSucursal, int PExistencia, int PExistenciaMinima, string PDescripcion, string PCodigo, int PidPresentacion, string PPresentacion, string PColor, int PPiezas, int PPiezasTotales, int PidProveedor, string PProveedor)//ASR 24-05-2018
        {
            m_idInventario = PidInventario;
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_idSucursal = PidSucursal;
            m_Sucursal = PSucursal;
            m_Existencia = PExistencia;
            m_ExistenciaMinima = PExistenciaMinima;
            m_Descripcion = PDescripcion;
            m_Codigo = PCodigo;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
            m_Color = PColor;
            m_Piezas = PPiezas;
            m_piezasTotales = PPiezasTotales;
            m_idProveedor = PidProveedor;
            m_Proveedor = PProveedor;
        }//ASR 24-05-2018
    }
}
