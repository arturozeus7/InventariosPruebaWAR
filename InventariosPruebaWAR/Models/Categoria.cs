using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class Categoria
    {
        protected int m_idCategoria;
        protected string m_Categoria;

        public int idCategoria { get { return m_idCategoria; } set { m_idCategoria = value; } }
        public string NCategoria { get { return m_Categoria; } set { m_Categoria = value; } }

        public Categoria() { }

        public Categoria(int PidCategoria, string PCategoria)
        {
            m_idCategoria = PidCategoria;
            m_Categoria = PCategoria;
        }
    }
}
