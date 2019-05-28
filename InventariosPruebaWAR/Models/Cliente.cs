using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Cliente
    {
        protected int m_idCliente;
        protected int m_idEmpresa;
        protected string m_Empresa;
        protected int m_DiasCredito;
        protected double m_CantidadCredito;
        protected string m_NumeroTarjeta;
        protected int m_idBanco;
        protected string m_Banco;
        protected int m_idTipodePago;
        protected string m_TipodePago;
        protected double m_Descuento;
        protected string m_NumeroConvenio;
        protected string m_NombreContacto;
        protected string m_CorreoContacto;
        protected string m_TelefonoContacto;

        public int idCliente { get { return m_idCliente; } set { m_idCliente = value; } }
        public int idEmpresa { get { return m_idEmpresa; } set { m_idEmpresa = value; } }
        public string Empresa { get { return m_Empresa; } set { m_Empresa = value; } }
        public int DiasCredito { get { return m_DiasCredito; } set { m_DiasCredito = value; } }
        public double CantidadCredito { get { return m_CantidadCredito; } set { m_CantidadCredito = value; } }
        public string NumeroTarjeta { get { return m_NumeroTarjeta; } set { m_NumeroTarjeta = value; } }
        public int idBanco { get { return m_idBanco; } set { m_idBanco = value; } }
        public string Banco { get { return m_Banco; } set { m_Banco = value; } }
        public int idTipodePago { get { return m_idTipodePago; } set { m_idTipodePago = value; } }
        public string TipodePago { get { return m_TipodePago; } set { m_TipodePago = value; } }
        public double Descuento { get { return m_Descuento; } set { m_Descuento = value; } }
        public string NumeroConvenio { get { return m_NumeroConvenio; } set { m_NumeroConvenio = value; } }
        public string NombreContacto { get { return m_NombreContacto; } set { m_NombreContacto = value; } }
        public string CorreoContacto { get { return m_CorreoContacto; } set { m_CorreoContacto = value; } }
        public string TelefonoContacto { get { return m_TelefonoContacto; } set { m_TelefonoContacto = value; } }

        public Cliente() { }

        public Cliente(int PidCliente, int PidEmpresa, string PEmpresa, int PDiasCredito, double PCantidadCredito, string PNumeroTarjeta, 
            int PidBanco, string PBanco, int PidTipodePago, string PTipodePago, double PDescuento, string PNumeroConvenio, string PNombre, string PCorreo, string PTelefono)
        {
            m_idCliente = PidCliente;
            m_idEmpresa = PidEmpresa;
            m_Empresa = PEmpresa;
            m_DiasCredito = PDiasCredito;
            m_CantidadCredito = PCantidadCredito;
            m_NumeroTarjeta = PNumeroTarjeta;
            m_idBanco = PidBanco;
            m_Banco = PBanco;
            m_idTipodePago = PidTipodePago;
            m_TipodePago = PTipodePago;
            m_Descuento = PDescuento;
            m_NumeroConvenio = PNumeroConvenio;
            m_NombreContacto = PNombre;
            m_CorreoContacto = PCorreo;
            m_TelefonoContacto = PTelefono;
        }
    }
}
