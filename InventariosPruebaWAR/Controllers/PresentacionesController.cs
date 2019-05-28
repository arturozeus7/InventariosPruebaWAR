using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class PresentacionesController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Presentaciones
        public List<Presentaciones> Get()
        {
            return ClassBD.ObtenerPresentaciones();
        }

        // GET: api/Presentaciones/5
        public Presentaciones Get(int id)
        {
            return ClassBD.ObtenerPresentacionById(id);
        }

        [Route("api/Presentaciones/Nombre/{nombre}")]
        public Presentaciones Get(string nombre)
        {
            return ClassBD.ObtenerPresentacionByName(nombre);
        }

        // POST: api/Presentaciones
        public ID Post([FromBody]Presentaciones Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarPresentacion(Datos);

            return new ID(ret);
        }

        // PUT: api/Presentaciones
        public void Put([FromBody]Presentaciones Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPresentacion(Datos);
        }

        // DELETE: api/Presentaciones/5
        public void Delete(int id)
        {
            ClassBD.EliminarPresentacion(id);
        }
    }
}
