using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Presentaciones
    {
        protected int m_idPresentacion;
        protected string m_Presentacion;
        protected int m_Cantidad;

        public int idPresentacion { get { return m_idPresentacion; } set { m_idPresentacion = value; } }
        public string Presentacion { get { return m_Presentacion; } set { m_Presentacion = value; } }
        public int Cantidad { get { return m_Cantidad; } set { m_Cantidad = value; } }

        public Presentaciones() { }

        public Presentaciones(int PidPresentaciones, string PPresentacion, int PCantidad)
        {
            m_idPresentacion = PidPresentaciones;
            m_Presentacion = PPresentacion;
            m_Cantidad = PCantidad;
        }
    }
}
