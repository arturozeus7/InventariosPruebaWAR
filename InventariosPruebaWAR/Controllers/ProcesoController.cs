using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class ProcesoController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Proceso
        public List<Proceso> Get()
        {
            return ClassBD.ObtenerProcesos();
        }

        // GET: api/Proceso/5
        public Proceso Get(int id)
        {
            return ClassBD.ObtenerProcesoById(id);
        }

        [Route("api/Proceso/Nombre/{nombre}")]
        public Proceso Get(string nombre)
        {
            return ClassBD.ObtenerProcesoByName(nombre);
        }

        // POST: api/Proceso
        public ID Post([FromBody]Proceso Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarProceso(Datos));
        }

        // PUT: api/Proceso
        public void Put(Proceso Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarProceso(Datos);
        }

        // DELETE: api/Proceso/5
        public void Delete(int id)
        {
            ClassBD.EliminarProceso(id);
        }
    }
}
