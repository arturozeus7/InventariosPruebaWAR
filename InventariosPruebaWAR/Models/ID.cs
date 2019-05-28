using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class ID
    {
        protected int m_id;

        public int id { get { return m_id; } set { m_id = value; } }

        public ID() { }

        public ID(int Pid)
        {
            m_id = Pid;
        }
    }
}
