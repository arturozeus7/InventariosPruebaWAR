using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Puestos
    {
        protected int m_idPuesto;
        protected string m_Puesto;

        public int idPuesto
        {
            get { return m_idPuesto; }
            set { m_idPuesto = value; }
        }
        public string Puesto { get { return m_Puesto; } set { m_Puesto = value; } }

        public Puestos() { }

        public Puestos(int PidPuesto, string PPuesto)
        {
            m_idPuesto = PidPuesto;
            m_Puesto = PPuesto;
        }
    }
}