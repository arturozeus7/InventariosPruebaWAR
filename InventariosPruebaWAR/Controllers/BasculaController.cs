using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class BasculaController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Bascula
        public List<Bascula> Get()
        {
            return ClassBD.ObtenerBasculas();
        }

        // GET: api/Bascula/5
        public Bascula Get(int id)
        {
            return ClassBD.ObtenerBasculaById(id);
        }

        [Route("api/Bascula/Modelo/{modelo}")]
        public Bascula Get(string modelo)
        {
            return ClassBD.ObtenerBasculaByName(modelo);
        }

        // POST: api/Bascula
        public ID Post([FromBody]Bascula Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarBascula(Datos));
        }

        // PUT: api/Bascula/5
        public void Put(Bascula Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarBascula(Datos);
        }

        // DELETE: api/Bascula/5
        public void Delete(int id)
        {
            ClassBD.EliminarBascula(id);
        }
    }
}
