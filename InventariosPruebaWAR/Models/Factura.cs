using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Factura
    {
        protected int m_idFactura;
        protected string m_Descripcion;
        protected string m_Directorio;
        protected int m_idOrdenCVP;

        public int idFactura { get { return m_idFactura; } set { m_idFactura = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }
        public string Directorio { get { return m_Directorio; } set { m_Directorio = value; } }
        public int idOrdenCVP { get { return m_idOrdenCVP; } set { m_idOrdenCVP = value; } }

        public Factura() { }

        public Factura(int PidFactura, string PDescripcion, string PDirectorio, int PidOrdenCVP)
        {
            m_idFactura = PidFactura;
            m_Descripcion = PDescripcion;
            m_Directorio = PDirectorio;
            m_idOrdenCVP = PidOrdenCVP;
        }
    }
}
