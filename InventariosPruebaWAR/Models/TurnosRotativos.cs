using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class TurnosRotativos
    {
        protected int m_idTurnoRotativo;
        protected DateTime m_Dia;
        protected int m_idTurno;
        protected string m_Turno;
        protected int m_idGrupo;
        protected string m_Grupo;

        public int idTurnoRotativo { get { return m_idTurnoRotativo; } set { m_idTurnoRotativo = value; } }
        public string Dia { get { return m_Dia.ToString("yyyy-MM-dd"); } set { m_Dia = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        public int idTurno { get { return m_idTurno; } set { m_idTurno = value; } }
        public string Turno { get { return m_Turno; } set { m_Turno = value; } }
        public int idGrupo { get { return m_idGrupo; } set { m_idGrupo = value; } }
        public string Grupo { get { return m_Grupo; } set { m_Grupo = value; } }

        public TurnosRotativos() { }

        public TurnosRotativos(int PidTurnoRotativo, DateTime PDia, int PidTurno, string PTurno, int PidGrupo, string Pgrupo)
        {
            m_idTurnoRotativo = PidTurnoRotativo;
            m_Dia = PDia;
            m_idTurno = PidTurno;
            m_Turno = PTurno;
            m_idGrupo = PidGrupo;
            m_Grupo = Pgrupo;
        }
    }
}