using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class TipoClientes
    {
        protected int m_idTipoCliente;
        protected string m_TipoCliente;

        public int idTipoCliente { get { return m_idTipoCliente; } set { m_idTipoCliente = value; } }
        public string TipoCliente { get { return m_TipoCliente; } set { m_TipoCliente = value; } }

        public TipoClientes() { }

        public TipoClientes(int PidTipoCliente, string PTipoCliente)
        {
            m_idTipoCliente = PidTipoCliente;
            m_TipoCliente = PTipoCliente;
        }
    }
}
