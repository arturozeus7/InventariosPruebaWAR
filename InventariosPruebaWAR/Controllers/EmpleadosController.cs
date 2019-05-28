using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class EmpleadosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Empleados
        public List<Empleado> Get()
        {
            return ClassBD.ObtenerEmpleado();
        }

        // GET: api/Empleados/5
        public Empleado Get(int id)
        {
            return ClassBD.ObtenerEmpleadoById(id);
        }

        [Route("api/Empleados/Busqueda/{consulta}")]
        public List<Empleado> Get(string consulta)
        {
            return ClassBD.ObtenerBusquedaEmpleado(consulta);
        }

        
        [Route("api/Empleados/Usuario/{Empleado}")]
        public Empleado GetUsuario(string Empleado)
        {
            return ClassBD.ObtenerEmpleadoByName(Empleado);
        }

        // POST: api/Empleados
        public ID Post([FromBody]Empleado Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarEmpleado(Datos));
        }

        // PUT: api/Empleados
        public void Put([FromBody]Empleado Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarEmpleado(Datos);
        }

        [Route("api/Empleados/CambiarPS")]
        public void PutPS([FromBody]Empleado Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPassEmpleado(Datos);
        }

        // DELETE: api/Empleados/5
        public void Delete(int id)
        {
            ClassBD.EliminarEmpleado(id);
        }
    }
}
