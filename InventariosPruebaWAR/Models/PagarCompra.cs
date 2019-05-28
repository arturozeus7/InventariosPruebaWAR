using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class PagarCompra
    {
        protected int m_idOrdenCVP;
        protected int m_idCuenta;

        public int idOrdenCVP { get { return m_idOrdenCVP; } set { m_idOrdenCVP = value; } }
        public int idCuenta { get { return m_idCuenta; } set { m_idCuenta = value; } }

        public PagarCompra() { }

        public PagarCompra(int PidOrdenCVP, int PidCuenta)
        {
            m_idOrdenCVP = PidOrdenCVP;
            m_idCuenta = PidCuenta;
        }
    }
}
