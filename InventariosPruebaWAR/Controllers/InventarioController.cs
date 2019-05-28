using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class InventarioController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Inventario
        public List<Inventario> Get()
        {
            List<Inventario> ProductosInventario = ClassBD.ObtenerInventario();
            CostosHistoricos CostoProducto = new CostosHistoricos();
            foreach (Inventario Producto in ProductosInventario)
            {
                CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                Producto.idProveedor = CostoProducto.idProveedor;
                Producto.Proveedor = CostoProducto.Proveedor;
            }
            return ProductosInventario;
        }

        // GET: api/Inventario/5
        public Inventario Get(int id)
        {
            return ClassBD.ObtenerInventarioById(id);
        }

        [Route("api/Inventario/Sucursal/{idSucursal}")]
        public List<Inventario> GetObtenerInventarioBySucursal(string idSucursal)
        {
            return ClassBD.ObtenerInventarioBySucursal(Convert.ToInt32(idSucursal));
        }

        [Route("api/Inventario/Sucursal/{idSucursal}/Busqueda/{consulta}")]
        public List<Inventario> GetBusquedaInventarioSuc(string idSucursal, string Consulta)
        {
            Consulta = Consulta.Replace("'", "-");
            return ClassBD.BusquedaInventarioSuc(Convert.ToInt32(idSucursal), Consulta);
        }

        [Route("api/Inventario/Bajo")]
        public List<Inventario> GetObtenerInventarioBajo()
        {
            List<Inventario> ProductosInventario = ClassBD.ObtenerInventario();
            CostosHistoricos CostoProducto = new CostosHistoricos();
            foreach (Inventario Producto in ProductosInventario)
            {
                CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                Producto.idProveedor = CostoProducto.idProveedor;
                Producto.Proveedor = CostoProducto.Proveedor;
            }

            List<Inventario> ProductosBajos = new List<Inventario>();
            foreach (Inventario Producto in ProductosInventario)
                if (Producto.Existencia <= Producto.ExistenciaMinima)
                    ProductosBajos.Add(Producto);

            return ProductosBajos;
        }

        [Route("api/Inventario/Busqueda/{consulta}")]
        public List<Inventario> Get(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            List<Inventario> ProductosInventario = ClassBD.ObtenerBusquedaInventario(consulta);
            CostosHistoricos CostoProducto = new CostosHistoricos();
            foreach (Inventario Producto in ProductosInventario)
            {
                CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                Producto.idProveedor = CostoProducto.idProveedor;
                Producto.Proveedor = CostoProducto.Proveedor;
            }
            return ProductosInventario;
        }
        
        [Route("api/Inventario/Busqueda/ProdSinInventario/{idSucursal}/{consulta}")]
        public List<ProductosCosto> GetBusquedaProductosSinInventario(string idSucursal, string consulta)
        {
            consulta = consulta.Replace("'", "-");
            int Verificador = 0;
            List<Inventario> ProductosEnLaSucursal = ClassBD.ObtenerInventarioBySucursal(Convert.ToInt32(idSucursal));
            List<ProductosCosto> ProductosSinInventario = new List<ProductosCosto>();
            List<ProductosCosto> Productos = ClassBD.ObtenerProductosBusquedaDinamica(consulta);

            foreach (ProductosCosto Producto in Productos)
            {
                foreach (Inventario InventarioActual in ProductosEnLaSucursal)
                {
                    if (Producto.idProducto == InventarioActual.idProducto)
                        Verificador = 1;
                }
                if (Verificador == 0)
                    ProductosSinInventario.Add(Producto);
                Verificador = 0;
            }
            return ProductosSinInventario;
        }

        [Route("api/Inventario/AbrirCaja/{id}")]
        public ID GetAbrirCaja(string id)
        {
            int ret = -1;
            int idinventario = 0;
            int.TryParse(id, out idinventario);
            if (idinventario == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (ClassBD.AbrirCaja(idinventario))
                ret = 0;
            return new ID(ret);
        }

        [Route("api/Inventario/AbrirCajaMaestra/{id}")]
        public ID GetAbrirCajaMaestra(string id)
        {
            int ret = -1;
            int idinventario = 0;
            int.TryParse(id, out idinventario);
            if (idinventario == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (ClassBD.AbrirCajaMaster(idinventario))
                ret = 0;
            return new ID(ret);
        }

        // POST: api/Inventario
        public ID Post([FromBody]Inventario Inventario)
        {
            if (Inventario == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;

            if (Inventario != null)
                ret = ClassBD.AgregarInventario(Inventario);
            if (ret != -1)
            {
                Inventario Actual = ClassBD.ObtenerInventarioById(ret);
                InventarioMov NuevoMovimiento = new InventarioMov()
                {
                    idInventario = Actual.idInventario,
                    idSucursal = Actual.idSucursal,
                    Movimiento = "Nuevo",
                    CantidadActual = 0,
                    Cantidad = Actual.Existencia,
                    CantidadNueva = Inventario.Existencia,
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                };
                ClassBD.AgregarMovimiento(NuevoMovimiento);
            }
            return new ID(ret);
        }

        [Route("api/Inventario/AgregarInventariotxt")]
        public ID PostAgregarInventariotxt([FromBody]List<Inventario> Inventario)
        {
            if (Inventario == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Productos AgregarProducto = new Productos();
            List<Inventario> Existente = new List<Inventario>();
            int ret = -1;

            if (Inventario != null)
                foreach (Inventario Nuevo in Inventario)
                {
                    AgregarProducto = ClassBD.ObtenerProductoByCodigoBarras(Nuevo.CodigoBarras);
                    Existente = ClassBD.BusquedaInventarioSuc(Nuevo.idSucursal, Nuevo.CodigoBarras);
                    if (Existente != null && Existente.Count > 0)
                    {

                        if (Existente[0].Codigo != null && Existente[0].Producto != null && Existente[0].Codigo != "" && Existente[0].Producto != "")
                        {
                            Inventario Actual = ClassBD.ObtenerInventarioById(Existente[0].idInventario);
                            Existente[0].Existencia += Nuevo.Existencia;
                            ClassBD.ActualizarInventario(Existente[0]);
                            InventarioMov NuevoMovimiento = new InventarioMov()
                            {
                                idInventario = Actual.idInventario,
                                idSucursal = Actual.idSucursal,
                                Movimiento = "Actualizar",
                                CantidadActual = Actual.Existencia,
                                Cantidad = Math.Abs(Actual.Existencia - Existente[0].Existencia),
                                CantidadNueva = Existente[0].Existencia,
                                Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            };
                            ClassBD.AgregarMovimiento(NuevoMovimiento);
                            ret = 1;
                        }

                    }
                    else
                    {
                        Existente.Add(new Inventario(0, AgregarProducto.idProducto, AgregarProducto.Producto, Nuevo.idSucursal, Nuevo.Sucursal, Nuevo.Existencia, 20, AgregarProducto.Descripcion,
                            AgregarProducto.Codigo, AgregarProducto.idPresentacion, AgregarProducto.Presentacion, AgregarProducto.Color, AgregarProducto.Piezas, AgregarProducto.PiezasTotales, AgregarProducto.CodigoBarras));
                        ret = ClassBD.AgregarInventario(Existente[0]);
                        if (ret != -1)
                        {
                            Inventario Actual = ClassBD.ObtenerInventarioById(ret);
                            InventarioMov NuevoMovimiento = new InventarioMov()
                            {
                                idInventario = Actual.idInventario,
                                idSucursal = Actual.idSucursal,
                                Movimiento = "Nuevo",
                                CantidadActual = 0,
                                Cantidad = Actual.Existencia,
                                CantidadNueva = Existente[0].Existencia,
                                Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            };
                            ClassBD.AgregarMovimiento(NuevoMovimiento);
                        }
                    }
                }
            return new ID(ret);
        }

        // PUT: api/Inventario
        public void Put([FromBody]Inventario Inventario)
        {
            if (Inventario == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Inventario Actual = ClassBD.ObtenerInventarioById(Inventario.idInventario);
            ClassBD.ActualizarInventario(Inventario);
            InventarioMov NuevoMovimiento = new InventarioMov()
            {
                idInventario = Inventario.idInventario,
                idSucursal = Actual.idSucursal,
                Movimiento = "Actualizar",
                CantidadActual = Actual.Existencia,
                Cantidad = Math.Abs(Actual.Existencia - Inventario.Existencia),
                CantidadNueva = Inventario.Existencia,
                Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            ClassBD.AgregarMovimiento(NuevoMovimiento);
        }

        // DELETE: api/Inventario/5
        public void Delete(int id)
        {
            ClassBD.EliminarInventario(id);
        }
    }
}
