using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class IVAsController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/IVAs
        public List<IVAs> Get()
        {
            return ClassBD.ObtenerIVAs();
        }

        // GET: api/IVAs/5
        public IVAs Get(int id)
        {
            return ClassBD.ObtenerIVAById(id);
        }

        [Route("api/IVAs/IVA/{ValorIVA}")]
        public IVAs Get(string ValorIVA)
        {
            return ClassBD.ObtenerIVAByIVA(Convert.ToDouble(ValorIVA));
        }

        // POST: api/IVAs
        public ID Post([FromBody]IVAs Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarIVA(Datos));
        }

        // PUT: api/IVAs
        public void Put([FromBody]IVAs Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarIVA(Datos);
        }

        // DELETE: api/IVAs/5
        public void Delete(int id)
        {
            ClassBD.EliminarIVA(id);
        }
    }
}
