using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class RecepcionMerca
    {
        protected int m_idRecepcion;
        protected int m_idOrdenCVP;
        protected string m_TipoDocumento;

        public int idRecepcion { get { return m_idRecepcion; } set { m_idRecepcion = value; } }
        public int idOrdenCVP { get { return m_idOrdenCVP; } set { m_idOrdenCVP = value; } }
        public string TipoDocumento { get { return m_TipoDocumento; } set { m_TipoDocumento = value; } }

        public RecepcionMerca() { }

        public RecepcionMerca(int PidRecepcion, int PidOrdenCVP, string PTipoDocumento)
        {
            m_idRecepcion = PidRecepcion;
            m_idOrdenCVP = PidOrdenCVP;
            m_TipoDocumento = PTipoDocumento;
        }
    }
}
