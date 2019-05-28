using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class EstadoMaquina
    {
        protected int m_idEstadoMaquina;
        protected string m_NombreEstado;

        public int idEstadoMaquina { get { return m_idEstadoMaquina; } set { m_idEstadoMaquina = value; } }
        public string NombreEstado { get { return m_NombreEstado; } set { m_NombreEstado = value; } }

        public EstadoMaquina() { }

        public EstadoMaquina(int PidEstadoMaquina, string PNombreEstado)
        {
            m_idEstadoMaquina = PidEstadoMaquina;
            m_NombreEstado = PNombreEstado;
        }
    }
}