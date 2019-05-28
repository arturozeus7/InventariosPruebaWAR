using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Paises
    {
        protected int m_idPais;
        protected string m_Pais;

        public int idPais { get { return m_idPais; } set { m_idPais = value; } }
        public string Pais { get { return m_Pais; } set { m_Pais = value; } }

        public Paises() { }

        public Paises(int PidPais, string PPais)
        {
            m_idPais = PidPais;
            m_Pais = PPais;
        }
    }
}
