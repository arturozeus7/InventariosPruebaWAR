using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Insumos
    {
        protected int m_idInsumo;
        protected string m_Insumo;
        protected string m_Cantidad;
        protected string m_Descripcion;

        public int idInsumo { get { return m_idInsumo; } set { m_idInsumo = value; } }
        public string Insumo { get { return m_Insumo; } set { m_Insumo = value; } }
        public string Cantidad { get { return m_Cantidad; } set { m_Cantidad = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }

        public Insumos() { }

        public Insumos(int PidInsumo, string Pinsumo, string PCantidad, string PDescripcion)
        {
            m_idInsumo = PidInsumo;
            m_Insumo = Pinsumo;
            m_Cantidad = PCantidad;
            m_Descripcion = PDescripcion;
        }
    }
}