using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class EstadosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Estados
        public List<Estados> Get()
        {
            return ClassBD.ObtenerEstados();
        }

        // GET: api/Estados/5
        public Estados Get(int id)
        {
            return ClassBD.ObtenerEstadoById(id);
        }

        [Route("api/Estados/Nombre/{nombre}")]
        public Estados Get(string nombre)
        {
            return ClassBD.ObtenerEstadoByName(nombre);
        }

        // POST: api/Estados
        public ID Post([FromBody]Estados Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarEstado(Datos));
        }

        // PUT: api/Estados/5
        public void Put(int id, [FromBody]Estados Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarEstado(Datos);
        }

        // DELETE: api/Estados/5
        public void Delete(int id)
        {
            ClassBD.EliminarEstado(id);
        }
    }
}
