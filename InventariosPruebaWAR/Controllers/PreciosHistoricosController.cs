using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class PreciosHistoricosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/PreciosHistoricos
        public List<PreciosHistoricos> Get()
        {
            return ClassBD.ObtenerPreciosH();
        }

        // GET: api/PreciosHistoricos/5
        public PreciosHistoricos Get(int id)
        {
            return ClassBD.ObtenerPrecioH(id);
        }

        // POST: api/PreciosHistoricos
        public ID Post([FromBody]PreciosHistoricos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarPreciosH(Datos);
            return new ID(ret);
        }

        // PUT: api/PreciosHistoricos
        public void Put([FromBody]PreciosHistoricos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPreciosH(Datos);
        }

        // DELETE: api/PreciosHistoricos/5
        public void Delete(int id)
        {
            ClassBD.EliminarPreciosH(id);
        }
    }
}
