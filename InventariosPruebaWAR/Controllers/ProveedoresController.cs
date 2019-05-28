using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class ProveedoresController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Proveedores
        public List<Proveedor> Get()
        {
            return ClassBD.ObtenerProveedor();
        }

        // GET: api/Proveedores/5
        public Proveedor Get(int id)
        {
            return ClassBD.ObtenerProveedorById(id);
        }

        [Route("api/Proveedores/Busqueda/{consulta}")]
        public List<Proveedor> Get(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerProveedorBusquedaDinamica(consulta);
        }

        // POST: api/Proveedores
        public ID Post([FromBody]Proveedor Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarProveedor(Datos);

            return new ID(ret);
        }

        // PUT: api/Proveedores
        public void Put([FromBody]Proveedor Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarProveedor(Datos);
        }

        // DELETE: api/Proveedores/5
        public void Delete(int id)
        {
            ClassBD.EliminarProveedor(id);
        }
    }
}
