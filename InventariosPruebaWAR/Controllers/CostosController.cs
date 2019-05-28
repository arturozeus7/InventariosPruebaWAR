using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class CostosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Costos
        public List<Costos> Get()
        {
            return ClassBD.ObtenerCostos();
        }

        // GET: api/Costos/5
        public Costos Get(int id)
        {
            return ClassBD.ObtenerCostoById(id);
        }

        // POST: api/Costos
        public ID Post([FromBody]Costos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarCosto(Datos));
        }

        // PUT: api/Costos
        public void Put([FromBody]Costos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarCosto(Datos);
        }

        // DELETE: api/Costos/5
        public void Delete(int id)
        {
            ClassBD.EliminarCosto(id);
        }
    }
}
