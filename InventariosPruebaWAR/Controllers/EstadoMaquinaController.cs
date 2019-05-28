using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class EstadoMaquinaController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/EstadoMaquina
        public List<EstadoMaquina> Get()
        {
            return ClassBD.ObtenerEstadosMaquina();
        }

        // GET: api/EstadoMaquina/5
        public EstadoMaquina Get(int id)
        {
            return ClassBD.ObtenerEstadoMaquinaById(id);
        }


        [Route("api/EstadoMaquina/Nombre/{nombre}")]
        public EstadoMaquina Get(string nombre)
        {
            return ClassBD.ObtenerEstadoMaquinaByName(nombre);
        }

        // POST: api/EstadoMaquina
        public ID Post([FromBody]EstadoMaquina Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarEstadoMaquina(Datos));
        }

        // PUT: api/EstadoMaquina
        public void Put([FromBody]EstadoMaquina Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarEstadoMaquina(Datos);
        }

        // DELETE: api/EstadoMaquina/5
        public void Delete(int id)
        {
            ClassBD.EliminarEstadoMaquina(id);
        }
    }
}
