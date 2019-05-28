using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class CostosHistoricosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/CostosHistoricos
        public List<CostosHistoricos> Get()
        {
            return ClassBD.ObtenerCostosH();
        }

        // GET: api/CostosHistoricos/5
        public CostosHistoricos Get(int id)
        {
            return ClassBD.ObtenerCostoH(id);
        }

        [Route("api/CostosHistoricos/Producto/{idProducto}")]
        public CostosHistoricos Get(string idProducto)
        {
            return ClassBD.ObtenerCostosHbyProduct(Convert.ToInt32(idProducto));
        }

        // POST: api/CostosHistoricos/5
        public ID Post(int idOrden)
        {
            OrdenesCVP OrdenCompra = new OrdenesCVP();
            List<ListaProductos> ListaCompra = new List<ListaProductos>();
            CostosHistoricos Costo = new CostosHistoricos();
            int ret = -1;
            if (idOrden == 0)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            OrdenCompra = ClassBD.ObtenerOrdenCById(Convert.ToInt32(idOrden));
            ListaCompra = ClassBD.ObtenerProductosByOrden(OrdenCompra.idOrdenCVP);
            foreach (ListaProductos item in ListaCompra)
            {
                Costo.Fecha = OrdenCompra.Fecha;
                Costo.Costo = item.CostoPrecio;
                Costo.idProducto = item.idProducto;
                Costo.idProveedor = OrdenCompra.idProveedor;
                ret = ClassBD.AgregarCostosH(Costo);
            }

            return new ID(ret);
        }

        // PUT: api/CostosHistoricos
        public void Put([FromBody]CostosHistoricos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarCostosH(Datos);
        }

        // DELETE: api/CostosHistoricos/5
        public void Delete(int id)
        {
            ClassBD.EliminarCostosH(id);
        }
    }
}
