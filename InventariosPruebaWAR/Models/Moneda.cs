using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Moneda
    {
        protected int m_idMoneda;
        protected string m_Moneda;

        public int idMoneda { get { return m_idMoneda; } set { m_idMoneda = value; } }
        public string NMoneda { get { return m_Moneda; } set { m_Moneda = value; } }

        public Moneda() { }

        public Moneda(int PidMoneda, string PMoneda)
        {
            m_idMoneda = PidMoneda;
            m_Moneda = PMoneda;
        }
    }
}
