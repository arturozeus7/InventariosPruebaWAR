using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR.Models
{
    public class tipoMantenimiento
    {
        protected int m_idtipoMantenimiento;
        protected string m_Nombre;
        protected string m_Descripcion;

        public int idtipoMantenimiento { get { return m_idtipoMantenimiento; } set { m_idtipoMantenimiento = value; } }
        public string Nombre { get { return m_Nombre; } set { m_Nombre = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }


        public tipoMantenimiento() { }

        public tipoMantenimiento(int idtipoMantenimiento, string Nombre, string Descripcion)
        {
            m_idtipoMantenimiento = idtipoMantenimiento;
            m_Nombre = Nombre;
            m_Descripcion = Descripcion;
            
        }
    }

    
}