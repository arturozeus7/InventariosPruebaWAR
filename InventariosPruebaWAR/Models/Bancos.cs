using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Bancos
    {
        protected int m_idBanco;
        protected string m_Banco;

        public int idBanco { get { return m_idBanco; } set { m_idBanco = value; } }
        public string Banco { get { return m_Banco; } set { m_Banco = value; } }

        public Bancos() { }

        public Bancos(int PidBanco, string PBanco)
        {
            m_idBanco = PidBanco;
            m_Banco = PBanco;
        }
    }
}