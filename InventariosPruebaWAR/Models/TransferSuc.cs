using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class TransferSuc
    {
        protected int m_idTransferencia;
        protected int m_idSucTx;
        protected int m_idSucRx;
        protected string m_SucTx;
        protected string m_SucRx;
        protected int m_idEmpleado;
        protected string m_Empleado;
        protected string m_Comentarios;
        protected DateTime m_Fecha;

        public int idTransferencia { get { return m_idTransferencia; } set { m_idTransferencia = value; } }
        public int idSucTx { get { return m_idSucTx; } set { m_idSucTx = value; } }
        public int idSucRx { get { return m_idSucRx; } set { m_idSucRx = value; } }
        public string SucTx { get { return m_SucTx; } set { m_SucTx = value; } }
        public string SucRx { get { return m_SucRx; } set { m_SucRx = value; } }
        public int idEmpleado { get { return m_idEmpleado; } set { m_idEmpleado = value; } }
        public string Empleado { get { return m_Empleado; } set { m_Empleado = value; } }
        public string Comentarios { get { return m_Comentarios; } set { m_Comentarios = value; } }
        public string Fecha
        {
            get { return m_Fecha.ToString("yyyy-MM-dd HH:mm"); }
            set { m_Fecha = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture); }
        }

        public TransferSuc() { }

        public TransferSuc(int PidTransferencia, int PidSucTx, int PidSucRx, string PSucTx, string PSucRx, int PidEmpleado, string PEmpleado, string PComentarios, DateTime PFecha)
        {
            m_idTransferencia = PidTransferencia;
            m_idSucTx = PidSucTx;
            m_idSucRx = PidSucRx;
            m_SucTx = PSucTx;
            m_SucRx = PSucRx;
            m_idEmpleado = PidEmpleado;
            m_Empleado = PEmpleado;
            m_Comentarios = PComentarios;
            m_Fecha = PFecha;
        }
    }
}
