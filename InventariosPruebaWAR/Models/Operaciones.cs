using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Operaciones
    {
        protected int m_idOperacion;
        protected string m_Operacion;

        public int idOperacion { get { return m_idOperacion; } set { m_idOperacion = value; } }
        public string Operacion { get { return m_Operacion; } set { m_Operacion = value; } }

        public Operaciones() { }

        public Operaciones(int PidOperacion, string POperacion)
        {
            m_idOperacion = PidOperacion;
            m_Operacion = POperacion;
        }
    }
}
