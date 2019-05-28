using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class OperacionRecepcion
    {
        protected int m_idOperacionRecepcion;
        protected string m_NombreOperacion;

        public int idOperacionRecepcion { get { return m_idOperacionRecepcion; } set { m_idOperacionRecepcion = value; } }
        public string NombreOperacion { get { return m_NombreOperacion; } set { m_NombreOperacion = value; } }

        public OperacionRecepcion() { }

        public OperacionRecepcion(int Pidoperacion, string PNombreOperacion)
        {
            m_idOperacionRecepcion = Pidoperacion;
            m_NombreOperacion = PNombreOperacion;
        }
    }
}