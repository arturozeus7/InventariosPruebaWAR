using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR.Models
{
    public class Pieza
    {
        protected int m_idPieza;
        protected string m_Nombre;
        protected string m_Descripcion;

        public int idPieza { get { return m_idPieza; } set { m_idPieza = value; } }
        public string Nombre { get { return m_Nombre; } set { m_Nombre = value; } }
        public string Descripcion { get { return m_Descripcion; } set { m_Descripcion = value; } }

        public Pieza() { }

        public Pieza(int idPieza, string Nombre, string Descripcion)
        {
            m_idPieza = idPieza;
            m_Nombre = Nombre;
            m_Descripcion = Descripcion;
        }
    }
}