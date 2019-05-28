using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Sucursal
    {
        protected int m_idSucursal;
        protected string m_Sucursal;
        protected string m_Calle;
        protected string m_Numero;
        protected string m_Colonia;
        protected string m_CP;
        protected string m_Municipio;
        protected string m_Estado;
        protected int m_idEstado;
        protected string m_Telefono;
        protected bool m_Matriz;
        protected string m_Logotipo;

        public int idSucursal { get { return m_idSucursal; } set { m_idSucursal = value; } }
        public string NSucursal { get { return m_Sucursal; } set { m_Sucursal = value; } }
        public string Calle { get { return m_Calle; } set { m_Calle = value; } }
        public string Numero { get { return m_Numero; } set { m_Numero = value; } }
        public string Colonia { get { return m_Colonia; } set { m_Colonia = value; } }
        public string CP { get { return m_CP; } set { m_CP = value; } }
        public string Municipio { get { return m_Municipio; } set { m_Municipio = value; } }
        public string Estado { get { return m_Estado; } set { m_Estado = value; } }
        public int idEstado { get { return m_idEstado; } set { m_idEstado = value; } }
        public string Telefono { get { return m_Telefono; } set { m_Telefono = value; } }
        public bool Matriz {get { return m_Matriz; } set { m_Matriz = value; } }
        public string Logotipo { get { return m_Logotipo; } set { m_Logotipo = value; } }

        public Sucursal() { }

        public Sucursal(int PidSucursal, string PSucursal, string PCalle, string PNumero, string PCP, string PColonia, string PMunicipio, string PEstado, int PidEstado, 
            string PTelefono, bool PMatriz, string PLogotipo)
        {
            m_idSucursal = PidSucursal;
            m_Sucursal = PSucursal;
            m_Calle = PCalle;
            m_Numero = PNumero;
            m_Colonia = PColonia;
            m_CP = PCP;
            m_Municipio = PMunicipio;
            m_Estado = PEstado;
            m_idEstado = PidEstado;
            m_Telefono = PTelefono;
            m_Matriz = PMatriz;
            m_Logotipo = PLogotipo;
        }
    }
}
