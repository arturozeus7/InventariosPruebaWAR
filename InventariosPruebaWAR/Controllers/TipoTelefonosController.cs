using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class TipoTelefonosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/TipoTelefonos
        public List<TipoTelefono> Get()
        {
            return ClassBD.ObtenerTipoTelefono();
        }

        // GET: api/TipoTelefonos/5
        public TipoTelefono Get(int id)
        {
            return ClassBD.ObtenerTipoTelefonoById(id);
        }

        [Route("api/TipoTelefonos/Nombre/{nombre}")]
        public List<TipoTelefono> Get(string nombre)
        {
            return ClassBD.ObtenerTipoTelefonoByTipo(nombre);
        }

        // POST: api/TipoTelefonos
        public ID Post([FromBody]TipoTelefono Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarTipoTelefono(Datos);

            return new ID(ret);
        }

        // PUT: api/TipoTelefonos
        public void Put([FromBody]TipoTelefono Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTipoTelefono(Datos);
        }

        // DELETE: api/TipoTelefonos/5
        public void Delete(int id)
        {
            ClassBD.EliminarTipoTelefono(id);
        }
    }
}
