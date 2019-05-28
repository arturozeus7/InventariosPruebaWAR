using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class TipoMaquinas
    {
        protected int m_idTipoMaquina;
        protected string m_TipoMaquina;

        public int idTipoMaquina { get { return m_idTipoMaquina; } set { m_idTipoMaquina = value; } }
        public string TipoMaquina { get { return m_TipoMaquina; } set { m_TipoMaquina = value; } }

        public TipoMaquinas() { }

        public TipoMaquinas(int PidTipoMaquina, string PTipoMaquina)
        {
            m_idTipoMaquina = PidTipoMaquina;
            m_TipoMaquina = PTipoMaquina;
        }
    }
}