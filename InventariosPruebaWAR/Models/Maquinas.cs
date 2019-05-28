using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Maquinas
    {
        protected int m_idMaquina;
        protected string m_Maquina;
        protected DateTime m_UltimoMantenimiento;
        protected string m_Modelo;
        protected string m_Marca;
        protected DateTime m_FechaAdquisicion;
        protected string m_Descripcion;
        protected int m_idTipoMaquina;
        protected string m_TipoMaquina;
        protected int m_idEstadoMaquina;
        protected string m_EstadoMaquina;
        protected int m_idProceso;
        protected string m_Proceso;

        public int idMaquina { get { return m_idMaquina; } set { m_idMaquina = value; } }
        public string Maquina { get { return m_Maquina; } set { m_Maquina = value; } }
        public string UltimoMantenimiento { get { return m_UltimoMantenimiento.ToString("yyyy-MM-dd"); } set { m_UltimoMantenimiento = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        public string Modelo { get { return m_Modelo; } set { m_Modelo = value; } }
        public string Marca { get { return m_Marca; } set { m_Marca = value; } }
        public string FechaAdquisicion { get { return m_FechaAdquisicion.ToString("yyyy-MM-dd"); } set { m_FechaAdquisicion = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }
        public int idTipoMaquina { get { return m_idTipoMaquina; } set { m_idTipoMaquina = value; } }
        public string TipoMaquina { get { return m_TipoMaquina; } set { m_TipoMaquina = value; } }
        public int idEstadoMaquina { get { return m_idEstadoMaquina; }set { m_idEstadoMaquina = value; } }
        public string EstadoMaquina { get { return m_EstadoMaquina; } set { m_EstadoMaquina = value; } }
        public int idProceso { get { return m_idProceso; } set { m_idProceso = value; } }
        public string Proceso { get { return m_Proceso; } set { m_Proceso = value; } }

        public Maquinas() { }

        public Maquinas(int PidMaquina, string PMaquina, DateTime PUltimoMantenimineto, string PModelo, string PMarca, DateTime PFechaAdquisicion, string PDescripcion, int PidTipoMaquina,
            string PTipoMaquina, int PidEstadoMaquina, string PEstadoMaquina, int PidProceso = 0, string PProceso = null)
        {
            m_idMaquina = PidMaquina;
            m_Maquina = PMaquina;
            m_UltimoMantenimiento = PUltimoMantenimineto;
            m_Modelo = PModelo;
            m_Marca = PMarca;
            m_FechaAdquisicion = PFechaAdquisicion;
            m_Descripcion = PDescripcion;
            m_idTipoMaquina = PidTipoMaquina;
            m_TipoMaquina = PTipoMaquina;
            m_idEstadoMaquina = PidEstadoMaquina;
            m_EstadoMaquina = PEstadoMaquina;
            m_idProceso = PidProceso;
            m_Proceso = PProceso;
        }
    }
}