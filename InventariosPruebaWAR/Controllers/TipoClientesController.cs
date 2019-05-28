using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class TipoClientesController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/TipoClientes
        public List<TipoClientes> Get()
        {
            return ClassBD.ObtenerTipoClientes();
        }

        // GET: api/TipoClientes/5
        public TipoClientes Get(int id)
        {
            return ClassBD.ObtenerTipoClienteById(id);
        }

        [Route("api/TipoClientes/Nombre/{nombre}")]
        public TipoClientes Get(string nombre)
        {
            return ClassBD.ObtenerTipoClienteByTipo(nombre);
        }

        // POST: api/TipoClientes
        public ID Post([FromBody]TipoClientes Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarTipoCliente(Datos);

            return new ID(ret);
        }

        // PUT: api/TipoClientes
        public void Put([FromBody]TipoClientes Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTipoCliente(Datos);
        }

        // DELETE: api/TipoClientes/5
        public void Delete(int id)
        {
            ClassBD.EliminarTipoCliente(id);
        }
    }
}
