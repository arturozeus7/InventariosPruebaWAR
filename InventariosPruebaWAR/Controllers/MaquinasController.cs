using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class MaquinasController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Maquinas
        public List<Maquinas> Get()
        {
            return ClassBD.ObtenerMaquinas();
        }

        // GET: api/Maquinas/5
        public Maquinas Get(int id)
        {
            return ClassBD.ObtenerMaquinaById(id);
        }

        [Route("api/Maquina/Nombre/{nombre}")]
        public Maquinas Get(string nombre)
        {
            return ClassBD.ObtenerMaquinaByName(nombre);
        }

        [Route("api/Maquina/Proceso/{id}")]
        public List<Maquinas> GetByProceso(int id)
        {
            return ClassBD.ObtenerMaquinasByProceso(id);
        }

        // POST: api/Maquinas
        public ID Post([FromBody]Maquinas Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarMaquina(Datos));
        }

        // PUT: api/Maquinas
        public void Put([FromBody]Maquinas Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarMaquina(Datos);
        }

        // DELETE: api/Maquinas/5
        public void Delete(int id)
        {
            ClassBD.EliminarMaquina(id);
        }
    }
}
