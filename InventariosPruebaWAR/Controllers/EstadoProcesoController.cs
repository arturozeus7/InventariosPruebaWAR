using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class EstadoProcesoController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/EstadoProceso
        public List<EstadoProceso> Get()
        {
            return ClassBD.ObtenerEstadoProceso();
        }

        // GET: api/EstadoProceso/5
        public EstadoProceso Get(int id)
        {
            return ClassBD.ObtenerEstatusById(id);
        }

        [Route("api/EstadoProceso/Nombre/{nombre}")]
        public EstadoProceso Get(string nombre)
        {
            return ClassBD.ObtenerEstatusByName(nombre);
        }

        // POST: api/EstadoProceso
        public ID Post([FromBody]EstadoProceso Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarEstatus(Datos));
        }

        // PUT: api/EstadoProceso
        public void Put([FromBody]EstadoProceso Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarEstatus(Datos);
        }

        // DELETE: api/EstadoProceso/5
        public void Delete(int id)
        {
            ClassBD.EliminarEstatus(id);
        }
    }
}
