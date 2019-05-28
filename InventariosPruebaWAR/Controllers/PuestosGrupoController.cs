using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class PuestosGrupoController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/PuestosGrupo
        public List<PuestosGrupo> Get()
        {
            return ClassBD.ObtenerPuestosGrupos();
        }

        // GET: api/PuestosGrupo
        [Route("api/Puestos/Grupo/{id}")]
        public List<PuestosGrupo> Get(int id)
        {
            return ClassBD.ObtenerPuestosGrupoByIdGrupo(id);
        }

        // POST: api/PuestosGrupo
        public ID Post([FromBody]PuestosGrupo Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarPuestosGrupo(Datos));
        }

        [Route("api/Grupo/Puestos")]//Servicio el cual recibira la lista de empleados
        public ID PostListaEmpleadosGrupo([FromBody]List<PuestosGrupo> ListaDatos)
        {
            if (ListaDatos == null || ListaDatos.Count < 1)
                throw new HttpResponseException(HttpStatusCode.BadRequest);


            int ret = 0;
            int idgrupohistorico;
            Grupos Grupo = ClassBD.ObtenerGrupoById(ListaDatos[0].idGrupo);
            idgrupohistorico = ClassBD.AgregarGrupoHistoricoByGrupo(Grupo);


            List<PuestosGrupo> EmpleadosActuales = ClassBD.ObtenerPuestosGrupoByIdGrupo(Grupo.idGrupo); //Comprobar si este id ya tiene puestos para actualizarlos
            if(EmpleadosActuales != null && EmpleadosActuales.Count > 0)
                foreach (PuestosGrupo PuestoExistente in EmpleadosActuales)
                    ClassBD.EliminarPuestosGrupo(PuestoExistente.idPuestoGrupo);


            if (ListaDatos.Count > 0)
            {
                foreach (PuestosGrupo Puesto in ListaDatos)
                {
                    ret += ClassBD.AgregarPuestosGrupo(Puesto);
                    Puesto.idGrupo = idgrupohistorico;
                    ClassBD.AgregarPuestosGrupoHistoricoByPuestosGrupo(Puesto);
                }

            }
            
            return new ID(ret);
        }

        // PUT: api/PuestosGrupo
        public void Put([FromBody]PuestosGrupo Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPuestosGrupo(Datos);
        }

        // DELETE: api/PuestosGrupo/5
        public void Delete(int id)
        {
            ClassBD.EliminarPuestosGrupo(id);
        }
    }
}
