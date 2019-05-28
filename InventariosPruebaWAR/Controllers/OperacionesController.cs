using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class OperacionesController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Operaciones
        public List<Operaciones> Get()
        {
            return ClassBD.ObtenerOperaciones();
        }

        // GET: api/Operaciones/5
        public Operaciones Get(int id)
        {
            return ClassBD.ObtenerOperacionById(id);
        }

        [Route("api/Operaciones/Nombre/{nombre}")]
        public Operaciones Get(string nombre)
        {
            return ClassBD.ObtenerOperacionByTipo(nombre);
        }

        // POST: api/Operaciones
        public ID Post([FromBody]Operaciones Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarOperacion(Datos));
        }

        // PUT: api/Operaciones
        public void Put([FromBody]Operaciones Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarOperacion(Datos);
        }

        // DELETE: api/Operaciones/5
        public void Delete(int id)
        {
            ClassBD.EliminarOperacion(id);
        }
    }
}
