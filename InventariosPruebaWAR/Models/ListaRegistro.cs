using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventariosPruebaWAR
{
    public class ListaRegistro
    {
        protected int m_idListaRegistro;
        protected int m_idRegistro;
        protected int m_idMaterial;
        protected string m_Material;
        protected string m_Color;
        protected string m_Codigo;
        protected double m_Peso;

        public int idListaRegistro { get { return m_idListaRegistro; } set { m_idListaRegistro = value; } }
        public int idRegistro { get { return m_idRegistro; } set { m_idRegistro = value; } }
        public int idMaterial { get { return m_idMaterial; } set { m_idMaterial = value; } }
        public string Material { get { return m_Material; } set { m_Material = value; } }
        public string Color { get { return m_Color; } set { m_Color = value; } }
        public string Codigo { get { return m_Codigo; } set { m_Codigo = value; } }
        public double Peso { get { return m_Peso; } set { m_Peso = value; } }

        public ListaRegistro() { }

        public ListaRegistro(int PidListaRegistro, int PidRegistro, int PidMaterial, string PMaterial, string PColor, string PCodigo, double PPeso)
        {
            m_idListaRegistro = PidListaRegistro;
            m_idRegistro = PidRegistro;
            m_idMaterial = PidMaterial;
            m_Material = PMaterial;
            m_Color = PColor;
            m_Codigo = PCodigo;
            m_Peso = PPeso;
        }
    }
}