using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class TipoMaterialesController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/TipoMateriales
        public List<TipoMateriales> Get()
        {
            return ClassBD.ObtenerTipoMateriales();
        }
        /*
        // GET: api/TipoMateriales/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TipoMateriales
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TipoMateriales/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TipoMateriales/5
        public void Delete(int id)
        {
        }*/
    }
}
