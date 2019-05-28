using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class TiposdePago
    {
        protected int m_idTipodePago;
        protected string m_TipodePago;

        public int idTipodePago { get { return m_idTipodePago; } set { m_idTipodePago = value; } }
        public string TipodePago { get { return m_TipodePago; } set { m_TipodePago = value; } }

        public TiposdePago() { }

        public TiposdePago(int PidTipodePago, string PTipodePago)
        {
            m_idTipodePago = PidTipodePago;
            m_TipodePago = PTipodePago;
        }
    }
}
