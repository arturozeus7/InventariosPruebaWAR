using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class TransferenciasController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Transferencias
        public List<TransferSuc> Get()
        {
            return ClassBD.ObtenerTransferencias();
        }

        // GET: api/Transferencias/5
        public TransferSuc Get(int id)
        {
            return ClassBD.ObtenerTransferenciaById(id);
        }

        [Route("api/Transferencias/Busqueda/{consulta}")]
        public List<TransferSuc> Get(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerBusquedaTransferencias(consulta);
        }

        // POST: api/Transferencias
        public ID Post([FromBody]TransferSuc Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarTransfer(Datos);

            return new ID(ret);
        }

        // PUT: api/Transferencias
        public void Put([FromBody]TransferSuc Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTransfer(Datos);
        }

        // DELETE: api/Transferencias/5
        public void Delete(int id)
        {
            ClassBD.EliminarTransfer(id);
        }
    }
}
