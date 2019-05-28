using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class PaisesController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Paises
        public List<Paises> Get()
        {
            return ClassBD.ObtenerPais();
        }

        // GET: api/Paises/5
        public Paises Get(int id)
        {
            return ClassBD.ObtenerPaisById(id);
        }

        // GET: api/Paises/5
        [Route("api/Paises/Nombre/{nombre}")]
        public Paises Get(string nombre)
        {
            return ClassBD.ObtenerPaisByName(nombre);
        }

        // POST: api/Paises
        public ID Post([FromBody]Paises Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarPais(Datos));
        }

        // PUT: api/Paises
        public void Put([FromBody]Paises Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPais(Datos);
        }

        // DELETE: api/Paises/5
        public void Delete(int id)
        {
            ClassBD.EliminarPais(id);
        }
    }
}
