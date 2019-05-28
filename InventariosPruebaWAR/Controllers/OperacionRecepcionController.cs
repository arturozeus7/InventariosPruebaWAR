using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class OperacionRecepcionController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/OperacionRecepcion
        public List<OperacionRecepcion> Get()
        {
            return ClassBD.ObtenerOperacionesRecepcion();
        }

        // GET: api/OperacionRecepcion/5
        public OperacionRecepcion Get(int id)
        {
            return ClassBD.ObtenerOperacionRecepcionById(id);
        }

        [Route("api/OperacionRecepcion/Nombre/{nombre}")]
        public OperacionRecepcion Get(string nombre)
        {
            return ClassBD.ObtenerOperacionRecepcionByName(nombre);
        }

        // POST: api/OperacionRecepcion
        public ID Post([FromBody]OperacionRecepcion Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarOperacionRecepcion(Datos));
        }

        // PUT: api/OperacionRecepcion/5
        public void Put(OperacionRecepcion Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarOperacionRecepcion(Datos);
        }

        // DELETE: api/OperacionRecepcion/5
        public void Delete(int id)
        {
            ClassBD.EliminarOperacionRecepcion(id);
        }
    }
}
