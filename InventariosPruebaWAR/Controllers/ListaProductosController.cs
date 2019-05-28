using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class ListaProductosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();
        private FacturasController Facturas = new FacturasController();


        // GET: api/ListaProductos
        public List<ListaProductos> Get()
        {
            return ClassBD.ObtenerListaProductos();
        }

        // GET: api/ListaProductos/5
        public ListaProductos Get(int id)
        {
            return ClassBD.ObtenerListaProductoById(id);
        }


        //[Route("api/ObtenerProductosVendidos")]
        [Route("api/ListaProductos/ProductosVendidos")]
        public List<ListaProductos> GetObtenerProductosVendidos()
        {
            return ClassBD.ObtenerProductosVendidos();
        }

        [Route("api/ListaProductos/OrdenCVP/{idOrden}")]
        public List<ListaProductos> Get(string idOrden)
        {
            return ClassBD.ObtenerProductosByOrden(Convert.ToInt32(idOrden));
        }

        [Route("api/ListaProductos/Transferencia/{idTransferencia}")]
        public List<ListaProductos> GetListaProductoByTransfer(string idTransferencia)
        {
            return ClassBD.ObtenerProductosByTransfer(Convert.ToInt32(idTransferencia));
        }

        // POST: api/ListaProductos
        public ID Post([FromBody]List<ListaProductos> ListaProducto)
        {
            if (ListaProducto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            OrdenesCVP Orden = new OrdenesCVP();
            List<ListaProductos> ProductosOrden = null;
            int ret = -1;

            Orden = ClassBD.ObtenerOrdenCVPById(ListaProducto[0].idOrdenCVP);
            ProductosOrden = ClassBD.ObtenerProductosByOrden(Orden.idOrdenCVP);

            if (ProductosOrden != null && ProductosOrden.Count > 0)
                foreach (ListaProductos ListaExistente in ProductosOrden)
                    ClassBD.EliminarListaProducto(ListaExistente.idListaProductos);

            foreach (ListaProductos ListaProduc in ListaProducto)
            {
                if (Orden.idOperacion == 4)
                    ListaProduc.idPresentacion = 1;
                ret = ClassBD.AgregarListaProducto(ListaProduc);
            }
            if (ret != 0 && ret != -1)
                Facturas.PostGenerarFacturaNota(Orden);
            return new ID(ret);
        }

        [Route("api/ListaProductos/Transferencia")]
        public ID PostAgregarListaProductoTransferencia([FromBody]ListaProductos ListaProducto)
        {
            if (ListaProducto == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1, ExistenciaRX = 0;
            TransferSuc Transferencia = new TransferSuc();
            List<Inventario> InventarioTx = new List<Inventario>();
            List<Inventario> InventarioRx = new List<Inventario>();
            List<ListaProductos> ProductoTransfer = new List<ListaProductos>();
            Inventario Nuevo = new Inventario();

            if (ListaProducto != null)
                ret = ClassBD.AgregarListaProductoTransferencia(ListaProducto);
            if (ret != -1 && ret != 0)
            {
                Transferencia = ClassBD.ObtenerTransferenciaById(ListaProducto.idTransferencia);
                ProductoTransfer = ClassBD.ObtenerProductosByTransfer(ListaProducto.idTransferencia);
                InventarioTx = ClassBD.ObtenerInventarioBySucursal(Transferencia.idSucTx);
                InventarioRx = ClassBD.ObtenerInventarioBySucursal(Transferencia.idSucRx);

                foreach (ListaProductos Productos in ProductoTransfer)
                {
                    foreach (Inventario ProductoInventario in InventarioTx)
                    {
                        if (Productos.idProducto == ProductoInventario.idProducto)
                        {
                            ProductoInventario.Existencia -= Productos.Cantidad;
                            ClassBD.ActualizarInventario(ProductoInventario);
                        }

                    }
                }
                InventarioTx = ClassBD.ObtenerInventarioBySucursal(Transferencia.idSucTx);
                InventarioRx = ClassBD.ObtenerInventarioBySucursal(Transferencia.idSucRx);
                foreach (ListaProductos Productos in ProductoTransfer)
                {
                    foreach (Inventario ProductoInventario in InventarioRx)
                    {
                        if (Productos.idProducto == ProductoInventario.idProducto)
                        {
                            ExistenciaRX++;
                            ProductoInventario.Existencia += Productos.Cantidad;
                            ClassBD.ActualizarInventario(ProductoInventario);
                        }
                    }
                    if (ExistenciaRX == 0)
                    {
                        Nuevo = new Inventario()
                        {
                            idProducto = Productos.idProducto,
                            Existencia = Productos.Cantidad,
                            ExistenciaMinima = 20,
                            idSucursal = Transferencia.idSucRx
                        };
                        ClassBD.AgregarInventario(Nuevo);
                        ExistenciaRX = 0;
                    }
                }
            }
            return new ID(ret);
        }

        // PUT: api/ListaProductos
        public void Put([FromBody]ListaProductos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarListaProducto(Datos);
        }

        // DELETE: api/ListaProductos/5
        public void Delete(int id)
        {
            ClassBD.EliminarListaProducto(id);
        }
    }
}
