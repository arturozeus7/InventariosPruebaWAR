using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class EmpresaController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Empresa
        public List<Empresa> Get()
        {
            return ClassBD.ObtenerEmpresa();
        }

        // GET: api/Empresa/5
        public Empresa Get(int id)
        {
            return ClassBD.ObtenerEmpresaById(id);
        }

        [Route("api/Empresa/Nombre/{nombre}")]
        public Empresa Get(string nombre)
        {
            return ClassBD.ObtenerEmpresaByName(nombre);
        }

        // POST: api/Empresa
        public ID Post([FromBody]Empresa Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarEmpresa(Datos));
        }

        // PUT: api/Empresa
        public void Put([FromBody]Empresa Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarEmpresa(Datos);
        }

        // DELETE: api/Empresa/5
        public void Delete(int id)
        {
            ClassBD.EliminarEmpresa(id);
        }
    }
}
