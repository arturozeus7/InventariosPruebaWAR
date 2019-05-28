using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Registro
    {
        protected int m_idRegistro;
        protected DateTime m_Fecha;
        protected double m_PesoTotal;
        protected int m_idEmpleado;
        protected string m_Empleado;
        protected int m_idMaquina;
        protected string m_Maquina;
        protected List<ListaRegistro> m_ListaRegistros;

        public int idRegistro { get { return m_idRegistro; } set { m_idRegistro = value; } }
        public string Fecha { get { return m_Fecha.ToString("yyyy-MM-dd"); } set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        public double PesoTotal { get { return m_PesoTotal; } set { m_PesoTotal = value; } }
        public int idEmpleado { get { return m_idEmpleado; } set { m_idEmpleado = value; } }
        public string Empleado { get { return m_Empleado; } set { m_Empleado = value; } }
        public int idMaquina { get { return m_idMaquina; } set { m_idMaquina = value; } }
        public string Maquina { get { return m_Maquina; } set { m_Maquina = value; } }
        public List<ListaRegistro> ListaRegistros { get { return m_ListaRegistros; } set { m_ListaRegistros = value; } }

        public Registro() { }

        public Registro(int PidRegistro, DateTime PFecha, double PPesoTotal, int PidEmpleado, string PEmpleado, int PidMaquina, string PMaquina, List<ListaRegistro> PListaRegistros = null)
        {
            m_idRegistro = PidRegistro;
            m_Fecha = PFecha;
            m_PesoTotal = PPesoTotal;
            m_idEmpleado = PidEmpleado;
            m_Empleado = PEmpleado;
            m_idMaquina = PidMaquina;
            m_Maquina = PMaquina;
            m_ListaRegistros = PListaRegistros;
        }
    }
}