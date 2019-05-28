using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using InventariosPruebaWAR.Models;

namespace InventariosPruebaWAR.Controllers
{
    public class PiezaController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Pieza
        public List<Pieza> Get()
        {
            return ClassBD.ObtenerPieza();
        }

        // GET: api/Pieza/5
        public Pieza Get(int id)
        {
            return ClassBD.ObtenerPiezaById(id);
        }

        [Route("api/Pieza/Nombre/{Nombre}")]
        public Pieza Get(string Nombre)
        {
            return ClassBD.ObtenerPiezaByName(Nombre);
        }


        // POST: api/Pieza
        public ID Post([FromBody]Pieza Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarPieza(Datos));
        }

        // PUT: api/Pieza/5
        public void Put(Pieza Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPieza(Datos);
        }

        // DELETE: api/Pieza/5
        public void Delete(int id)
        {
            ClassBD.EliminarPieza(id);
        }

    }
}
