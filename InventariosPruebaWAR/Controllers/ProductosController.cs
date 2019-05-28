using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    //[Authorize]
    public class ProductosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Productos
        public List<ProductosCosto> Get()
        {
            List<ProductosCosto> Productos = ClassBD.ObtenerProductosRosedal();
            CostosHistoricos CostoProducto = new CostosHistoricos();
            foreach (ProductosCosto Producto in Productos)
            {
                CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                Producto.idProveedor = CostoProducto.idProveedor;
                Producto.Proveedor = CostoProducto.Proveedor;
                Producto.Costo = CostoProducto.Costo;
            }
            return Productos;
        }

        // GET: api/Productos/5
        public ProductosCosto Get(int id)
        {
            ProductosCosto Productos = ClassBD.ObtenerProductoById(id);
            CostosHistoricos CostoProducto = new CostosHistoricos();

            CostoProducto = ClassBD.ObtenerCostosHbyProduct(Productos.idProducto);
            Productos.idProveedor = CostoProducto.idProveedor;
            Productos.Proveedor = CostoProducto.Proveedor;
            Productos.Costo = CostoProducto.Costo;

            return Productos;
        }

        [Route("api/Productos/Busqueda/{consulta}")]
        public List<ProductosCosto> Get(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            List<ProductosCosto> Productos = ClassBD.ObtenerProductosBusquedaDinamica(consulta);
            CostosHistoricos CostoProducto = new CostosHistoricos();
            foreach (ProductosCosto Producto in Productos)
            {
                CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                Producto.idProveedor = CostoProducto.idProveedor;
                Producto.Proveedor = CostoProducto.Proveedor;
                Producto.Costo = CostoProducto.Costo;
            }
            return Productos;
        }

        [Route("api/Productos/Proveedor/{idProveedor}/Busqueda/{consulta}")]
        public List<ProductosCosto> GetObtenerBusquedaProductosByProveedor(string idProveedor, string consulta)
        {
            consulta = consulta.Replace("'", "-");
            List<ProductosCosto> Productos = ClassBD.ObtenerBusquedaProductosByProveedor(Convert.ToInt32(idProveedor), consulta);
            CostosHistoricos CostoProducto = new CostosHistoricos();
            foreach (ProductosCosto Producto in Productos)
            {
                CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                Producto.idProveedor = CostoProducto.idProveedor;
                Producto.Proveedor = CostoProducto.Proveedor;
                Producto.Costo = CostoProducto.Costo;
            }
            return Productos;
        }

        [Route("api/Productos/Rosedal/{idProducto}")]
        public ProductosCostoRosedal GetObtenerProductoRosedalById(string idProducto)
        {
            ProductosCostoRosedal ret = new ProductosCostoRosedal();
            ProductosCosto Productos = ClassBD.ObtenerProductoById(Convert.ToInt32(idProducto));
            ProductosCosto ProductoCaja = ClassBD.ObtenerProductoCajaInterna(Productos);

            /*
            CostosHistoricos CostoProducto = new CostosHistoricos();
            CostoProducto = ClassBD.ObtenerCostosHbyProduct(Productos.idProducto);*/
            Costos CostoProducto = ClassBD.ObtenerCostoByIdProducto(Productos.idProducto);

            ret.Producto = Productos.Producto;
            ret.CajasInternas = Productos.Piezas;
            ret.idProducto = Productos.idProducto;
            ret.Codigo = Productos.Codigo;
            ret.CodigoBarras = Productos.CodigoBarras;
            ret.Color = Productos.Color;
            ret.Descripcion = Productos.Descripcion;
            ret.SKU = Productos.SKU;
            ret.idPresentacion = 3;
            ret.ImagenP = Productos.ImagenP;


            ret.idProductoCaja = ProductoCaja.idProducto;
            ret.Piezas = ProductoCaja.Piezas;

            ret.idProveedor = CostoProducto.idProveedor;
            ret.Proveedor = CostoProducto.Proveedor;
            ret.Costo = CostoProducto.Costo;

            return ret;
        }

        [Route("api/Productos/Codigo/{Codigo}")]
        public ProductosCosto GetObtenerProductoByCodigo(string Codigo)
        {
            Codigo = Codigo.Replace("'", "-");
            ProductosCosto Productos = ClassBD.ObtenerProductoByCodigo(Codigo);
            CostosHistoricos CostoProducto = new CostosHistoricos();

            CostoProducto = ClassBD.ObtenerCostosHbyProduct(Productos.idProducto);
            Productos.idProveedor = CostoProducto.idProveedor;
            Productos.Proveedor = CostoProducto.Proveedor;
            Productos.Costo = CostoProducto.Costo;

            return Productos;
        }

        [Route("api/Productos/Barras/{Codigo}")]
        public ProductosCosto GetObtenerProductoByBarras(string Codigo)
        {
            Codigo = Codigo.Replace("'", "-");
            ProductosCosto Productos = ClassBD.ObtenerProductoByBarras(Codigo);
            CostosHistoricos CostoProducto = new CostosHistoricos();

            CostoProducto = ClassBD.ObtenerCostosHbyProduct(Productos.idProducto);
            Productos.idProveedor = CostoProducto.idProveedor;
            Productos.Proveedor = CostoProducto.Proveedor;
            Productos.Costo = CostoProducto.Costo;

            return Productos;
        }

        [Route("api/Productos/SKU/{SKU}")]
        public ProductosCosto GetObtenerProductoBySKU(string SKU)
        {
            ProductosCosto Productos = ClassBD.ObtenerProductoByCodigo(SKU);
            CostosHistoricos CostoProducto = new CostosHistoricos();

            CostoProducto = ClassBD.ObtenerCostosHbyProduct(Productos.idProducto);
            Productos.idProveedor = CostoProducto.idProveedor;
            Productos.Proveedor = CostoProducto.Proveedor;
            Productos.Costo = CostoProducto.Costo;

            return Productos;
        }

        [Route("api/Productos/Nombre/{nombre}")]
        public List<ProductosCosto> GetObtenerProductosByName(string nombre)
        {
            List<ProductosCosto> Productos = ClassBD.ObtenerProductosByName(nombre);
            CostosHistoricos CostoProducto = new CostosHistoricos();
            foreach (ProductosCosto Producto in Productos)
            {
                CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                Producto.idProveedor = CostoProducto.idProveedor;
                Producto.Proveedor = CostoProducto.Proveedor;
                Producto.Costo = CostoProducto.Costo;
            }
            return Productos;
        }

        // POST: api/Productos
        public ID Post(Productos Producto)
        {
            if (Producto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");
            return new ID(ClassBD.AgregarProducto(Producto));
        }

        [Route("api/Productos/Costo")]
        public ID PostAgregarProductoCosto(ProductosCosto Producto)
        {
            if (Producto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");
            if (ClassBD.ExisteProducto(Producto.Producto, Producto.Color, Producto.SKU, Producto.CodigoBarras, Producto.idPresentacion) == 0)
            {
                ret = ClassBD.AgregarProducto(Producto);
                if (ret != 0)
                {
                    CostosHistoricos costo = new CostosHistoricos();
                    costo.idProducto = ret;
                    costo.idProveedor = Producto.idProveedor;
                    costo.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    costo.Costo = Producto.Costo;
                    ClassBD.AgregarCostosH(costo);
                }
            }
            return new ID(ret);
        }

        [Route("api/Productos/Rosedal")]
        public ID Post(ProductosCostoRosedal Producto)
        {
            if (Producto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");
            if (ClassBD.ExisteProducto(Producto.Producto, Producto.Color, Producto.SKU, Producto.CodigoBarras, 3) == 0)
            {
                Producto.PiezasTotales = Producto.Piezas;
                ProductosCosto prod = new ProductosCosto(0, Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.CodigoBarras, Producto.SKU, 2, "", Producto.ImagenP, Producto.Piezas, Producto.PiezasTotales, Producto.Color);
                ret = ClassBD.AgregarProducto(prod);
                CostosHistoricos costo = new CostosHistoricos();
                costo.idProducto = ret;
                costo.idProveedor = Producto.idProveedor;
                costo.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                costo.Costo = Producto.Costo;
                ClassBD.AgregarCostosH(costo);
                Costos ct = new Costos(0, ret, "", costo.idProveedor, "", costo.Costo);
                ClassBD.ActualizarCosto(ct);


                Producto.PiezasTotales = Producto.Piezas * Producto.CajasInternas;
                prod = new ProductosCosto(0, Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.CodigoBarras, Producto.SKU, 3, "", Producto.ImagenP, Producto.CajasInternas, Producto.PiezasTotales, Producto.Color);
                ret = ClassBD.AgregarProducto(prod);
                costo.idProducto = ret;
                ClassBD.AgregarCostosH(costo);
                ct.idProducto = ret;
                ClassBD.ActualizarCosto(ct);
            }
            return new ID(ret);
        }

        [Route("api/Productos/Rosedal/Txt")]
        public ID PostAgregarProductoCostoRosedalProovedor(ProductosCostoRosedal Producto)
        {
            if (Producto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            if (Producto.Descripcion == null)
                Producto.Descripcion = "";
            Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");
            if (ClassBD.ExisteProducto(Producto.Producto, Producto.Color, Producto.SKU, Producto.CodigoBarras, 3) == 0)
            {
                if (Producto.Proveedor != null && Producto.Proveedor != "")
                {
                    Proveedor prov = ClassBD.ObtenerProveedorByNombre(Producto.Proveedor);
                    if (prov.idProveedor == 0)
                    {
                        ret = ClassBD.AgregarEmpresa(new Empresa(0, Producto.Proveedor, "", "", "", "", "", "", "", 1, "", "", "", "", ""));
                        if (ret != -1)
                        {

                            Producto.idProveedor = ClassBD.AgregarProveedor(new Proveedor(0, ret, "", 0, 0, "", 1, "", 1, "", "", "", ""));
                            ret = -1;
                        }
                    }
                    else
                        Producto.idProveedor = prov.idProveedor;


                    if (Producto.idProveedor > 0)
                    {
                        Producto.PiezasTotales = Producto.Piezas;
                        ProductosCosto prod = new ProductosCosto(0, Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.CodigoBarras, Producto.SKU, 2, "", Producto.ImagenP, Producto.Piezas, Producto.PiezasTotales, Producto.Color);
                        ret = ClassBD.AgregarProducto(prod);
                        CostosHistoricos costo = new CostosHistoricos();
                        costo.idProducto = ret;
                        costo.idProveedor = Producto.idProveedor;
                        costo.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        costo.Costo = Producto.Costo;
                        ClassBD.AgregarCostosH(costo);
                        Costos ct = new Costos(0, ret, "", costo.idProveedor, "", costo.Costo);
                        ClassBD.ActualizarCosto(ct);


                        Producto.PiezasTotales = Producto.Piezas * Producto.CajasInternas;
                        prod = new ProductosCosto(0, Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.CodigoBarras, Producto.SKU, 3, "", Producto.ImagenP, Producto.CajasInternas, Producto.PiezasTotales, Producto.Color);
                        ret = ClassBD.AgregarProducto(prod);
                        costo.idProducto = ret;
                        ClassBD.AgregarCostosH(costo);
                        ct.idProducto = ret;
                        ClassBD.ActualizarCosto(ct);
                    }

                }

            }
            return new ID(ret);
        }

        [Route("api/Productos/Rosedal/Txts")]
        public List<ID> PostAgregarProductoCostoRosedalProovedor(List<InventarioIngesta> Productos)
        {
            if (Productos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            List<ID> listaID = new List<ID>();

            foreach (InventarioIngesta Producto in Productos)
            {
                ret = -1;
                if (Producto != null)
                {
                    if (Producto.Descripcion == null)
                        Producto.Descripcion = "";
                    Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");
                    if ((Producto.idProducto = ClassBD.ExisteProducto(Producto.Producto, Producto.Color, Producto.SKU, Producto.CodigoBarras, 3)) == 0)
                    {
                        if (Producto.Proveedor != null && Producto.Proveedor != "")
                        {
                            Proveedor prov = ClassBD.ObtenerProveedorByNombre(Producto.Proveedor);
                            if (prov.idProveedor == 0)
                            {
                                ret = ClassBD.AgregarEmpresa(new Empresa(0, Producto.Proveedor, "", "", "", "", "", "", "", 1, "", "", "", "", ""));
                                if (ret != -1)
                                {
                                    Producto.idProveedor = ClassBD.AgregarProveedor(new Proveedor(0, ret, "", 0, 0, "", 1, "", 1, "", "", "", ""));
                                    ret = -1;
                                }
                            }
                            else
                                Producto.idProveedor = prov.idProveedor;


                            if (Producto.idProveedor > 0)
                            {
                                Producto.PiezasTotales = Producto.Piezas;
                                ProductosCosto prod = new ProductosCosto(0, Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.CodigoBarras, Producto.SKU, 2, "", Producto.ImagenP, Producto.Piezas, Producto.PiezasTotales, Producto.Color);
                                ret = ClassBD.AgregarProducto(prod);
                                CostosHistoricos costo = new CostosHistoricos();
                                costo.idProducto = ret;
                                costo.idProveedor = Producto.idProveedor;
                                costo.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                costo.Costo = Producto.Costo;
                                ClassBD.AgregarCostosH(costo);
                                Costos ct = new Costos(0, ret, "", costo.idProveedor, "", costo.Costo);
                                ClassBD.ActualizarCosto(ct);


                                Producto.PiezasTotales = Producto.Piezas * Producto.CajasInternas;
                                prod = new ProductosCosto(0, Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.CodigoBarras, Producto.SKU, 3, "", Producto.ImagenP, Producto.CajasInternas, Producto.PiezasTotales, Producto.Color);
                                ret = ClassBD.AgregarProducto(prod);
                                costo.idProducto = ret;
                                ClassBD.AgregarCostosH(costo);
                                ct.idProducto = ret;
                                ClassBD.ActualizarCosto(ct);
                                if (ret != -1)
                                {
                                    ret = 0;
                                }
                                //listaID.Add(new ID(Producto.idProveedor <= 0 ? -1 : ret));
                            }
                        }
                    }
                    if (Producto.Almacen != null && Producto.Almacen != "")
                    {
                        Producto.Almacen = Producto.Almacen.Trim();
                        Sucursal almacen = ClassBD.ObtenerSucursalByName(Producto.Almacen);
                        if (almacen.idSucursal == 0)
                        {
                            Producto.idAlmacen = ClassBD.AgregarSucursal(new Sucursal(0, Producto.Almacen, "", "", "", "", "", "", 1, "", false, ""));
                        }
                        else
                            Producto.idAlmacen = almacen.idSucursal;

                        ClassBD.IngestarInventario(Producto.idProducto, Producto.idAlmacen, Producto.ExistenciaMinima, Producto.Existencia, Producto.Nuevo == "Sí");
                        ret = 1;
                    }
                    listaID.Add(new ID(ret));
                }
            }
            return listaID;
        }

        // PUT: api/Productos
        public void Put(Productos Producto)
        {
            if (Producto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");
            ClassBD.ActualizarProducto(Producto);
        }

        [Route("api/Productos/Costo")]
        public void Put(ProductosCosto Producto)
        {
            if (Producto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");
            ClassBD.ActualizarProducto(Producto);
            CostosHistoricos costo = new CostosHistoricos();
            costo.idProducto = Producto.idProducto;
            costo.idProveedor = Producto.idProveedor;
            costo.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            costo.Costo = Producto.Costo;
            ClassBD.AgregarCostosH(costo);
        }

        [Route("api/Productos/Rosedal")]
        public void Put(ProductosCostoRosedal Producto)
        {
            if (Producto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int id = Producto.idProducto;

            Producto.CodigoBarras = Producto.CodigoBarras.Replace("'", "-");

            Producto.idProducto = Producto.idProductoCaja;
            Producto.idPresentacion = 2;
            Producto.PiezasTotales = Producto.Piezas;
            ClassBD.ActualizarProducto(Producto);

            CostosHistoricos costo = new CostosHistoricos();
            costo.idProducto = Producto.idProducto;
            costo.idProveedor = Producto.idProveedor;
            costo.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            costo.Costo = Producto.Costo;
            ClassBD.AgregarCostosH(costo);
            Costos ct = new Costos(0, Producto.idProducto, "", costo.idProveedor, "", costo.Costo);
            ClassBD.ActualizarCosto(ct);

            Producto.idPresentacion = 3;
            Producto.idProducto = id;
            Producto.PiezasTotales = Producto.Piezas * Producto.CajasInternas;
            Producto.Piezas = Producto.CajasInternas;
            ClassBD.ActualizarProducto(Producto);
            costo.idProducto = Producto.idProducto;
            ClassBD.AgregarCostosH(costo);
            ct.idProducto = Producto.idProducto;
            ClassBD.ActualizarCosto(ct);
        }

        // DELETE: api/Productos/5
        public void Delete(int id)
        {
            ClassBD.EliminarProducto(id);
        }
    }
}
