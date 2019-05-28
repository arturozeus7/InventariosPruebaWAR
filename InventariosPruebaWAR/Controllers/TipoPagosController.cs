using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class TipoPagosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/TipoPagos
        public List<TiposdePago> Get()
        {
            return ClassBD.ObtenerTipodePago();
        }

        // GET: api/TipoPagos/5
        public TiposdePago Get(int id)
        {
            return ClassBD.ObtenerTipodePagoById(id);
        }

        [Route("api/TipoPagos/Nombre/{nombre}")]
        public TiposdePago Get(string nombre)
        {
            return ClassBD.ObtenerTipodePagoByTipo(nombre);
        }

        // POST: api/TipoPagos
        public ID Post([FromBody]TiposdePago Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarTipodePago(Datos);

            return new ID(ret);
        }

        // PUT: api/TipoPagos
        public void Put([FromBody]TiposdePago Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTipodePago(Datos);
        }

        // DELETE: api/TipoPagos/5
        public void Delete(int id)
        {
            ClassBD.EliminarTipodePago(id);
        }
    }
}
