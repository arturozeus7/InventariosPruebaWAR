using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class PuestosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Puestos
        public List<Puestos> Get()
        {
            return ClassBD.ObtenerPuestos();
        }

        // GET: api/Puestos/5
        public Puestos Get(int id)
        {
            return ClassBD.ObtenerPuestoById(id);
        }


        [Route("api/Puesto/Nombre/{nombre}")]
        public Puestos Get(string nombre)
        {
            return ClassBD.ObtenerPuestoByName(nombre);
        }

        // POST: api/Puestos
        public ID Post([FromBody]Puestos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarPuesto(Datos));
        }

        // PUT: api/Puestos/5
        public void Put([FromBody]Puestos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPuesto(Datos);
        }

        // DELETE: api/Puestos/5
        public void Delete(int id)
        {
            ClassBD.EliminarPuestos(id);
        }
    }
}
