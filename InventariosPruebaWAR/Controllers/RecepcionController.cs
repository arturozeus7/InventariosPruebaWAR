using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class RecepcionController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Recepcion
        public List<Recepcion> Get()
        {
            return ClassBD.ObtenerRecepciones();
        }

        // GET: api/Recepcion/5
        public Recepcion Get(int id)
        {
            return ClassBD.ObtenerRecepcionRecicladoraById(id);
        }

        [Route("api/Terminar/Recepcion/{id}")]
        public Respuesta GetTerminar(int id)
        {
            List<Paquete> PaquetesRecepcion = ClassBD.ObtenerPaquetesByRecepcion(id);
            int Terminados = 0;
            string noTerminados ="";
            Respuesta Response = new Respuesta();

            foreach (Paquete PaqueteRecepcion in PaquetesRecepcion)
            {
                if (PaqueteRecepcion.idEstatus != 1 && PaqueteRecepcion.idEstatus != 2)
                    Terminados++;
                else
                    noTerminados += PaqueteRecepcion + "\n";
            }
            if (Terminados == PaquetesRecepcion.Count)
            {
                Response.Resultado = true;
                Response.Texto = "Todos los paquetes han sido Finalizados";
            }
            else
            {
                Response.Resultado = false;
                Response.Texto = "Los siguientes paquetes no han sido Finalizados: \n\n";
            }
            return Response;
        }

        // POST: api/Recepcion
        public ID Post([FromBody]Recepcion Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            ret = ClassBD.AgregarRecepcion(Datos);

            if (ret > 0 && Datos.idPedido != 0)
                ClassBD.AsignarRecepcionaPedido(ret, Datos.idPedido);
            return new ID(ret);
        }

        // PUT: api/Recepcion
        public void Put([FromBody]Recepcion Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarRecepcion(Datos);
        }

        // DELETE: api/Recepcion/5
        public void Delete(int id)
        {
            ClassBD.EliminarRecepcionReci(id);
        }
    }
}
