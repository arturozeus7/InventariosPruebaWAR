using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Privilegios
    {
        protected int m_idPrivilegio;
        protected string m_Privilegio;

        public int idPrivilegio { get { return m_idPrivilegio; } set { m_idPrivilegio = value; } }
        public string Privilegio { get { return m_Privilegio; } set { m_Privilegio = value; } }

        public Privilegios() { }

        public Privilegios(int PidPrivilegio, string PPrivilegio)
        {
            m_idPrivilegio = PidPrivilegio;
            m_Privilegio = PPrivilegio;
        }
    }
}
