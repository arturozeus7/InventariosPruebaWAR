using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class RegistroController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Registro
        public List<Registro> Get()
        {
            return ClassBD.ObtenerRegistros();
        }

        // GET: api/Registro/Empleado/5
        [Route("api/Registros/Empleado/{id}")]
        public List<Registro> Get(int id)
        {
            return ClassBD.ObtenerRegistroByIdEmpleado(id);
        }

        [Route("api/Registros/Completos")]
        public List<Registro> GetCompletos()
        {
            return ClassBD.ObtenerRegistrosCompletos();
        }

        // POST: api/Registro
        public ID Post([FromBody]Registro Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarRegistro(Datos));
        }

        [Route("api/Registro/Completo")]
        public ID PostCompleto([FromBody]Registro Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarRegistro(Datos);

            if(Datos.ListaRegistros != null && ret > 0)
                foreach (ListaRegistro Registro in Datos.ListaRegistros)
                {
                    Registro.idRegistro = ret;
                    ClassBD.AgregarListaRegistro(Registro);
                }

            return new ID(ret);
        }

        // PUT: api/Registro/5
        public void Put([FromBody]Registro Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarRegistro(Datos);
        }

        // DELETE: api/Registro/5
        public void Delete(int id)
        {
            List<ListaRegistro> ListaRegistros = ClassBD.ObtenerListaRegistroByIdRegistro(id);
            foreach (ListaRegistro Registro in ListaRegistros)
                ClassBD.EliminarListaRegistro(Registro.idListaRegistro);

            ClassBD.EliminarRegistro(id);
        }
    }
}
