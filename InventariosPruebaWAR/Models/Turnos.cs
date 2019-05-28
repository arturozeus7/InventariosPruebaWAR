using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Turnos
    {
        protected int m_idTurno;
        protected string m_Turno;
        protected DateTime m_HoraInicio;
        protected DateTime m_HoraFinal;

        public int idTurno { get { return m_idTurno; } set { m_idTurno = value; } }
        public string Turno { get { return m_Turno; } set { m_Turno = value; } }
        public string HoraInicio { get { return m_HoraInicio.ToString("HH:mm"); } set { m_HoraInicio = DateTime.ParseExact(value, "HH:mm", System.Globalization.CultureInfo.InvariantCulture); } }
        public string HoraFinal { get { return m_HoraFinal.ToString("HH:mm"); } set { m_HoraFinal = DateTime.ParseExact(value, "HH:mm", System.Globalization.CultureInfo.InvariantCulture); } }

        public Turnos() { }

        public Turnos(int PidTurno, string PTurno, DateTime PHoraInicio, DateTime PHoraFinal)
        {
            m_idTurno = PidTurno;
            m_Turno = PTurno;
            m_HoraInicio = PHoraInicio;
            m_HoraFinal = PHoraFinal;
        }
    }
}