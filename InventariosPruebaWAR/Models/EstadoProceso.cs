using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class EstadoProceso
    {
        protected int m_idEstatus;
        protected string m_Estatus;

        public int idEstatus { get { return m_idEstatus; } set { m_idEstatus = value; } }
        public string Estatus { get { return m_Estatus; } set { m_Estatus = value; } }

        public EstadoProceso() { }

        public EstadoProceso(int PidEstatus, string PEstatus)
        {
            m_idEstatus = PidEstatus;
            m_Estatus = PEstatus;
        }
    }
}
