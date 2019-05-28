using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class InsumosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Insumos
        public List<Insumos> Get()
        {
            return ClassBD.ObtenerInsumos();
        }

        // GET: api/Insumos/5
        public Insumos Get(int id)
        {
            return ClassBD.ObtenerInsumoById(id);
        }


        [Route("api/Insumos/Nombre/{nombre}")]
        public Insumos Get(string nombre)
        {
            return ClassBD.ObtenerInsumoByName(nombre);
        }

        // POST: api/Insumos
        public ID Post([FromBody]Insumos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarInsumos(Datos));
        }

        // PUT: api/Insumos
        public void Put([FromBody]Insumos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarInsumos(Datos);
        }

        // DELETE: api/Insumos/5
        public void Delete(int id)
        {
            ClassBD.EliminarInsumos(id);
        }
    }
}
