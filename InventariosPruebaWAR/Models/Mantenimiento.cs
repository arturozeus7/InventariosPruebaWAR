using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR.Models
{
    public class Mantenimiento
    {
        protected int m_idMantenimiento;
        protected string m_FechaFallo;
        protected string m_FechaSolucion;
        protected string m_Fallo;
        protected string m_Solucion;
        protected int m_idEmpleado;
        protected int m_idMaquina;
        protected int m_idtipoMantenimiento;

        public int idMantenimiento { get { return m_idMantenimiento; } set { m_idMantenimiento = value; } }
        public string FechaFallo { get { return m_FechaFallo; } set { m_FechaFallo = value; } }
        public string FechaSolucion { get { return m_FechaSolucion; } set { m_FechaSolucion = value; } }


        public Mantenimiento() { }

        public Mantenimiento(int idMantenimiento, string FechaFallo, string FechaSolucion)
        {
            m_idMantenimiento = idMantenimiento;
            m_FechaFallo = FechaFallo;
            m_FechaSolucion = FechaSolucion;

        }
    }
}