using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class ClientesController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Clientes
        public List<Cliente> Get()
        {
            return ClassBD.ObtenerCliente();
        }

        // GET: api/Clientes/5
        public Cliente Get(int id)
        {
            return ClassBD.ObtenerClienteById(id);
        }

        [Route("api/Clientes/Busqueda/{consulta}")]
        public List<Cliente> Get(string consulta)
        {
            return ClassBD.ObtenerBusquedaCliente(consulta);
        }

        // POST: api/Clientes
        public ID Post([FromBody]Cliente Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarCliente(Datos));
        }

        // PUT: api/Clientes
        public void Put([FromBody]Cliente Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarCliente(Datos);
        }

        // DELETE: api/Clientes/5
        public void Delete(int id)
        {
            ClassBD.EliminarCliente(id);
        }
    }
}
