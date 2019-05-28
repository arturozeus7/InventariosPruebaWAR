using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class NumeroTelefonico
    {
        protected int m_idNumTelefonico;
        protected string m_NumTelefonico;
        protected int m_idTipoNumTelefono;
        protected string m_TipoNumTelefono;

        public int idNumTelefonico { get { return m_idNumTelefonico; } set { m_idNumTelefonico = value; } }
        public string NumTelefonico { get { return m_NumTelefonico; } set { m_NumTelefonico = value; } }
        public int idTipoNumTelefono { get { return m_idTipoNumTelefono; } set { m_idTipoNumTelefono = value; } }
        public string TipoNumTelefono { get { return m_TipoNumTelefono; } set { m_TipoNumTelefono = value; } }

        public NumeroTelefonico() { }

        public NumeroTelefonico(int PidNumTelefonico, string PNumTelefonico, int PidNumeroTelefono, string PTipoNumTelefono)
        {
            m_idNumTelefonico = PidNumTelefonico;
            m_NumTelefonico = PNumTelefonico;
            m_idTipoNumTelefono = PidNumeroTelefono;
            m_TipoNumTelefono = PTipoNumTelefono;
        }
    }
}
