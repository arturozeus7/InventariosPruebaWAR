using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Estados
    {
        protected int m_idEstado;
        protected string m_Estado;
        protected int m_idPais;
        protected string m_Pais;

        public int idEstado { get { return m_idEstado; } set { m_idEstado = value; } }
        public string Estado { get { return m_Estado; } set { m_Estado = value; } }
        public int idPais { get { return m_idPais; } set { m_idPais = value; } }
        public string Pais { get { return m_Pais; } set { m_Pais = value; } }

        public Estados() { }

        public Estados(int PidEstado, string PEstado, int PidPais, string PPais)
        {
            m_idEstado = PidEstado;
            m_Estado = PEstado;
            m_idPais = PidPais;
            m_Pais = PPais;
        }
    }
}
