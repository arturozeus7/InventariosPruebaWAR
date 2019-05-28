using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class TurnosRotativosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/TurnosRotativos
        public List<TurnosRotativos> Get()
        {
            return ClassBD.ObtenerTurnosRotatativos();
        }

        // GET: api/TurnosRotativos/5
        public TurnosRotativos Get(int id)
        {
            return ClassBD.ObtenerTurnoRotatativoById(id);
        }

        // POST: api/TurnosRotativos
        public ID Post([FromBody]TurnosRotativos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarTurnoRotatativo(Datos));
        }

        // PUT: api/TurnosRotativos/5
        public void Put([FromBody]TurnosRotativos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTurnoRotatativo(Datos);
        }

        // DELETE: api/TurnosRotativos/5
        public void Delete(int id)
        {
            ClassBD.EliminarTurnoRotatativo(id);
        }
    }
}
