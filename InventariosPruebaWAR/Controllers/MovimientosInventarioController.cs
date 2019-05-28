using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class MovimientosInventarioController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/MovimientosInventario
        /*public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

        // GET: api/MovimientosInventario/5
        public List<InventarioMov> Get(int id)
        {
            return ClassBD.ObtenerMovimientos(id);
        }

        // POST: api/MovimientosInventario
        /*public void Post([FromBody]string value)
        {
        }

        // PUT: api/MovimientosInventario/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MovimientosInventario/5
        public void Delete(int id)
        {
        }*/
    }
}
