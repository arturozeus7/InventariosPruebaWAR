using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Grupos
    {
        protected int m_idGrupo;
        protected string m_Grupo;
        protected List<PuestosGrupo> m_Puestos;
        //protected DateTime m_Fecha;
        //protected int m_idTurno;
        //protected string m_Turno;

        public int idGrupo { get { return m_idGrupo; } set { m_idGrupo = value; } }
        public string Grupo { get { return m_Grupo; } set { m_Grupo = value; } }
        public List<PuestosGrupo> Puestos { get { return m_Puestos; } set { m_Puestos = value; } }
        //public string Fecha { get { return m_Fecha.ToString("yyyy-MM-dd"); } set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        //public int idTurno { get { return m_idTurno; } set { m_idTurno = value; } }
        //public string Turno { get { return m_Turno; } set { m_Turno = value; } }

        public Grupos() { }

        public Grupos(int PidGrupo,/* DateTime PFecha, int PidTurno, string PTurno, */string PGrupo, List<PuestosGrupo> PPuestos = null)
        {
            m_idGrupo = PidGrupo;
            //m_Fecha = PFecha;
            //m_idTurno = PidTurno;
            //m_Turno = PTurno;
            m_Grupo = PGrupo;
            m_Puestos = PPuestos;
        }
    }
}