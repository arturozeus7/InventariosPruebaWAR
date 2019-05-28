using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Recepcion
    {
        protected int m_idRecepcion;
        protected string m_Embarque;
        protected int m_idAlmacen;
        protected string m_NombreAlmacen;
        protected int m_idMaterial;
        protected string m_NombreMaterial;
        protected int m_idCliente;
        protected string m_Cliente;
        protected int m_idOperador;
        protected string m_NombreOperador;
        protected int m_idTurno;
        protected string m_Turno;
        protected double m_PesoPresentado;
        protected double m_PesoInterno;
        protected string m_Folio;
        protected DateTime m_FechaRecepcion;
        protected string m_Transportista;
        protected int m_idOperacionRecepcion;
        protected string m_NombreOperacionRecepcion;
        protected int m_idPersonaRecepcion;
        protected string m_NombrePersonaRecepcion;
        protected string m_Descripcion;
        protected string m_Placas;
        protected string m_Chofer;
        protected int m_idPersonaEntrega;
        protected string m_NombrePersonaEntrega;
        protected int m_idBascula;

        protected int m_idPedido;

        public int idRecepcion { get { return m_idRecepcion; } set { m_idRecepcion = value; } }
        public string Embarque { get { return m_Embarque; } set { m_Embarque = value; } }
        public int idAlmacen { get { return m_idAlmacen; } set { m_idAlmacen = value; } }
        public string NombreAlmacen { get { return m_NombreAlmacen; } set { m_NombreAlmacen = value; } }
        public int idMaterial { get { return m_idMaterial; } set { m_idMaterial = value; } }
        public string NombreMaterial { get { return m_NombreMaterial; } set { m_NombreMaterial = value; } }
        public int idCliente { get { return m_idCliente; } set { m_idCliente = value; } }
        public string Cliente { get { return m_Cliente; } set { m_Cliente = value; } }
        public int idOperador { get { return m_idOperador; } set { m_idOperador = value; } }
        public string NombreOperador { get { return m_NombreOperador; } set { m_NombreOperador = value; } }
        public int idTurno { get { return m_idTurno; } set { m_idTurno = value; } }
        public string Turno { get { return m_Turno; } set { m_Turno = value; } }
        public double PesoPresentado { get { return m_PesoPresentado; } set { m_PesoPresentado = value; } }
        public double PesoInterno { get { return m_PesoInterno; } set { m_PesoInterno = value; } }
        public string Folio { get { return m_Folio; } set { m_Folio = value; } }
        public string FechaRecepcion { get { return m_FechaRecepcion.ToString("yyyy-MM-dd HH:mm:ss"); } set { m_FechaRecepcion = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); } }
        public string Transportista { get { return m_Transportista; } set { m_Transportista = value; } }
        public int idOperacionRecepcion { get { return m_idOperacionRecepcion; } set { m_idOperacionRecepcion = value; } }
        public string NombreOperacionRecepcion { get { return m_NombreOperacionRecepcion; } set { m_NombreOperacionRecepcion = value; } }
        public int idPersonaRecepcion { get { return m_idPersonaRecepcion; } set { m_idPersonaRecepcion = value; } }
        public string NombrePersonaRecepcion { get { return m_NombrePersonaRecepcion; } set { m_NombrePersonaRecepcion = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }
        public string Placas { get { return m_Placas; } set { m_Placas = value; } }
        public string Chofer { get { return m_Chofer; } set { m_Chofer = value; } }
        public int idPersonaEntrega { get { return m_idPersonaEntrega; } set { m_idPersonaEntrega = value; } }
        public string NombrePersonaEntrega { get { return m_NombrePersonaEntrega; } set { m_NombrePersonaEntrega = value; } }
        public int idBascula { get { return m_idBascula; } set { m_idBascula = value; } }

        public int idPedido { get { return m_idPedido; } set { m_idPedido = value; } }

        public Recepcion() { }

        public Recepcion(int PidRecepcion, string PEmbarque, int PidAlmacen, string PNombreAlmacen, int PidMaterial, string PNombreMaterial, int PidCliente, string PCliente, int PidOperador, string PNombreOperador, int PidTturno, string PTurno,
            double PPesoPresentado, double PPesoInterno, string PFolio, DateTime PFechaRecepcion, string PTransportista, int PidOperacionRecepcion, string PNombreOperacionRecepcion, int PPersonaRecepcion, string PNombrePersonaRecepcion,
            string PDescripcion, string PPlacas, string PChofer, int PPersonaEntrega, string PNombrePersonaEntrega)
        {
            m_idRecepcion = PidRecepcion;
            m_Embarque = PEmbarque;
            m_idAlmacen = PidAlmacen;
            m_NombreAlmacen = PNombreAlmacen;
            m_idMaterial = PidMaterial;
            m_NombreMaterial = PNombreMaterial;
            m_idCliente = PidCliente;
            m_Cliente = PCliente;
            m_idOperador = PidOperador;
            m_NombreOperador = PNombreOperador;
            m_idTurno = PidTturno;
            m_Turno = PTurno;
            m_PesoPresentado = PPesoPresentado;
            m_PesoInterno = PPesoInterno;
            m_Folio = PFolio;
            m_FechaRecepcion = PFechaRecepcion;
            m_Transportista = PTransportista;
            m_idOperacionRecepcion = PidOperacionRecepcion;
            m_NombreOperacionRecepcion = PNombreOperacionRecepcion;
            m_idPersonaRecepcion = PPersonaRecepcion;
            m_NombrePersonaRecepcion = PNombrePersonaRecepcion;
            m_Descripcion = PDescripcion;
            m_Placas = PPlacas;
            m_Chofer = PChofer;
            m_idPersonaEntrega = PPersonaEntrega;
            m_NombrePersonaEntrega = PNombrePersonaEntrega;
        }
    }
}