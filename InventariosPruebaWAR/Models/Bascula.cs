using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Bascula
    {
        protected int m_idBascula;
        protected string m_Modelo;
        protected double m_PesoMaximo;
        protected DateTime m_FechaMantenimiento;

        public int idBascula { get { return m_idBascula; } set { m_idBascula = value; } }
        public string Modelo { get { return m_Modelo; } set { m_Modelo = value; } }
        public double PesoMaximo { get { return m_PesoMaximo; } set { m_PesoMaximo = value; } }
        public string FechaMantenimiento { get { return m_FechaMantenimiento.ToString("yyyy-MM-dd"); } set { m_FechaMantenimiento = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); } }

        public Bascula() { }

        public Bascula(int PidBascula, string PModelo, double PPesoMaximo, DateTime PFecha)
        {
            m_idBascula = PidBascula;
            m_Modelo = PModelo;
            m_PesoMaximo = PPesoMaximo;
            m_FechaMantenimiento = PFecha;
        }
    }
}