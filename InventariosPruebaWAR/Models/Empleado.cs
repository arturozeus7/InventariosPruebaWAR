using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Empleado
    {
        protected int m_idEmpleado;
        protected string m_Nombre;
        protected string m_ApellidoPaterno;
        protected string m_ApellidoMaterno;
        protected string m_Correo;
        protected string m_Usuario;
        protected string m_Contra;
        protected string m_Puesto;
        protected string m_Telefono;
        protected string m_Sucursal;
        protected int m_idPrivilegio;
        protected int m_idSucursal;
        protected DateTime m_Fecha;
        protected string m_Privilegio;

        public int idEmpleado { get { return m_idEmpleado; } set { m_idEmpleado = value; } }
        public string Nombre { get { return m_Nombre; } set { m_Nombre = value; } }
        public string ApellidoPaterno { get { return m_ApellidoPaterno; } set { m_ApellidoPaterno = value; } }
        public string ApellidoMaterno { get { return m_ApellidoMaterno; } set { m_ApellidoMaterno = value; } }
        public string Correo { get { return m_Correo; } set { m_Correo = value; } }
        public string Usuario { get { return m_Usuario; } set { m_Usuario = value; } }
        public string Contra { get { return m_Contra; } set { m_Contra = value; } }
        public string Puesto { get { return m_Puesto; } set { m_Puesto = value; } }
        public string Telefono { get { return m_Telefono; } set { m_Telefono = value; } }
        public string Sucursal { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public int idPrivilegio { get { return m_idPrivilegio; } set { m_idPrivilegio = value; } }
        public int idSucursal { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string Fecha
        {
            get { return m_Fecha.ToString("yyyy-MM-dd"); }
            set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); }
        }
        public string Privilegio { get { return m_Privilegio; } set { m_Privilegio = value; } }


        public Empleado() { }
        public Empleado(int PidEmpleado, string PNombre, string PPaterno, string PMaterno, string PCorreo, string PUsuario,
                   string PContra, string PPuesto, string PTelefono, int PPrivilegios, int PidSucursal, string PSucursal, DateTime PFecha, string PPrivilegio)
        {
            m_idEmpleado = PidEmpleado;
            m_Nombre = PNombre;
            m_ApellidoPaterno = PPaterno;
            m_ApellidoMaterno = PMaterno;
            m_Correo = PCorreo;
            m_Usuario = PUsuario;
            m_Contra = PContra;
            m_Puesto = PPuesto;
            m_Telefono = PTelefono;
            m_idSucursal = PidSucursal;
            m_idPrivilegio = PPrivilegios;
            m_Sucursal = PSucursal;
            m_Fecha = PFecha;
            m_Privilegio = PPrivilegio;
        }
    }
}
