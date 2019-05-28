using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class RecepcionMercaController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/RecepcionMerca
        public List<RecepcionMerca> Get()
        {
            return ClassBD.ObtenerRecepcionMerca();
        }

        // GET: api/RecepcionMerca/5
        public RecepcionMerca Get(int id)
        {
            return ClassBD.ObtenerRecepcionById(id);
        }

        [Route("api/RecepcionMerca/Orden/{id}")]
        public RecepcionMerca Get(string id)
        {
            return ClassBD.ObtenerRecepcionByOrden(Convert.ToInt32(id));
        }

        // POST: api/RecepcionMerca
        public ID Post([FromBody]RecepcionMerca Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            OrdenesCVP OrdenCVP = new OrdenesCVP();
            List<Inventario> InventarioActual = new List<Inventario>();
            List<ListaProductos> ProductosOrden = new List<ListaProductos>();

            if (Datos != null)
                ret = ClassBD.AgregarRecepcion(Datos);
            if (ret != 0 & ret != -1)
            {
                OrdenCVP = ClassBD.ObtenerOrdenCById(Datos.idOrdenCVP);
                ProductosOrden = ClassBD.ObtenerProductosByOrden(Datos.idOrdenCVP);
                InventarioActual = ClassBD.ObtenerInventarioBySucursal(OrdenCVP.idSucursal);
                foreach (ListaProductos ProductoOrden in ProductosOrden)
                {
                    foreach (Inventario ProductoInventario in InventarioActual)
                    {
                        if (ProductoOrden.idProducto == ProductoInventario.idProducto)
                        {
                            ProductoInventario.Existencia += ProductoOrden.Cantidad;
                            ClassBD.ActualizarInventario(ProductoInventario);
                            OrdenCVP.idEstatus = 5;
                            ClassBD.ActualizarOrdenC(OrdenCVP);
                        }
                    }

                }
            }

            return new ID(ret);
        }

        // PUT: api/RecepcionMerca
        public void Put([FromBody]RecepcionMerca Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarRecepcion(Datos);
        }

        // DELETE: api/RecepcionMerca/5
        public void Delete(int id)
        {
            ClassBD.EliminarRecepcion(id);
        }
    }
}
