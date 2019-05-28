using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class MaterialController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Material
        public List<Material> Get()
        {
            return ClassBD.ObtenerMateriales();
        }

        // GET: api/Material/5
        public Material Get(int id)
        {
            return ClassBD.ObtenerMaterialById(id);
        }

        [Route("api/Material/Nombre/{nombre}")]
        public Material Get(string nombre)
        {
            return ClassBD.ObtenerMaterialByName(nombre);
        }

        // POST: api/Material
        public ID Post([FromBody]Material Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarMaterial(Datos));
        }

        // PUT: api/Material
        public void Put(Material Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarMaterial(Datos);
        }

        // DELETE: api/Material/5
        public void Delete(int id)
        {
            ClassBD.EliminarMaterial(id);
        }
    }
}
