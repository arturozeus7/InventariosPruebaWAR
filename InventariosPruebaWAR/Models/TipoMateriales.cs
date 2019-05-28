using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class TipoMateriales
    {
        protected int m_idTipoMaterial;
        protected string m_TipoMaterial;

        public int idTipoMaterial { get { return m_idTipoMaterial; } set { m_idTipoMaterial = value; } }
        public string TipoMaterial { get { return m_TipoMaterial; } set { m_TipoMaterial = value; } }

        public TipoMateriales() { }

        public TipoMateriales(int PidTipoMaterial, string PTipoMaterial)
        {
            m_idTipoMaterial = PidTipoMaterial;
            m_TipoMaterial = PTipoMaterial;
        }
    }
}