using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class Material
    {
        protected int m_idMaterial;
        protected string m_NombreMaterial;
        protected string m_Color;
        protected int m_idTipoMaterial;
        protected string m_TipoMaterial;

        public int idMaterial { get { return m_idMaterial; } set { m_idMaterial = value; } }
        public string NombreMaterial { get { return m_NombreMaterial; } set { m_NombreMaterial = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }
        public int idTipoMaterial { get { return m_idTipoMaterial; } set { m_idTipoMaterial = value; } }
        public string TipoMaterial { get { return m_TipoMaterial; } set { m_TipoMaterial = value; } }

        public Material() { }

        public Material(int PidMaterial, string PNombre, string PColor, int PidTipoMaterial, string PTipoMaterial)
        {
            m_idMaterial = PidMaterial;
            m_NombreMaterial = PNombre;
            m_Color = PColor;
            m_idTipoMaterial = PidTipoMaterial;
            m_TipoMaterial = PTipoMaterial;
        }
    }
}