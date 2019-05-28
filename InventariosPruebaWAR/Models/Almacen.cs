using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Almacen
    {
        protected int m_idAlmacen;
        protected string m_NombreAlmacen;
        protected int m_idProceso;
        protected string m_Proceso;

        public int idAlmacen { get { return m_idAlmacen; } set { m_idAlmacen = value; } }
        public string NombreAlmacen { get { return m_NombreAlmacen; } set { m_NombreAlmacen = value; } }
        public int idProceso { get { return m_idProceso; } set { m_idProceso = value; } }
        public string Proceso { get { return m_Proceso; } set { m_Proceso = value; } }

        public Almacen() { }

        public Almacen(int Pidalmacen, string Pnombrealmacen, int PidProceso = 0, string PProceso = null)
        {
            m_idAlmacen = Pidalmacen;
            m_NombreAlmacen = Pnombrealmacen;
            m_idProceso = PidProceso;
            m_Proceso = PProceso;
        }
    }
}