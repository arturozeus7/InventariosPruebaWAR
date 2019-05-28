using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class OrdenesCVPController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();
        private FacturasController Facturas = new FacturasController();
        private CostosHistoricosController CostosH = new CostosHistoricosController();

        [Route("api/Ordenes/Compras")]
        public List<OrdenesCVP> Get()
        {
            return ClassBD.ObtenerOrdenesC();
        }

        [Route("api/Ordenes/Compras/{id}")]
        public OrdenesCVP Get(int id)
        {
            return ClassBD.ObtenerOrdenCById(id);
        }

        [Route("api/Ordenes/Compras/Completa/{id}")]
        public OrdenesCVP GetObtenerOrdenCompletaById(int id)
        {
            return ClassBD.ObtenerOrdenCompraCompletaById(id);
        }

        [Route("api/Ordenes/Compras/Procesar")]
        public List<OrdenesCVP> GetObtenerOrdenesCP()
        {
            return ClassBD.ObtenerOrdenesCP();
        }

        [Route("api/Ordenes/Compras/Recepciones")]
        public List<OrdenesCVP> GetObtenerOrdenesCR()
        {
            return ClassBD.ObtenerOrdenesCR();
        }

        [Route("api/Ordenes/Compras/Recibir")]
        public List<OrdenesCVP> GetObtenerOrdenesCPR()
        {
            return ClassBD.ObtenerOrdenesCPR();
        }

        [Route("api/Ordenes/Compras/Busqueda/{consulta}")]
        public List<OrdenesCVP> GetBusquedaCompras(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerBusquedaOrdenesC(consulta);
        }

        [Route("api/Ordenes/Ventas")]
        public List<OrdenesCVP> GetOrdenesV()
        {
            return ClassBD.ObtenerOrdenesV();
        }

        [Route("api/Ordenes/Ventas/{id}")]
        public OrdenesCVP GetOrdenVById(int id)
        {
            return ClassBD.ObtenerOrdenVById(id);
        }

        [Route("api/Ordenes/Ventas/Completa/{id}")]
        public OrdenesCVP GetObtenerOrdenVCompletaById(int id)
        {
            return ClassBD.ObtenerOrdenVCompletaById(id);
        }

        [Route("api/Ordenes/Ventas/Procesar")]
        public List<OrdenesCVP> GetOrdenesVP()
        {
            return ClassBD.ObtenerOrdenesVP();
        }

        [Route("api/Ordenes/Ventas/Entregas")]
        public List<OrdenesCVP> GetOrdenesVE()
        {
            return ClassBD.ObtenerOrdenesVE();
        }

        [Route("api/Ordenes/Ventas/Entregar")]
        public List<OrdenesCVP> GetOrdenesVPE()
        {
            return ClassBD.ObtenerOrdenesVPE();
        }

        [Route("api/Ordenes/Ventas/Busqueda/{consulta}")]
        public List<OrdenesCVP> GetBusquedaVentas(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerBusquedaOrdenesV(consulta);
        }

        [Route("api/Ordenes/Cotizaciones")]
        public List<OrdenesCVP> GetCotizaciones()
        {
            return ClassBD.ObtenerCotizaciones();
        }

        [Route("api/Ordenes/Cotizaciones/{id}")]
        public OrdenesCVP GetCotizacionById(int id)
        {
            return ClassBD.ObtenerCotizacionById(id);
        }

        [Route("api/Ordenes/Cotizaciones/Completa/{id}")]
        public OrdenesCVP GetObtenerCotizacionCompletaById(int id)
        {
            return ClassBD.ObtenerCotizacionCompletaById(id);
        }

        [Route("api/Ordenes/Cotizaciones/Estatus/{estatus}")]
        public List<OrdenesCVP> GetCotizacionesEstado(string estatus)
        {
            return ClassBD.ObtenerCotizaciones(estatus);
        }

        [Route("api/Ordenes/Cotizaciones/Busqueda/{consulta}")]
        public List<OrdenesCVP> GetBusquedaCotizaciones(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerBusquedaCotizaciones(consulta);
        }

        [Route("api/Ordenes/Compras")]
        public ID Post([FromBody]OrdenesCVP Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;

            try
            {
                ret = ClassBD.AgregarOrdenC(Datos);
            }
            catch (Exception e)
            {
                StreamWriter fs = File.AppendText("log.txt");
                fs.WriteLine(e.Message);
                fs.Close();
                fs.Dispose();
            }
            return new ID(ret);
        }

        [Route("api/Ordenes/Ventas")]
        public ID PostOrdenV([FromBody]OrdenesCVP OrdenV)
        {
            if (OrdenV == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            OrdenesCVP OrdenCVP = new OrdenesCVP();
            List<Inventario> InventarioActual = new List<Inventario>();
            List<ListaProductos> ProductosOrden = new List<ListaProductos>();

            if (OrdenV != null)
                ret = ClassBD.AgregarOrdenV(OrdenV);

            if (ret != -1 && ret != 0)
            {
                OrdenCVP = ClassBD.ObtenerOrdenVById(ret);
                ProductosOrden = ClassBD.ObtenerProductosByOrden(OrdenCVP.idOrdenCVP);
                InventarioActual = ClassBD.ObtenerInventarioBySucursal(OrdenCVP.idSucursal);
                foreach (ListaProductos ProductoOrden in ProductosOrden)
                {
                    foreach (Inventario ProductoInventario in InventarioActual)
                    {
                        if (ProductoOrden.idProducto == ProductoInventario.idProducto)
                        {
                            ProductoInventario.Existencia -= ProductoOrden.Cantidad;
                            ClassBD.ActualizarInventario(ProductoInventario);
                            OrdenCVP.idEstatus = 5;
                            ClassBD.ActualizarOrdenV(OrdenCVP);
                        }
                    }
                }
            }
            return new ID(ret);
        }

        [Route("api/Ordenes/Cotizaciones")]
        public ID PostCotizacion([FromBody]OrdenesCVP OrdenV)
        {
            if (OrdenV == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            OrdenesCVP Cotizacion = new OrdenesCVP();

            ret = ClassBD.AgregarCotizacion(OrdenV);
            return new ID(ret);
        }

        [Route("api/Ordenes/Compras")]
        public void PutOrdenC([FromBody]OrdenesCVP Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            OrdenesCVP OrdenActual = null;

            OrdenActual = ClassBD.ObtenerOrdenCById(Datos.idOrdenCVP);
            OrdenActual.Total = Datos.Total;
            OrdenActual.Subtotal = Datos.Subtotal;
            OrdenActual.ValorIVA = Datos.ValorIVA;
            //ClassBD.ActualizarOrdenC(OrdenC);
            ClassBD.ActualizarOrdenC(OrdenActual);
        }

        [Route("api/Ordenes/Ventas")]
        public void PutOrdenV([FromBody]OrdenesCVP Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            OrdenesCVP OrdenActual = null;
            OrdenActual = ClassBD.ObtenerOrdenVById(Datos.idOrdenCVP);
            OrdenActual.Total = Datos.Total;
            OrdenActual.Subtotal = Datos.Subtotal;
            OrdenActual.ValorIVA = Datos.ValorIVA;
            //ClassBD.ActualizarOrdenV(OrdenV);
            ClassBD.ActualizarOrdenV(OrdenActual);
        }

        [Route("api/Ordenes/Cotizaciones")]
        public void PutCotizacion([FromBody]OrdenesCVP Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            OrdenesCVP OrdenActual = null;

            OrdenActual = ClassBD.ObtenerCotizacionById(Datos.idOrdenCVP);
            OrdenActual.Total = Datos.Total;
            OrdenActual.Subtotal = Datos.Subtotal;
            OrdenActual.ValorIVA = Datos.ValorIVA;
            //ClassBD.ActualizarCotizacion(OrdenV);
            ClassBD.ActualizarCotizacion(OrdenActual);
        }

        [Route("api/Ordenes/Compras/{id}/GenerarPDF")]
        public ID GetCrearPDF(string id)
        {
            if (id == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            OrdenesCVP orden;
            try
            {
                orden = ClassBD.ObtenerOrdenCVPById(Convert.ToInt32(id));

                if (orden.idOperacion == 4)
                    ClassBD.FacturaCotizacionXpz(orden);
                if (orden.idOperacion == 3)
                    ClassBD.LlenarFactura(orden);
                if (orden.idOperacion == 2)
                    ClassBD.FacturaCotizacion(orden);
                if (orden.idOperacion == 1)
                    ClassBD.FacturaCompas(orden);
            }
            catch (Exception e)
            {
                StreamWriter fs = File.AppendText("log.txt");
                fs.WriteLine(e.Message);
                fs.Close();
                fs.Dispose();
            }
            return new ID(ret);
        }

        [Route("api/Ordenes/Ventas/Salida/{id}")]
        public ID GetActualizarSalida(string id)
        {
            if (id == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            InventarioMov NuevoMovimiento = new InventarioMov();
            OrdenesCVP OrdenCVP = new OrdenesCVP();
            List<Inventario> InventarioActual = new List<Inventario>();
            List<ListaProductos> ProductosOrden = new List<ListaProductos>();

            if (id != null && Convert.ToInt32(id) > 0)
                if (ClassBD.ObtenerOrdenVById(Convert.ToInt32(id)).idOperacion == 3)
                {
                    ret = Convert.ToInt32(id);
                    OrdenCVP = ClassBD.ObtenerOrdenVById(ret);
                    ProductosOrden = ClassBD.ObtenerProductosByOrden(OrdenCVP.idOrdenCVP);
                    InventarioActual = ClassBD.ObtenerInventarioBySucursal(OrdenCVP.idSucursal);
                    foreach (ListaProductos ProductoOrden in ProductosOrden)
                    {
                        foreach (Inventario ProductoInventario in InventarioActual)
                        {
                            if (ProductoOrden.idProducto == ProductoInventario.idProducto)
                            {
                                NuevoMovimiento = new InventarioMov()
                                {
                                    idInventario = ProductoInventario.idInventario,
                                    idSucursal = ProductoInventario.idSucursal,
                                    Movimiento = "Venta",
                                    CantidadActual = ProductoInventario.Existencia,
                                    Cantidad = ProductoOrden.Cantidad,
                                    CantidadNueva = ProductoInventario.Existencia - ProductoOrden.Cantidad,
                                    Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                                };
                                ClassBD.AgregarMovimiento(NuevoMovimiento);
                                ProductoInventario.Existencia -= ProductoOrden.Cantidad;
                                ClassBD.ActualizarInventario(ProductoInventario);
                                if (OrdenCVP.idEstatus == 2)
                                    OrdenCVP.idEstatus = 3;
                                if (OrdenCVP.idEstatus == 4)
                                    OrdenCVP.idEstatus = 5;
                                ClassBD.ActualizarOrdenV(OrdenCVP);
                            }
                        }
                    }
                    ret = 0;
                }
            return new ID(ret);
        }

        [Route("api/Ordenes/Compras/Entrada/{id}")]
        public ID GetActualizarEntrada(string id)
        {
            if (id == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            InventarioMov NuevoMovimiento = new InventarioMov();
            OrdenesCVP OrdenCVP = new OrdenesCVP();
            List<Inventario> InventarioActual = new List<Inventario>();
            List<ListaProductos> ProductosOrden = new List<ListaProductos>();
            Inventario NuevoInventario = new Inventario();
            int existencia = 0;

            if (id != null && Convert.ToInt32(id) > 0)
            {
                if (ClassBD.ObtenerOrdenCById(Convert.ToInt32(id)).idOperacion == 1)
                {
                    ret = Convert.ToInt32(id);
                    OrdenCVP = ClassBD.ObtenerOrdenCById(ret);
                    ProductosOrden = ClassBD.ObtenerProductosByOrden(OrdenCVP.idOrdenCVP);
                    InventarioActual = ClassBD.ObtenerInventarioBySucursal(OrdenCVP.idSucursal);
                    foreach (ListaProductos ProductoOrden in ProductosOrden)
                    {
                        foreach (Inventario ProductoInventario in InventarioActual)
                        {
                            if (ProductoOrden.idProducto == ProductoInventario.idProducto)
                            {
                                NuevoMovimiento = new InventarioMov()
                                {
                                    idInventario = ProductoInventario.idInventario,
                                    idSucursal = ProductoInventario.idSucursal,
                                    Movimiento = "Compra",
                                    CantidadActual = ProductoInventario.Existencia,
                                    Cantidad = ProductoOrden.Cantidad,
                                    CantidadNueva = ProductoInventario.Existencia + ProductoOrden.Cantidad,
                                    Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                                };
                                ClassBD.AgregarMovimiento(NuevoMovimiento);
                                ProductoInventario.Existencia += ProductoOrden.Cantidad;
                                ClassBD.ActualizarInventario(ProductoInventario);
                                existencia = 1;
                            }
                        }
                        if (existencia == 0)
                        {
                            NuevoInventario.idProducto = ProductoOrden.idProducto;
                            NuevoInventario.idSucursal = OrdenCVP.idSucursal;
                            NuevoInventario.Existencia = ProductoOrden.Cantidad;
                            NuevoInventario.ExistenciaMinima = 20;
                            ClassBD.AgregarInventario(NuevoInventario);
                        }
                        existencia = 0;
                    }
                    if (OrdenCVP.idEstatus == 4)
                        OrdenCVP.idEstatus = 5;
                    if (OrdenCVP.idEstatus == 2)
                        OrdenCVP.idEstatus = 3;
                    ClassBD.ActualizarOrdenC(OrdenCVP);
                }
                ret = 0;
            }
            return new ID(ret);
        }

        [Route("api/Ordenes/{id}")]
        public void Delete(int id)
        {
            ClassBD.EliminarOrdenCVP(id);
        }

        [Route("api/Ordenes/Aprobar/{idEmpleado}/{id}")]
        public ID GetAprobarOrdenCVP(string id, string idEmpleado)
        {
            if (id == null || idEmpleado == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            if (id != null)
            {
                if (ClassBD.ObtenerOrdenCVPById(Convert.ToInt32(id)).idEstatus == 1)
                    ClassBD.AprobarOrdenCVP(Convert.ToInt32(id), Convert.ToInt32(idEmpleado));
                if (ClassBD.ObtenerOrdenCVPById(Convert.ToInt32(id)).idOperacion == 3)
                    ClassBD.LlenarFactura(GetOrdenVById(Convert.ToInt32(id)));
                ret = 0;
            }
            return new ID(ret);
        }

        [Route("api/Ordenes/Pagar/{id}")]
        public ID GetPagarOrdenCVP(string id)
        {
            if (id == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            if (id != null)
            {
                ClassBD.PagarOrdenCVP(Convert.ToInt32(id));
                ret = 0;
            }
            return new ID(ret);
        }

        [Route("api/Ordenes/Cotizaciones/ToVenta/{id}")]
        public ID GetCotizacionToVenta(string id)
        {
            if (id == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            if (id != null)
                if (ClassBD.ObtenerOrdenCVPById(Convert.ToInt32(id)).idOperacion == 2)
                    ret = ClassBD.CotizacionToVenta(Convert.ToInt32(id));

            return new ID(ret);
        }

        [Route("api/Ordenes/ActualizarSaldoCuenta")]
        public ID PostActualizarSaldoCuenta([FromBody]PagarCompra CompraCuenta)
        {
            if (CompraCuenta == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            OrdenesCVP Orden = ClassBD.ObtenerOrdenCVPById(CompraCuenta.idOrdenCVP);
            if (CompraCuenta != null)
            {
                if (Orden.idOperacion == 1)
                    CostosH.Post(CompraCuenta.idOrdenCVP);
                ret = ClassBD.ActualizarSaldoCuenta(CompraCuenta.idOrdenCVP, CompraCuenta.idCuenta);
            }

            if (ret != -1 && ret != 0)
                Facturas.PostGenerarFacturaNota(Orden);

            return new ID(ret);
        }

        [Route("api/Ordenes/Cotizaciones/Pieza")]
        public List<OrdenesCVP> GetCotizacionesXpz()
        {
            return ClassBD.ObtenerCotizacionesXpz();
        }

        [Route("api/Ordenes/Cotizaciones/Pieza/{id}")]
        public OrdenesCVP GetCotizacionByIdXpz(int id)
        {
            return ClassBD.ObtenerCotizacionByIdXpz(id);
        }

        [Route("api/Ordenes/Cotizaciones/Pieza/Busqueda/{consulta}")]
        public List<OrdenesCVP> GetBusquedaCotizacionesXpz(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerBusquedaCotizacionesXpz(consulta);
        }

        [Route("api/Ordenes/Cotizaciones/Pieza")]
        public ID PostCotizacionXpz([FromBody]OrdenesCVP OrdenV)
        {
            if (OrdenV == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            OrdenesCVP Cotizacion = new OrdenesCVP();

            ret = ClassBD.AgregarCotizacionXpz(OrdenV);
            return new ID(ret);
        }

        [Route("api/Ordenes/Cotizaciones/Pieza")]
        public void PutCotizacionXpz([FromBody]OrdenesCVP Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarCotizacionXpz(Datos);
        }
    }
}
