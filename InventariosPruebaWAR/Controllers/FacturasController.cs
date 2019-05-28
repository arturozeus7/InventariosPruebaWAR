using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class FacturasController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Facturas
        public List<Factura> Get()
        {
            return ClassBD.ObtenerFactura();
        }

        // GET: api/Facturas/5
        public Factura Get(int id)
        {
            return ClassBD.ObtenerFacturaById(id);
        }

        // POST: api/Facturas
        public ID Post([FromBody]Factura Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarFactura(Datos));
        }

        [Route("api/Facturas/GenerarFacturaNota")]
        public void PostGenerarFacturaNota(OrdenesCVP OrdenV)
        {
            if (OrdenV != null)
            {
                if (ClassBD.ObtenerOrdenCVPById(OrdenV.idOrdenCVP).idOperacion == 4)
                    ClassBD.FacturaCotizacionXpz(OrdenV);
                if (ClassBD.ObtenerOrdenCVPById(OrdenV.idOrdenCVP).idOperacion == 3)
                    ClassBD.LlenarFactura(OrdenV);
                if (ClassBD.ObtenerOrdenCVPById(OrdenV.idOrdenCVP).idOperacion == 2)
                    ClassBD.FacturaCotizacion(OrdenV);
                if (ClassBD.ObtenerOrdenCVPById(OrdenV.idOrdenCVP).idOperacion == 1)
                    ClassBD.FacturaCompas(OrdenV);
            }
        }

        [Route("api/Facturas/FacturaCompra")]
        public void PostFacturaCompra(OrdenesCVP OrdenV)
        {
            if (OrdenV != null)
                if (ClassBD.ObtenerOrdenCVPById(OrdenV.idOrdenCVP).idOperacion == 1)
                    ClassBD.FacturaCompas(OrdenV);
        }

        // PUT: api/Facturas
        public void Put([FromBody]Factura Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarFactura(Datos);
        }

        // DELETE: api/Facturas/5
        public void Delete(int id)
        {
            ClassBD.EliminarFactura(id);
        }
    }
}
