using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class GruposController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Grupos
        public List<Grupos> Get()
        {
            return ClassBD.ObtenerGrupos();
        }

        // GET: api/Grupos/5
        public Grupos Get(int id)
        {
            return ClassBD.ObtenerGrupoById(id);
        }

        [Route("api/Grupo/Completo/{id}")]
        public Grupos GetGrupoCompletoById(int id)
        {
            return ClassBD.ObtenerGrupoCompleto(id);
        }

        // POST: api/Grupos
        public ID Post([FromBody]Grupos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarGrupo(Datos);
            return new ID(ret);
        }

        // PUT: api/Grupos
        public void Put([FromBody]Grupos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarGrupo(Datos);
        }

        // DELETE: api/Grupos/5
        public void Delete(int id)
        {
            ClassBD.EliminarGrupo(id);
        }
    }
}
