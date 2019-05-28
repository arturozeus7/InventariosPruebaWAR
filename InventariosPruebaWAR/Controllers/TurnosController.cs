using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class TurnosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Turnos
        public List<Turnos> Get()
        {
            return ClassBD.ObtenerTurnos();
        }

        // GET: api/Turnos/5
        public Turnos Get(int id)
        {
            return ClassBD.ObtenerTurnoById(id);
        }

        [Route("api/Turno/Actual")]
        public Turnos GetTurnoByHora()
        {
            List<Turnos> Turnos = ClassBD.ObtenerTurnos();
            string Hora = DateTime.Now.ToString("HH:mm");
            DateTime HoraActual = DateTime.ParseExact(Hora, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
            Turnos TurnoActual = new Turnos();
            foreach (Turnos Turno in Turnos)
            {
                if (HoraActual >= DateTime.ParseExact(Turno.HoraInicio, "HH:mm", System.Globalization.CultureInfo.CurrentCulture) && HoraActual <= DateTime.ParseExact(Turno.HoraFinal, "HH:mm", System.Globalization.CultureInfo.CurrentCulture))
                    TurnoActual = Turno;
            }
            return TurnoActual;
        }

        [Route("api/Turno/Nombre/{nombre}")]
        public Turnos Get(string nombre)
        {
            return ClassBD.ObtenerTurnoByName(nombre);
        }

        // POST: api/Turnos
        public ID Post([FromBody]Turnos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarTurno(Datos));
        }

        // PUT: api/Turnos
        public void Put([FromBody]Turnos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarTurno(Datos);
        }

        // DELETE: api/Turnos/5
        public void Delete(int id)
        {
            ClassBD.EliminarTurno(id);
        }
    }
}
