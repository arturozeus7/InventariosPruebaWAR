using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class BancosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Bancos
        public List<Bancos> Get()
        {
            return ClassBD.ObtenerBancos();
        }

        // GET: api/Bancos/5
        public Bancos Get(int id)
        {
            return ClassBD.ObtenerBancoById(id);
        }

        
        [Route("api/Bancos/Nombre/{nombre}")]
        public Bancos Get(string nombre)
        {
            return ClassBD.ObtenerBancoByName(nombre);
        }

        // POST: api/Bancos
        public ID Post([FromBody]Bancos Banco)
        {
            if (Banco == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarBanco(Banco));
        }

        // PUT: api/Bancos
        public void Put(Bancos Banco)
        {
            if(Banco == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarBanco(Banco);
        }

        // DELETE: api/Bancos/5
        public void Delete(int id)
        {
            ClassBD.EliminarBanco(id);
        }
    }
}
