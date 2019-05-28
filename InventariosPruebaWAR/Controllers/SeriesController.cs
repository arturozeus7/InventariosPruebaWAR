using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class SeriesController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Series
        public List<Serie> Get()
        {
            return ClassBD.ObtenerSerie();
        }

        // GET: api/Series/5
        public Serie Get(int id)
        {
            return ClassBD.ObtenerSerieById(id);
        }

        [Route("api/Series/Prefijo/{prefijo}")]
        public List<Serie> Get(string prefijo)
        {
            return ClassBD.ObtenerSerieByPrefijo(prefijo);
        }

        // POST: api/Series
        public ID Post([FromBody]Serie Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarSerie(Datos);

            return new ID(ret);
        }

        // PUT: api/Series
        public void Put([FromBody]Serie Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarSerie(Datos);
        }

        // DELETE: api/Series/5
        public void Delete(int id)
        {
            ClassBD.EliminarSerie(id);
        }
    }
}
