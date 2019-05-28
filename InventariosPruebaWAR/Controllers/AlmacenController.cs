using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class AlmacenController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Almacen
        public List<Almacen> Get()
        {
            return ClassBD.ObtenerAlmacenes();
        }

        // GET: api/Almacen/5
        public Almacen Get(int id)
        {
            return ClassBD.ObtenerAlmacenById(id);
        }

        [Route("api/Almacen/Nombre/{nombre}")]
        public Almacen Get(string nombre)
        {
            return ClassBD.ObtenerAlmacenByName(nombre);
        }

        [Route("api/Almacen/Proceso/{id}")]
        public List<Almacen> GetByProceso(int id)
        {
            return ClassBD.ObtenerAlmacenByProceso(id);
        }

        // POST: api/Almacen
        public ID Post([FromBody]Almacen Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarAlmacen(Datos));
        }

        // PUT: api/Almacen/5
        public void Put(Almacen Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarAlmacen(Datos);
        }

        // DELETE: api/Almacen/5
        public void Delete(int id)
        {
            ClassBD.EliminarAlmacen(id);
        }
    }
}
