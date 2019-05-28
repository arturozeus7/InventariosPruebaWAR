using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class MonedaController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Moneda
        public List<Moneda> Get()
        {
            return ClassBD.ObtenerMoneda();
        }

        // GET: api/Moneda/5
        public Moneda Get(int id)
        {
            return ClassBD.ObtenerMonedaById(id);
        }

        [Route("api/Monedas/Nombre/{nombre}")]
        public Moneda Get(string nombre)
        {
            return ClassBD.ObtenerMonedaByName(nombre);
        }

        // POST: api/Moneda
        public ID Post([FromBody]Moneda Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarMoneda(Datos));
        }

        // PUT: api/Moneda
        public void Put([FromBody]Moneda Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarMoneda(Datos);
        }

        // DELETE: api/Moneda
        public void Delete(int id)
        {
            ClassBD.EliminarMoneda(id);
        }
    }
}
