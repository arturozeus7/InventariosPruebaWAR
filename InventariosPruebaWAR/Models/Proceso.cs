using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Proceso
    {
        protected int m_idProceso;
        protected string m_NombreProceso;
        protected string m_Codigo;
        protected double m_PrecioKG;

        public int idProceso { get { return m_idProceso; } set { m_idProceso = value; } }
        public string NombreProceso { get { return m_NombreProceso; } set { m_NombreProceso = value; } }
        public string Codigo { get { return m_Codigo; } set { m_Codigo = value; } }
        public double PrecioKG { get { return m_PrecioKG; } set { m_PrecioKG = value; } }

        public Proceso() { }

        public Proceso(int PidProceso, string PNombre, string PCodigo, double PPrecioKG)
        {
            m_idProceso = PidProceso;
            m_NombreProceso = PNombre;
            m_Codigo = PCodigo;
            m_PrecioKG = PPrecioKG;
        }
    }
}