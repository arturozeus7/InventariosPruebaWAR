using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Empresa
    {
        //Miembros
        protected int m_idEmpresa;
        protected string m_Nombre;
        protected string m_RazonSocial;
        protected string m_RFC;
        protected string m_Calle;
        protected string m_numero;
        protected string m_Colonia;
        protected string m_CP;
        protected string m_Municipio;
        protected string m_Estado;
        protected int m_idEstado;
        protected string m_NombreContacto;
        protected string m_CorreoContacto;
        protected string m_TelContacto;
        protected string m_idTelefono;

        //Propiedades
        public int idEmpresa
        {
            get { return m_idEmpresa; }
            set { m_idEmpresa = value; }
        }

        public string Nombre
        {
            get { return m_Nombre; }
            set { m_Nombre = value; }
        }
        public string RazonSocial
        {
            get { return m_RazonSocial; }
            set { m_RazonSocial = value; }
        }
        public string RFC
        {
            get { return m_RFC; }
            set { m_RFC = value; }
        }
        public string Calle
        {
            get { return m_Calle; }
            set { m_Calle = value; }
        }

        public string Numero { get { return m_numero; } set { m_numero = value; } }

        public string Colonia
        {
            get { return m_Colonia; }
            set { m_Colonia = value; }
        }
        public string CP
        {
            get { return m_CP; }
            set { m_CP = value; }
        }
        public string Municipio
        {
            get { return m_Municipio; }
            set { m_Municipio = value; }
        }
        public string Estado
        {
            get { return m_Estado; }
            set { m_Estado = value; }
        }
        public int idEstado
        {
            get { return m_idEstado; }
            set { m_idEstado = value; }
        }
        public string NombreContacto
        {
            get { return m_NombreContacto; }
            set { m_NombreContacto = value; }
        }
        public string CorreoContacto
        {
            get { return m_CorreoContacto; }
            set { m_CorreoContacto = value; }
        }

        public string TelContacto
        {
            get { return m_TelContacto; }
            set { m_TelContacto = value; }
        }
        public string idTelefono
        {
            get { return m_idTelefono; }
            set { m_idTelefono = value; }
        }

        public Empresa(int PidEmpresa, string PNombre, string PRazonSocial, string PRFC, string PCalle, string PNumero, string PCP, string PColonia, string PMunicipio,
            int PidEstado, string PNombreContacto, string PCorreoContacto, string PidTelefono, string PEstado, string PTelContacto)
        {
            m_idEmpresa = PidEmpresa;
            m_Nombre = PNombre;
            m_RazonSocial = PRazonSocial;
            m_RFC = PRFC;
            m_Calle = PCalle;
            m_numero = PNumero;
            m_Colonia = PColonia;
            m_CP = PCP;
            m_Municipio = PMunicipio;
            m_Estado = PEstado;
            m_idEstado = PidEstado;
            m_NombreContacto = PNombreContacto;
            m_CorreoContacto = PCorreoContacto;
            m_TelContacto = PTelContacto;
            m_idTelefono = PidTelefono;
        }

        public Empresa() { }
    }
}
