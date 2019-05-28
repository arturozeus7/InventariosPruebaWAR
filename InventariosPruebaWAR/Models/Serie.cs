using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Serie
    {
        protected int m_idSerie;
        protected string m_Prefijo;
        protected double m_Numero;

        public int idSerie { get { return m_idSerie; } set { m_idSerie = value; } }
        public string Prefijo { get { return m_Prefijo; } set { m_Prefijo = value; } }
        public double Numero { get { return m_Numero; } set { m_Numero = value; } }

        public Serie() { }

        public Serie(int PidSerie, string PPrefijo, double PNumero)
        {
            m_idSerie = PidSerie;
            m_Prefijo = PPrefijo;
            m_Numero = PNumero;
        }
    }
}
