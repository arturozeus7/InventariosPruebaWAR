using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class TipoCuentasController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/TipoCuentas
        public List<TipoCuenta> Get()
        {
            return ClassBD.ObtenerTipoCuentas();
        }

        // GET: api/TipoCuentas/5
        public TipoCuenta Get(int id)
        {
            return ClassBD.ObtenerTipoCuenta(id);
        }

        // POST: api/TipoCuentas
        public ID Post([FromBody]TipoCuenta Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarTipoCuenta(Datos);

            return new ID(ret);
        }

        // PUT: api/TipoCuentas
        public void Put([FromBody]TipoCuenta Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTipoCuenta(Datos);
        }

        // DELETE: api/TipoCuentas/5
        public void Delete(int id)
        {
            ClassBD.EliminarTipoCuenta(id);
        }
    }
}
