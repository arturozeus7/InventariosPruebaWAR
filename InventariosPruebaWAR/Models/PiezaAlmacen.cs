using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR.Models
{
    public class PiezaAlmacen
    {
        private int m_idPieza;
        private int m_idAlmacen;
        private int m_Existencia;

        public int idPieza { get { return m_idPieza; } set { m_idPieza = value; } }
        public int idAlmacen { get { return m_idAlmacen; } set { m_idAlmacen = value; } }
        public int Existencia { get { return m_Existencia; } set { m_Existencia = value; } }
        

        public PiezaAlmacen() { }

        public PiezaAlmacen(int idPieza, int idAlmacen, int Existencia)
        {
            m_idPieza = idPieza;
            m_idAlmacen = idAlmacen;
            m_Existencia = Existencia;
        }





    }
}