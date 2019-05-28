using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class OrdenCuenta
    {
        protected int m_idOrdenCuenta;
        protected int m_idOrdenCVP;
        protected int m_idCuenta;
        protected DateTime m_Fecha;
        protected double m_Cantidad;

        public int idOrdenCuenta;
        public int idOrdenCVP;
        public int idCuenta;
        public string Fecha
        {
            get { return m_Fecha.ToString("yyyy-MM-dd"); }
            set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); }
        }
        public double Cantidad;

        public OrdenCuenta() { }

        public OrdenCuenta(int PidOrdenCuenta, int PidOrdenCVP, int PidCuenta, DateTime PFecha, double PCantidad)
        {
            m_idOrdenCuenta = PidOrdenCuenta;
            m_idOrdenCVP = PidOrdenCVP;
            m_idCuenta = PidCuenta;
            m_Fecha = PFecha;
            m_Cantidad = PCantidad;
        }
    }
}
