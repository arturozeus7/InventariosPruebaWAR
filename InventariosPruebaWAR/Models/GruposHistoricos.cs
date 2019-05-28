using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class GruposHistoricos
    {
        protected int m_idGrupoHistorico;
        protected string m_Grupo;
        protected DateTime m_Fecha;
        //protected int m_idTurno;
        //protected string m_Turno;

        public int idGrupoHistorico { get { return m_idGrupoHistorico; } set { m_idGrupoHistorico = value; } }
        public string Grupo { get { return m_Grupo; } set { m_Grupo = value; } }
        public string Fecha { get { return m_Fecha.ToString("yyyy-MM-dd"); } set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        //public int idTurno { get { return m_idTurno; } set { m_idTurno = value; } }
        //public string Turno { get { return m_Turno; } set { m_Turno = value; } }

        public GruposHistoricos() { }

        public GruposHistoricos(int PidGrupo, DateTime PFecha,/* int PidTurno, string PTurno, */string PGrupo)
        {
            m_idGrupoHistorico = PidGrupo;
            m_Fecha = PFecha;
            //m_idTurno = PidTurno;
            //m_Turno = PTurno;
            m_Grupo = PGrupo;
        }
    }
}