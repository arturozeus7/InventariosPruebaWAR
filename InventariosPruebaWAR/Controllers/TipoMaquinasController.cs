using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class TipoMaquinasController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/TipoMaquinas
        public List<TipoMaquinas> Get()
        {
            return ClassBD.ObtenerTipoMaquinas();
        }

        // GET: api/TipoMaquinas/5
        public TipoMaquinas Get(int id)
        {
            return ClassBD.ObtenerTipoMaquinaById(id);
        }

        [Route("api/TipoMaquinas/Nombre/{nombre}")]
        public TipoMaquinas Get(string nombre)
        {
            return ClassBD.ObtenerTipoMaquinaByName(nombre);
        }

        // POST: api/TipoMaquinas
        public ID Post([FromBody]TipoMaquinas Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarTipoMaquina(Datos));
        }

        // PUT: api/TipoMaquinas
        public void Put([FromBody]TipoMaquinas Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTipoMaquina(Datos);
        }

        // DELETE: api/TipoMaquinas/5
        public void Delete(int id)
        {
            ClassBD.EliminarTipoMaquina(id);
        }
    }
}
