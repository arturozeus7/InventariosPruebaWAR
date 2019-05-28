using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class IVAs
    {
        protected int m_idIVA;
        protected double m_IVA;
        
        public int idIVA { get { return m_idIVA; } set { m_idIVA = value; } }
        public double IVA { get { return m_IVA; } set { m_IVA = value; } }

        public IVAs() { }

        public IVAs(int PidIVA, double PIVA)
        {
            m_idIVA = PidIVA;
            m_IVA = PIVA;
        }
    }
}
