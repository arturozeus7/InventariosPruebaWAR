using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class SucursalController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Sucursal
        public List<Sucursal> Get()
        {
            return ClassBD.ObtenerSucursal();
        }

        // GET: api/Sucursal/5
        public Sucursal Get(int id)
        {
            return ClassBD.ObtenerSucursalById(id);
        }

        [Route("api/Sucursal/Nombre/{nombre}")]
        public Sucursal GetSucursalByName(string nombre)
        {
            return ClassBD.ObtenerSucursalByName(nombre);
        }

        [Route("api/Sucursal/Busqueda/{consulta}")]
        public List<Sucursal> GetBusquedaSucursal(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerBusquedaSucursal(consulta);
        }

        // POST: api/Sucursal
        public ID Post([FromBody]Sucursal Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarSucursal(Datos);

            return new ID(ret);
        }

        // PUT: api/Sucursal
        public void Put([FromBody]Sucursal Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarSucursal(Datos);
        }

        // DELETE: api/Sucursal/5
        public void Delete(int id)
        {
            ClassBD.EliminarSucursal(id);
        }
    }
}
