using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class CategoriaController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Categoria
        public List<Categoria> Get()
        {
            return ClassBD.ObtenerCategoria();
        }

        // GET: api/Categoria/5
        public Categoria Get(int id)
        {
            return ClassBD.ObtenerCategoriaById(id);
        }

        [Route("api/Categoria/Nombre/{nombre}")]
        public Categoria Get(string nombre)
        {
            return ClassBD.ObtenerCategoriaByName(nombre);
        }

        // POST: api/Categoria
        public ID Post(Categoria Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarCategoria(Datos));
        }

        // PUT: api/Categoria
        public void Put(Categoria Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarCategoria(Datos);
        }

        // DELETE: api/Categoria/5
        public void Delete(int id)
        {
            ClassBD.EliminarCategoria(id);
        }
    }
}
