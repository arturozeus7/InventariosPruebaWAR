using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class PrivilegiosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Privilegios
        public List<Privilegios> Get()
        {
            return ClassBD.ObtenerPrivilegios();
        }
        /*
        // GET: api/Privilegios/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Privilegios
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Privilegios/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Privilegios/5
        public void Delete(int id)
        {
        }*/
    }
}
