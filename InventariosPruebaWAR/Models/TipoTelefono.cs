using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class TipoTelefono
    {
        protected int m_idTipoTelefono;
        protected string m_TipoTelefono;

        public int idTipoTelefono { get { return m_idTipoTelefono; } set { m_idTipoTelefono = value; } }
        public string TipoTele { get { return m_TipoTelefono; } set { m_TipoTelefono = value; } }

        public TipoTelefono() { }

        public TipoTelefono(int PidTipoTelefono, string PTipoTelefono)
        {
            m_idTipoTelefono = PidTipoTelefono;
            m_TipoTelefono = PTipoTelefono;
        }
    }
}
