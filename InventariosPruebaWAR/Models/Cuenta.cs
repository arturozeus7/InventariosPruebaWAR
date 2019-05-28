using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Cuenta
    {
        protected int m_idCuenta;
        protected string m_NumeroCuenta;
        protected string m_banco;
        protected int m_idBanco;
        protected double m_saldo;
        protected int m_idTipoCuenta;
        protected string m_TipoCuenta;
        protected Cuenta() { }
        public Cuenta(int idCuenta, string numeroCuenta, string banco, int idBanco, string tipoCuenta, int idTipoCuenta, double saldo)
        {
            m_idCuenta = idCuenta;
            m_idBanco = idBanco;
            m_NumeroCuenta = numeroCuenta;
            m_banco = banco;
            m_saldo = saldo;
            m_idTipoCuenta = idTipoCuenta;
            m_TipoCuenta = tipoCuenta;
        }
        public int idCuenta
        {
            get
            {
                return m_idCuenta;
            }
            set
            {
                m_idCuenta = value;
            }
  
        }
        public int idBanco
        {
            get
            {
                return m_idBanco;
            }
            set
            {
                m_idBanco = value;
            }

        }
        public string NumeroCuenta
        {
            get
            {
                return m_NumeroCuenta;
            }
            set
            {
                m_NumeroCuenta = value;
            }

        }
        public string Banco
        {
            get
            {
                return m_banco;
            }
            set
            {
                m_banco = value;
            }

        }

        public double Saldo
        {
            get
            {
                return m_saldo;
            }
            set
            {
                m_saldo = value;
            }
        }
        public int IdTipoCuenta
        {
            get
            {
                return m_idTipoCuenta;
            }
            set
            {
                m_idTipoCuenta = value;
            }
        }
        public string TipoCuenta
        {
            get
            {
                return m_TipoCuenta;
            }
            set
            {
                m_TipoCuenta = value;
            }
        }

    }
}
