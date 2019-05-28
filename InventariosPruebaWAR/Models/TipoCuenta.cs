using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class TipoCuenta
    {
        protected int m_idTipoCuenta;
        protected string m_TipodeCuenta;

        public int idTipoCuenta { get { return m_idTipoCuenta; } set { m_idTipoCuenta = value; } }
        public string TipodeCuenta { get { return m_TipodeCuenta; } set { m_TipodeCuenta = value; } }

        public TipoCuenta() { }

        public TipoCuenta(int PidTipoCuenta, string PTipodeCuenta)
        {
            m_idTipoCuenta = PidTipoCuenta;
            m_TipodeCuenta = PTipodeCuenta;
        }
    }
}
