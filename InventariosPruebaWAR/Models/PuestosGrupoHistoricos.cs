using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class PuestosGrupoHistoricos
    {
        protected int m_idPuestoGrupo;
        protected int m_idEmpleado;
        protected string m_Empleado;
        protected int m_idPuesto;
        protected string m_Puesto;
        protected int m_idGrupoHistorico;

        public int idPuestoGrupo { get { return m_idPuestoGrupo; } set { m_idPuestoGrupo = value; } }
        public int idEmpleado { get { return m_idEmpleado; } set { m_idEmpleado = value; } }
        public string Empleado { get { return m_Empleado; } set { m_Empleado = value; } }
        public int idPuesto { get { return m_idPuesto; } set { m_idPuesto = value; } }
        public string Puesto { get { return m_Puesto; } set { m_Puesto = value; } }
        public int idGrupoHistorico { get { return m_idGrupoHistorico; } set { m_idGrupoHistorico = value; } }

        public PuestosGrupoHistoricos() { }

        public PuestosGrupoHistoricos(int PidPuestosGrupo, int PidEmpleado, string PEmpleado, int PidPuesto, string PPuesto, int PidGrupoHistorico)
        {
            m_idPuestoGrupo = PidPuestosGrupo;
            m_idEmpleado = PidEmpleado;
            m_Empleado = PEmpleado;
            m_idPuesto = PidPuesto;
            m_Puesto = PPuesto;
            m_idGrupoHistorico = PidGrupoHistorico;
        }
    }
}