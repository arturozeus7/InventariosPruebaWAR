using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Paquete
    {
        protected int m_idPaquete;
        protected double m_Peso;
        protected string m_Embarque;
        protected int m_idAlmacen;
        protected string m_Almacen;
        protected int m_idRecepcion;
        protected int m_idBascula;
        protected string m_Modelo;
        protected DateTime m_FechaPesado;
        protected int m_idOperador;
        protected string m_NombreOperador;
        protected int m_idMaterial;
        protected string m_NombreMaterial;
        protected string m_Color;
        protected string m_Descripcion;
        protected int m_idProceso;
        protected string m_NombreProceso;
        protected int m_idTurno;
        protected string m_Turno;
        protected int m_idMaquina;
        protected string m_Maquina;
        protected string m_CodigoPaquete;
        protected int m_idEstatus;
        protected string m_Estatus;
        protected int m_idPaqueteSuperior;
        protected int m_Finalizado;
        protected string m_Cliente;
        protected int m_idProcesoProduccion;
        protected string m_NombreProcesoProduccion;
        protected List<Paquete> m_SubPaquetes;
        protected double m_PesoSuperiorRestante;
        protected double m_Precio;

        
        public int idPaquete { get { return m_idPaquete; } set { m_idPaquete = value; } }
        public double Peso { get { return m_Peso; } set { m_Peso = value; } }
        public string Embarque { get { return m_Embarque; } set { m_Embarque = value; } }
        public int idAlmacen { get { return m_idAlmacen; } set { m_idAlmacen = value; } }
        public string Almacen { get { return m_Almacen; } set { m_Almacen = value; } }
        public int idRecepcion { get { return m_idRecepcion; } set { m_idRecepcion = value; } }
        public int idBascula { get { return m_idBascula; } set { m_idBascula = value; } }
        public string Modelo { get { return m_Modelo; } set { m_Modelo = value; } }
        public string FechaPesado { get { return m_FechaPesado.ToString("yyyy-MM-dd HH:mm:ss"); } set { m_FechaPesado = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); } }
        public int idOperador { get { return m_idOperador; } set { m_idOperador = value; } }
        public string NombreOperador { get { return m_NombreOperador; } set { m_NombreOperador = value; } }
        public int idMaterial { get { return m_idMaterial; } set { m_idMaterial = value; } }
        public string NombreMaterial { get { return m_NombreMaterial; } set { m_NombreMaterial = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }
        public int idProceso { get { return m_idProceso; } set { m_idProceso = value; } }
        public string NombreProceso { get { return m_NombreProceso; } set { m_NombreProceso = value; } }
        public int idTurno { get { return m_idTurno; } set { m_idTurno = value; } }
        public string Turno { get { return m_Turno; } set { m_Turno = value; } }
        public int idMaquina { get { return m_idMaquina; } set { m_idMaquina = value; } }
        public string Maquina { get { return m_Maquina; } set { m_Maquina = value; } }
        public string CodigoPaquete { get { return m_CodigoPaquete; } set { m_CodigoPaquete = value; } }
        public int idEstatus { get { return m_idEstatus; } set { m_idEstatus = value; } }
        public string Estatus { get { return m_Estatus; } set { m_Estatus = value; } } 
        public int idPaqueteSuperior { get { return m_idPaqueteSuperior; } set { m_idPaqueteSuperior = value; } }
        public int Finalizado { get { return m_Finalizado; } set { m_Finalizado = value; } }
        public string Cliente { get { return m_Cliente; } set { m_Cliente = value; } }
        public int idProcesoProduccion { get { return m_idProceso; } set { m_idProceso = value; } }
        public string NombreProcesoProduccion { get { return m_NombreProceso; } set { m_NombreProceso = value; } }
        public List<Paquete> SubPaquetes { get { return m_SubPaquetes; } set { m_SubPaquetes = value; } }
        public double PesoSuperiorRestante { get { return m_PesoSuperiorRestante; } set { m_PesoSuperiorRestante = value; } }
        public double Precio { get { return m_Precio; } set { m_Precio = value; } }

        public Paquete() { }

        public Paquete(int PidPaquete, double PPeso, string PEmbarque, int PidAlmacen, string PAlmacen, int PidRecepcion, int PidBascula, string PModelo, DateTime PFechaPesado, int PidOperador, string PNombreOperador, int PidMaterial, string PNombreMaterial,
            string PDescripcion, int PidProceso, string PNombreProceso, int PidTurno, string PTurno, int PidMaquina, string PMaquina, string PCodigoPaquete, int PidEstatus, string PEstatus, int PidPaqueteSuperior, string PColor, double PPesoRestante, double PPrecio, string PCliente=null, List<Paquete> PSubPaquetes = null)
        {
            m_idPaquete = PidPaquete;
            m_Peso = PPeso;
            m_Embarque = PEmbarque;
            m_idAlmacen = PidAlmacen;
            m_Almacen = PAlmacen;
            m_idRecepcion = PidRecepcion;
            m_idBascula = PidBascula;
            m_Modelo = PModelo;
            m_FechaPesado = PFechaPesado;
            m_idOperador = PidOperador;
            m_NombreOperador = PNombreOperador;
            m_idMaterial = PidMaterial;
            m_NombreMaterial = PNombreMaterial;
            m_Descripcion = PDescripcion;
            m_idProceso = PidProceso;
            m_NombreProceso = PNombreProceso;
            m_idTurno = PidTurno;
            m_Turno = PTurno;
            m_idMaquina = PidMaquina;
            m_Maquina = PMaquina;
            m_CodigoPaquete = PCodigoPaquete;
            m_idEstatus = PidEstatus;
            m_Estatus = PEstatus;
            m_idPaqueteSuperior = PidPaqueteSuperior;
            m_Color = PColor;
            m_SubPaquetes = PSubPaquetes;
            m_Cliente = PCliente;
            m_PesoSuperiorRestante = PPesoRestante;
            m_Precio = PPrecio;
        }

        public Paquete(int PidProcesoProduccion, DateTime PFechaPesado, double PPeso, int PidMaquina, string PMaquina, int PidTurno, string PTurno)
        {
            m_idProcesoProduccion = PidProcesoProduccion;
            m_FechaPesado = PFechaPesado;
            m_Peso = PPeso;
            m_idMaquina = PidMaquina;
            m_Maquina = PMaquina;
            m_idTurno = PidTurno;
            m_Turno = PTurno;
        }
    }
}