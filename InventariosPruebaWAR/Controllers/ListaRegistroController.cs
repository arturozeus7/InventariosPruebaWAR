using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class ListaRegistroController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/ListaRegistro
        public List<ListaRegistro> Get()
        {
            return ClassBD.ObtenerListaRegistros();
        }

        // GET: api/Registro/5/ListaRegistro
        [Route("api/Registro/{id}/ListaRegistros")]
        public List<ListaRegistro> Get(int id)
        {
            return ClassBD.ObtenerListaRegistroByIdRegistro(id);
        }

        // POST: api/ListaRegistro
        public ID Post([FromBody]ListaRegistro Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarListaRegistro(Datos));
        }

        // PUT: api/ListaRegistro/5
        public void Put([FromBody]ListaRegistro Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarListaRegistro(Datos);
        }

        // DELETE: api/ListaRegistro/5
        public void Delete(int id)
        {
            ClassBD.EliminarListaRegistro(id);
        }
    }
}
