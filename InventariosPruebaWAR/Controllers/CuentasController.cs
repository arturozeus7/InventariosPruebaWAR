using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class CuentasController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Cuentas
        public List<Cuenta> Get()
        {
            return ClassBD.ObtenerCuentas();
        }

        // GET: api/Cuentas/5
        public Cuenta Get(int id)
        {
            return ClassBD.ObtenerCuenta(id);
        }

        [Route("api/Cuentas/Busqueda/{consulta}")]
        public List<Cuenta> Get(string consulta)
        {
            return ClassBD.ObtenerBusquedaCuentas(consulta);
        }

        // POST: api/Cuentas
        public ID Post([FromBody]Cuenta Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarCuenta(Datos));
        }

        // PUT: api/Cuentas
        public void Put([FromBody]Cuenta Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarCuenta(Datos);
        }

        // DELETE: api/Cuentas/5
        public void Delete(int id)
        {
            ClassBD.EliminarCuenta(id);
        }
    }
}
