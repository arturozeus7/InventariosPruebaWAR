using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Productos
    {
        protected int m_idProducto;
        protected string m_Producto;
        protected string m_Descripcion;
        protected string m_Codigo;
        protected string m_SKU;
        protected int m_idPresentacion;
        protected string m_Presentacion;
        protected string m_imagenP = "";
        protected int m_piezas;
        protected int m_piezasTotales;
        protected string m_Color;
        protected int m_activo = 1;
        protected string m_CodigoBarras;

        public int idProducto { get { return m_idProducto; } set { m_idProducto = value; } }
        public int Piezas { get { return m_piezas; } set { m_piezas = value; } }
        public int PiezasTotales { get { return m_piezasTotales; } set { m_piezasTotales = value; } }
        public string Producto { get { return m_Producto; } set { m_Producto = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }
        public string Codigo { get { return m_Codigo; } set { m_Codigo = value; } }
        public string CodigoBarras { get { return m_CodigoBarras; } set { m_CodigoBarras = value; } }
        public string SKU { get { return m_SKU; } set { m_SKU = value; } }
        public int idPresentacion { get { return m_idPresentacion; } set { m_idPresentacion = value; } }
        public string Presentacion { get { return m_Presentacion; } set { m_Presentacion = value; } }
        public string ImagenP { get { return m_imagenP; } set { m_imagenP = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }
        public int Activo { get { return m_activo; } set { m_activo = value; } }

        public Productos() { }

        public Productos(int PidProducto, string PProducto, string PDescripcion, string PCodigo, string PCodigoBarras, string PSKU, int PidPresentacion, string PPresentacion, string PImagen, int PPiezas, int PPiezasTotales, string PColor)
        {
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_Descripcion = PDescripcion;
            m_Codigo = PCodigo;
            m_CodigoBarras = PCodigoBarras;
            m_SKU = PSKU;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
            m_imagenP = PImagen;
            m_piezas = PPiezas;
            m_piezasTotales = PPiezasTotales;
            m_Color = PColor;
        }
    }

    public class ProductosCosto : Productos
    {
        protected int m_idProveedor;
        protected string m_Proveedor;
        protected double m_Costo;
        public int idProveedor { get { return m_idProveedor; } set { m_idProveedor = value; } }
        public string Proveedor { get { return m_Proveedor; } set { m_Proveedor = value; } }
        public double Costo { get { return m_Costo; } set { m_Costo = value; } }

        public ProductosCosto() { }

        public ProductosCosto(int PidProducto, string PProducto, string PDescripcion, string PCodigo, string PCodigoBarras, string PSKU, int PidPresentacion, string PPresentacion, string PImagen, int PPiezas, int PPiezasTotales, string PColor)
        {
            m_idProducto = PidProducto;
            m_Producto = PProducto;
            m_Descripcion = PDescripcion;
            m_Codigo = PCodigo;
            m_SKU = PSKU;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
            m_imagenP = PImagen;
            m_piezas = PPiezas;
            m_Color = PColor;
            m_CodigoBarras = PCodigoBarras;
            m_piezasTotales = PPiezasTotales;
        }
    }

    public class ProductosCostoRosedal : ProductosCosto
    {
        protected int m_cajasinternas;
        protected int m_idProductoCaja;

        public int idProductoCaja
        {
            get
            {
                return m_idProductoCaja;
            }
            set
            {
                m_idProductoCaja = value;
            }
        }

        public int CajasInternas
        {
            get
            {
                return m_cajasinternas;
            }
            set
            {
                m_cajasinternas = value;
            }
        }
        public ProductosCostoRosedal() { }
        public ProductosCostoRosedal(int PidProducto, int PidProductoCaja, string PProducto, string PDescripcion, string PCodigo, string PCodigoBarras, string PSKU, int PidPresentacion, string PPresentacion, string PImagen, int PPiezas, int PPiezasTotales, string PColor, int cajasInternas)

        {
            m_idProducto = PidProducto;
            m_idProductoCaja = PidProductoCaja;
            m_Producto = PProducto;
            m_Descripcion = PDescripcion;
            m_Codigo = PCodigo;
            m_SKU = PSKU;
            m_idPresentacion = PidPresentacion;
            m_Presentacion = PPresentacion;
            m_imagenP = PImagen;
            m_piezas = PPiezas;
            m_Color = PColor;
            m_CodigoBarras = PCodigoBarras;
            m_cajasinternas = cajasInternas;
            m_piezasTotales = PPiezasTotales;
        }

    }
    public class InventarioIngesta : ProductosCostoRosedal
    {
        protected int m_idInventario;
        protected int m_idSucursal;
        protected string m_Sucursal;
        protected int m_Existencia;
        protected int m_ExistenciaMinima;
        protected string m_nuevo;

        public int idInventario { get { return m_idInventario; } set { m_idInventario = value; } }
        public int idAlmacen { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string Almacen { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public int Existencia { get { return m_Existencia; } set { m_Existencia = value; } }
        public int ExistenciaMinima { get { return m_ExistenciaMinima; } set { m_ExistenciaMinima = value; } }
        public string Nuevo { get { return m_nuevo; } set { m_nuevo = value; } }
        public InventarioIngesta() { }
    }
}