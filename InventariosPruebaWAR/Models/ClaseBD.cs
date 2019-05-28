using InventariosPruebaWAR.Models;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosPruebaWAR
{
    public class ClaseBD
    {
        private bool Conexion_Abierta;
        private MySqlConnection Conection;

        private void Get_Connection()
        {
            Conexion_Abierta = false;

            Conection = new MySqlConnection();
            //connection = DB_Connect.Make_Connnection(ConfigurationManager.ConnectionStrings["SQLConnection"].ConnectionString);
            Conection.ConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

            //            if (db_manage_connnection.DB_Connect.OpenTheConnection(connection))
            if (Open_Local_Connection())
            {
                Conexion_Abierta = true;
            }
            else
            {
                //					MessageBox::Show("No database connection connection made...\n Exiting now", "Database Connection Error");
                //					 Application::Exit();
            }

        }

        private bool Open_Local_Connection()
        {
            try
            {
                Conection.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public void Cerrar_Conexion()
        {
            try
            {
                //if (Conexion_Abierta)
                //{
                Conection.Close();
                Conection.Dispose();
                MySqlConnection.ClearPool(Conection);
                Conexion_Abierta = false;
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Metodo Generico "Select"
        public MySqlDataReader ObtenerDatosBD(string Consulta)
        {
            Get_Connection();
            MySqlDataReader ReaderBD = null;
            try
            {
                MySqlCommand ComandoBD = new MySqlCommand(String.Format(Consulta), Conection);
                ReaderBD = ComandoBD.ExecuteReader();
               // ComandoBD.Dispose();
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
            }
            return ReaderBD;
        }

        //Metodo Insertar, Actualizar y Eliminar
        public int ActualizarDatos(string Consulta, MySqlConnection Conexion = null)
        {
            int ret = 0;
            Get_Connection();
            try
            {
                MySqlCommand ComandoBD = new MySqlCommand(String.Format(Consulta), Conection);
                ComandoBD.ExecuteNonQuery();
                if (Consulta.ToLower().IndexOf("insert") != -1)
                    ret = (int)ComandoBD.LastInsertedId;
                ComandoBD.Dispose();
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
            }
            Cerrar_Conexion();
            return ret;
        }

        public bool VerificarExistencia(string Tabla, string Condicion, string Valor)
        {
            bool ret = false;
            Get_Connection();
            MySqlDataReader ReaderBD = null;
            try
            {

                string Query = string.Format("SELECT EXISTS(SELECT * FROM '{0}' WHERE '{1}' = '{2}') AS Existe;", Tabla, Condicion, Valor);
                MySqlCommand ComandoBD = new MySqlCommand(String.Format(Query), Conection);
                ReaderBD = ComandoBD.ExecuteReader();
                ComandoBD.Dispose();
                if (ReaderBD != null)
                    while (ReaderBD.Read())
                        ret = ReaderBD.GetBoolean(0);
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
            }
            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ret;
        }


        #region Login
        public long Login(string usr, string pwd, out int privLevel)
        {
            long id = -1;
            byte[] hash;
            string Query = "SELECT idEmpleado, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, idSucursal, Usuario, Contra, Puesto, Telefono, Privilegios, Fecha FROM Empleado WHERE Usuario = '" + usr + "'";
            privLevel = 0;

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            
                if (ReaderBD.Read())
                {
                    string hs = (string)ReaderBD["Contra"];
                    hash = Convert.FromBase64String(hs);
                    PasswordHash ph = new PasswordHash(hash);
                    id = ph.Verify(pwd) ? Convert.ToInt64(ReaderBD["idEmpleado"]) : 0;
                    privLevel = Convert.ToInt32(ReaderBD["Privilegios"]);
                }
            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return id;
        }
        #endregion

        /**
         * Pais
         **/
        #region Pais
        public List<Paises> ObtenerPais()
        {
            List<Paises> ListaPaises = new List<Paises>();
            string Query = "SELECT idPais, Pais FROM Pais;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPaises.Add(new Paises(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPaises;
        }

        public Paises ObtenerPaisById(int idPais)
        {
            Paises Pais = new Paises();
            string Query = "SELECT idPais, Pais FROM Pais WHERE idPais = '" + idPais + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Pais = new Paises(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Pais;
        }

        public Paises ObtenerPaisByName(string NombrePais)
        {
            Paises Pais = new Paises();

            string Query = "SELECT idPais, Pais FROM Pais WHERE Pais = '" + NombrePais + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Pais = new Paises(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Pais;
        }

        public int AgregarPais(Paises Pais)
        {

            string Query = string.Format("INSERT INTO Pais (Pais) VALUES ('{0}');", Pais.Pais);
            return ActualizarDatos(Query);

        }

        public void ActualizarPais(Paises Pais)
        {
            string Query = string.Format("UPDATE Pais SET Pais = '{0}' WHERE idPais = '{1}';", Pais.Pais, Pais.idPais);
            ActualizarDatos(Query);
        }

        public void EliminarPais(int idPais)
        {
            string Query = string.Format("DELETE FROM Pais WHERE idPais = '{0}';", idPais);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Estados
         **/
        #region Estados
        public List<Estados> ObtenerEstados()
        {
            List<Estados> ListaEstados = new List<Estados>();

            string Query = "SELECT idEstados, Estado, Estados.idPais, Pais.Pais FROM Estados " +
                "INNER JOIN Pais ON Estados.idPais = Pais.idPais;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaEstados.Add(new Estados(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaEstados;
        }

        public Estados ObtenerEstadoById(int idEstado)
        {
            Estados Estado = new Estados();

            string Query = "SELECT idEstados, Estado, Estados.idPais, Pais.Pais FROM Estados " +
                "INNER JOIN Pais ON Estados.idPais = Pais.idPais WHERE idEstados = '" + idEstado + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Estado = new Estados(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Estado;
        }

        public Estados ObtenerEstadoByName(string NombreEstado)
        {
            Estados Estado = new Estados();

            string Query = "SELECT idEstados, Estado, Estados.idPais, Pais.Pais FROM Estados " +
                "INNER JOIN Pais ON Estados.idPais = Pais.idPais WHERE Estado = '" + NombreEstado + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Estado = new Estados(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Estado;
        }

        public int AgregarEstado(Estados Estado)
        {

            string Query = string.Format("INSERT INTO Estados (Estado, idPais) VALUES ('{0}', '{1}');", Estado.Estado, Estado.idPais);
            return ActualizarDatos(Query);
        }

        public void ActualizarEstado(Estados Estado)
        {

            string Query = string.Format("UPDATE Estados SET Estado = '{0}', idPais = '{1}' WHERE idEstados = '{2}';", Estado.Estado, Estado.idPais, Estado.idEstado);
            ActualizarDatos(Query);
        }

        public void EliminarEstado(int idEstado)
        {

            string Query = string.Format("DELETE FROM Estados WHERE idEstados = '{0}';", idEstado);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Moneda
         **/
        #region Moneda
        public List<Moneda> ObtenerMoneda()
        {
            List<Moneda> ListaMoneda = new List<Moneda>();

            string Query = "SELECT idMoneda, Moneda FROM Moneda;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaMoneda.Add(new Moneda(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaMoneda;
        }

        public Moneda ObtenerMonedaById(int idMoneda)
        {
            Moneda Moneda = new Moneda();

            string Query = "SELECT idMoneda, Moneda FROM Moneda WHERE idMoneda = '" + idMoneda + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Moneda = new Moneda(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Moneda;
        }

        public Moneda ObtenerMonedaByName(string NombreMoneda)
        {
            Moneda Moneda = new Moneda();

            string Query = "SELECT idMoneda, Moneda FROM Moneda WHERE Moneda = '" + NombreMoneda + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Moneda = new Moneda(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Moneda;
        }

        public int AgregarMoneda(Moneda Moneda)
        {

            string Query = string.Format("INSERT INTO Moneda (Moneda) VALUES ('{0}');", Moneda.NMoneda);
            return ActualizarDatos(Query);
        }

        public void ActualizarMoneda(Moneda Moneda)
        {

            string Query = string.Format("UPDATE Moneda SET Moneda = '{0}' WHERE idMoneda = '{1}';", Moneda.NMoneda, Moneda.idMoneda);
            ActualizarDatos(Query);
        }

        public void EliminarMoneda(int idMoneda)
        {

            string Query = string.Format("DELETE FROM Moneda WHERE idMoneda = '{0}';", idMoneda);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * TipoTelefono
         **/
        #region TipoTelefono
        public List<TipoTelefono> ObtenerTipoTelefono()
        {
            List<TipoTelefono> ListaTipoTelefono = new List<TipoTelefono>();

            string Query = "SELECT idTipoTelefono, TipoTelefono FROM TipoTelefono;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaTipoTelefono.Add(new TipoTelefono(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaTipoTelefono;
        }

        public TipoTelefono ObtenerTipoTelefonoById(int idTipoTelefono)
        {
            TipoTelefono TipoTelefono = new TipoTelefono();

            string Query = "SELECT idTipoTelefono, TipoTelefono FROM TipoTelefono WHERE idtipotelefono = '" + idTipoTelefono + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    TipoTelefono = new TipoTelefono(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return TipoTelefono;
        }

        public List<TipoTelefono> ObtenerTipoTelefonoByTipo(string TipoTelefono)
        {
            List<TipoTelefono> ListaTipoTelefono = new List<TipoTelefono>();

            string Query = "SELECT idTipoTelefono, TipoTelefono FROM TipoTelefono WHERE TipoTelefono = '" + TipoTelefono + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaTipoTelefono.Add(new TipoTelefono(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaTipoTelefono;
        }

        public int AgregarTipoTelefono(TipoTelefono TipoTelefono)
        {

            string Query = string.Format("INSERT INTO tipotelefono (TipoTelefono) VALUES( '{0}');", TipoTelefono.TipoTele);
            return ActualizarDatos(Query);
        }

        public void ActualizarTipoTelefono(TipoTelefono TipoTelefono)
        {

            string Query = string.Format("UPDATE TipoTelefono SET TipoTelefono = '{0}' WHERE idTipoTelefono = '{1}';", TipoTelefono.TipoTele, TipoTelefono.idTipoTelefono);
            ActualizarDatos(Query);
        }

        public void EliminarTipoTelefono(int idTipoTelefono)
        {

            string Query = string.Format("DELETE FROM TipoTelefono WHERE idTipoTelefono = '{0}';", idTipoTelefono);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Telefono
         **/
        #region Telefono
        public List<NumeroTelefonico> ObtenerNumTelefonico()
        {
            List<NumeroTelefonico> ListaNumTelefonico = new List<NumeroTelefonico>();

            string Query = "SELECT idTelefono, NumeroTelefonico, Telefono.idTipoTelefono, TipoTelefono.TipoTelefono FROM Telefono " +
                "INNER JOIN TipoTelefono ON Telefono.idTipoTelefono = TipoTelefono.idTipoTelefono;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaNumTelefonico.Add(new NumeroTelefonico(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaNumTelefonico;
        }

        public NumeroTelefonico ObtenerNumTelefonicoById(int idNumeroTel)
        {
            NumeroTelefonico NumeroTelefonico = new NumeroTelefonico();

            string Query = "SELECT idTelefono, NumeroTelefonico, Telefono.idTipoTelefono, TipoTelefono.TipoTelefono FROM Telefono " +
                "INNER JOIN TipoTelefono ON Telefono.idTipoTelefono = TipoTelefono.idTipoTelefono WHERE idTelefono = '" + idNumeroTel + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    NumeroTelefonico = new NumeroTelefonico(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return NumeroTelefonico;
        }

        public NumeroTelefonico ObtenerNumTelefonicoByTelefono(string NumTelefono)
        {
            NumeroTelefonico NumeroTelefonico = new NumeroTelefonico();

            string Query = "SELECT idTelefono, NumeroTelefonico, Telefono.idTipoTelefono, TipoTelefono.TipoTelefono FROM Telefono " +
                "INNER JOIN TipoTelefono ON Telefono.idTipoTelefono = TipoTelefono.idTipoTelefono WHERE NumeroTelefonico = '" + NumTelefono + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    NumeroTelefonico = new NumeroTelefonico(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return NumeroTelefonico;
        }

        public int AgregarNumTelefonico(NumeroTelefonico NumTelefonico)
        {

            string Query = string.Format("INSERT INTO Telefono (NumeroTelefonico, idtipotelefono) VALUES('{0}', '{1}');", NumTelefonico.NumTelefonico, NumTelefonico.idTipoNumTelefono);
            return ActualizarDatos(Query);
        }

        public void ActualizarNumTelefonico(NumeroTelefonico NumTelefonico)
        {

            string Query = string.Format("UPDATE Telefono SET NumeroTelefonico = '{0}', idtipotelefono = '{1}' WHERE idTelefono = '{2}';", NumTelefonico.NumTelefonico, NumTelefonico.idTipoNumTelefono, NumTelefonico.idNumTelefonico);
            ActualizarDatos(Query);
        }

        public void EliminarNumTelefonico(int idNumTelefonico)
        {

            string Query = string.Format("DELETE FROM Telefono WHERE idTelefono= '{0}';", idNumTelefonico);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Categoria
         **/
        #region Categoria
        public List<Categoria> ObtenerCategoria()
        {
            List<Categoria> ListaCategoria = new List<Categoria>();

            string Query = "SELECT idCategoria, Categoria FROM Categoria;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaCategoria.Add(new Categoria(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaCategoria;
        }

        public Categoria ObtenerCategoriaById(int idCategoria)
        {
            Categoria Categoria = new Categoria();

            string Query = "SELECT idCategoria, Categoria FROM Categoria WHERE idCategoria = '" + idCategoria + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Categoria = new Categoria(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Categoria;
        }

        public Categoria ObtenerCategoriaByName(string NombreCategoria)
        {
            Categoria Categoria = new Categoria();

            string Query = "SELECT idCategoria, Categoria FROM Categoria WHERE Categoria = '" + NombreCategoria + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Categoria = new Categoria(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Categoria;
        }

        public int AgregarCategoria(Categoria Categoria)
        {

            string Query = string.Format("INSERT INTO Categoria (Categoria) VALUES ('{0}');", Categoria.NCategoria);
            return ActualizarDatos(Query);
        }

        public void ActualizarCategoria(Categoria Categoria)
        {

            string Query = string.Format("UPDATE Categoria SET Categoria = '{0}' WHERE idCategoria = '{1}';", Categoria.NCategoria, Categoria.idCategoria);
            ActualizarDatos(Query);
        }

        public void EliminarCategoria(int idCategoria)
        {

            string Query = string.Format("DELETE FROM Categoria WHERE idCategoria = '{0}';", idCategoria);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Banco
         **/
        #region Banco
        public List<Bancos> ObtenerBancos()
        {
            List<Bancos> ListaBancos = new List<Bancos>();

            string Query = "SELECT idBanco, Banco FROM Banco;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaBancos.Add(new Bancos(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaBancos;
        }

        public Bancos ObtenerBancoById(int idBancos)
        {
            Bancos Banco = new Bancos();

            string Query = "SELECT idBanco, Banco FROM Banco WHERE idBanco = '" + idBancos + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Banco = new Bancos(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Banco;
        }

        public Bancos ObtenerBancoByName(string NombreBanco)
        {
            Bancos Banco = new Bancos();

            string Query = "SELECT idBanco, Banco FROM Banco WHERE Banco = '" + NombreBanco + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Banco = new Bancos(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Banco;
        }

        public int AgregarBanco(Bancos Banco)
        {

            string Query = string.Format("INSERT INTO Banco (Banco) VALUES ('{0}');", Banco.Banco);
            return ActualizarDatos(Query);
        }

        public void ActualizarBanco(Bancos Banco)
        {

            string Query = string.Format("UPDATE Banco SET Banco = '{0}' WHERE idBanco = '{1}';", Banco.Banco, Banco.idBanco);
            ActualizarDatos(Query);
        }

        public void EliminarBanco(int idBanco)
        {

            string Query = string.Format("DELETE FROM Banco WHERE idBanco = '{0}';", idBanco);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Presentacion
         **/
        #region Presentacion
        public List<Presentaciones> ObtenerPresentaciones()
        {
            List<Presentaciones> ListaPresentaciones = new List<Presentaciones>();

            string Query = "SELECT idPresentaciones, Presentacion, Cantidad FROM Presentaciones;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPresentaciones.Add(new Presentaciones(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPresentaciones;
        }

        public Presentaciones ObtenerPresentacionById(int idPresentacion)
        {
            Presentaciones Presentacion = new Presentaciones();

            string Query = "SELECT idPresentaciones, Presentacion, Cantidad FROM Presentaciones WHERE idPresentaciones = '" + idPresentacion + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Presentacion = new Presentaciones(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Presentacion;
        }

        public Presentaciones ObtenerPresentacionByName(string NombrePresentacion)
        {
            Presentaciones Presentacion = new Presentaciones();

            string Query = "SELECT idPresentaciones, Presentacion, Cantidad FROM Presentaciones WHERE Presentacion = '" + NombrePresentacion + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Presentacion = new Presentaciones(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Presentacion;
        }

        public int AgregarPresentacion(Presentaciones Presentacion)
        {

            string Query = string.Format("INSERT INTO Presentaciones (Presentacion, Cantidad) VALUES ('{0}', '{1}');", Presentacion.Presentacion, Presentacion.Cantidad);
            return ActualizarDatos(Query);
        }

        public void ActualizarPresentacion(Presentaciones Presentacion)
        {

            string Query = string.Format("UPDATE Presentaciones SET Presentacion = '{0}', Cantidad = '{1}' WHERE idPresentaciones = '{2}';", Presentacion.Presentacion, Presentacion.Cantidad, Presentacion.idPresentacion);
            ActualizarDatos(Query);
        }

        public void EliminarPresentacion(int idPresentacion)
        {

            string Query = string.Format("DELETE FROM Presentaciones WHERE idPresentaciones = '{0}';", idPresentacion);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Inventario
         **/
        #region Inventario
        public List<Inventario> ObtenerInventario()
        {
            List<Inventario> ListaInventario = new List<Inventario>();

            string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Sucursal.Sucursal, Existencia, ExistenciaMinima, Producto.Descripcion, Producto.Codigo, Producto.idPresentacion, Presentaciones.Presentacion, Producto.Color, Producto.Piezas, Producto.PiezasTotales, Producto.CodigoBarras " +//ASR 17-05-2018
                "FROM Inventario Inv " +
                "INNER JOIN Producto ON Inv.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaInventario.Add(new Inventario(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetInt32(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14)));//ASR 10-04-2018//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaInventario;
        }

        public List<Inventario> ObtenerBusquedaInventario(string consulta)
        {
            List<Inventario> ListaInventario = new List<Inventario>();

            /*
                        string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Sucursal.Sucursal, Existencia, ExistenciaMinima, Producto.Descripcion, Producto.Codigo, Producto.idPresentacion, Presentaciones.Presentacion, Producto.Color, Producto.Piezas, Producto.PiezasTotales " +//ASR 10-04-2018//ASR 17-05-2018
                            "FROM Inventario Inv " +
                            "INNER JOIN Producto ON Inv.idProducto = Producto.idProducto " +
                            "INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal " +
                            "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                            "WHERE Producto.Producto LIKE '%" + consulta + "%' " +
                            "OR Sucursal.Sucursal LIKE '%" + consulta + "%' " +
                            "OR Producto.Descripcion LIKE '%" + consulta + "%' " +
                            "OR Producto.Codigo LIKE '%" + consulta + "%'; ";
                            */
            string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Sucursal.Sucursal, Existencia, ExistenciaMinima, Producto.Descripcion, " +
                            "Producto.Codigo, Producto.idPresentacion, Presentaciones.Presentacion, Producto.Color, Producto.Piezas, Producto.PiezasTotales, Producto.CodigoBarras " +
                            "FROM Inventario Inv INNER JOIN Producto ON Inv.idProducto = Producto.idProducto INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal " +
                            "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones INNER JOIN costos ON Inv.idProducto = costos.idProducto " +
                            " INNER JOIN proveedor ON costos.idproveedor = proveedor.idProveedor INNER JOIN empresa on proveedor.idEmpresa = empresa.idEmpresa " +
                             "WHERE Producto.Producto LIKE '%" + consulta + "%' " +
                            "OR Sucursal.Sucursal LIKE '%" + consulta + "%' " +
                            "OR Producto.Descripcion LIKE '%" + consulta + "%' " +
                            "OR Producto.Codigo LIKE '%" + consulta + "%' " +
                            "OR empresa.Nombre LIKE '%" + consulta + "%'";
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaInventario.Add(new Inventario(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetInt32(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14)));//ASR 10-04-2018//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaInventario;
        }

        public List<Inventario> ObtenerInventarioBySucursal(int idSucursal)
        {
            List<Inventario> ListaInventario = new List<Inventario>();

            string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Sucursal.Sucursal, Existencia, ExistenciaMinima, Producto.Descripcion, Producto.Codigo, Producto.idPresentacion, Presentaciones.Presentacion, Producto.Color, Producto.Piezas, Producto.PiezasTotales, Producto.CodigoBarras " +//ASR 10-04-2018//ASR 17-05-2018
                "FROM Inventario Inv " +
                "INNER JOIN Producto ON Inv.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Inv.idSucursal = '" + idSucursal + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaInventario.Add(new Inventario(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetInt32(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14)));//ASR 10-04-2018//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaInventario;
        }

        public List<Inventario> BusquedaInventarioSuc(int idSucursal, string Consulta)
        {
            List<Inventario> ListaInventario = new List<Inventario>();

            string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Sucursal.Sucursal, Existencia, ExistenciaMinima, Producto.Descripcion, Producto.Codigo, Producto.idPresentacion, Presentaciones.Presentacion, Producto.Color, Producto.Piezas, Producto.PiezasTotales, Producto.CodigoBarras " +//ASR 10-04-2018//ASR 17-05-2018
                "FROM Inventario Inv " +
                "INNER JOIN Producto ON Inv.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Producto.Producto LIKE '%" + Consulta + "%' AND Inv.idSucursal = '" + idSucursal + "' " +
                "OR Producto.Descripcion LIKE '%" + Consulta + "%' AND Inv.idSucursal = '" + idSucursal + "' " +
                "OR Producto.CodigoBarras LIKE '%" + Consulta + "%' AND Inv.idSucursal = '" + idSucursal + "' " +
                "OR Producto.Codigo LIKE '%" + Consulta + "%' AND Inv.idSucursal = '" + idSucursal + "'; ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaInventario.Add(new Inventario(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetInt32(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14)));//ASR 10-04-2018//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaInventario;
        }

        //BRS Abrir en piezas
        public bool AbrirCaja(int idInvetario)
        {
            bool ret = false;

            string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Existencia, Producto.idPresentacion, Producto.Piezas, Producto.Color FROM Inventario Inv INNER JOIN Producto ON Inv.idProducto = Producto.idProducto INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones WHERE idInventario = " + idInvetario;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
            {
                if (ReaderBD.Read())
                {
                    int idproductopiezas = 0;
                    int idinventariopiezas = 0;
                    int idinventario = Convert.ToInt32(ReaderBD[0]);
                    int idproducto = Convert.ToInt32(ReaderBD[1]);
                    int existenciaPiezas = 0;
                    int existencia = Convert.ToInt32(ReaderBD[4]);
                    int presentacion = Convert.ToInt32(ReaderBD[5]);
                    string color = ReaderBD[7].ToString();
                    string producto = ReaderBD[2].ToString();
                    int sucursal = Convert.ToInt32(ReaderBD[3]);
                    int piezas = Convert.ToInt32(ReaderBD[6]);
                    int multiplicacion = 0;
                    if (presentacion != 1)
                    {
                        if (existencia != 0)
                        {
                            existencia--;
                            if (presentacion == 3)
                            {
                                Query = "SELECT Piezas from producto where Producto = '" + producto + "' and Color = '" + color + "' and idpresentacion = 2 and Activo = 1";
                                ReaderBD = ObtenerDatosBD(Query);
                                if (ReaderBD.Read())
                                {
                                    multiplicacion = Convert.ToInt32(ReaderBD[0]);
                                }
                                ReaderBD.Dispose();

                            }
                            else
                                multiplicacion = 1;

                            if (multiplicacion != 0)
                            {


                                Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Existencia, Producto.idPresentacion FROM Inventario Inv INNER JOIN Producto ON Inv.idProducto = Producto.idProducto INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones WHERE   Producto.Color = '" + color + "' and Producto = '" + producto + "' and idPresentacion = 1 and Inv.idSucursal = " + sucursal;
                                ReaderBD = ObtenerDatosBD(Query);
                                if (ReaderBD != null)
                                {
                                    if (ReaderBD.Read())
                                    {

                                        existenciaPiezas = Convert.ToInt32(ReaderBD[4]) + piezas * multiplicacion;
                                        idinventariopiezas = Convert.ToInt32(ReaderBD[0]);
                                        Query = "UPDATE inventario SET Existencia = " + existenciaPiezas + " WHERE idInventario = " + idinventariopiezas;
                                        ActualizarDatos(Query);
                                        ret = true;
                                    }
                                    else
                                    {

                                        existenciaPiezas = piezas * multiplicacion;
                                        Query = "SELECT idProducto,Piezas from producto where Producto = '" + producto + "' and idpresentacion = 1  and Color = '" + color + "' and Activo = 1";
                                        ReaderBD = ObtenerDatosBD(Query);
                                        if (ReaderBD.Read()) //hay entrada producto por pieza
                                        {
                                            idproductopiezas = Convert.ToInt32(ReaderBD[0]);
                                            Query = "INSERT INTO inventario (idProducto, idSucursal, Existencia, ExistenciaMinima) VALUES " +
                                                 "(" + idproductopiezas + "," + sucursal + "," + existenciaPiezas + ",20)";
                                            ActualizarDatos(Query);
                                            ret = true;
                                        }

                                    }
                                    if (ret)
                                    {
                                        Query = "UPDATE inventario SET Existencia = " + existencia + " WHERE idInventario = " + idinventario;
                                        ActualizarDatos(Query);
                                    }
                                }
                            }
                        }
                    }
                }


            }
            ReaderBD.Dispose();
            ReaderBD.Close();

            Cerrar_Conexion();
            return ret;
        }

        //BRS Abrir master box en cajas internas
        public bool AbrirCajaMaster(int idInvetario)
        {
            bool ret = false;

            string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Existencia, Producto.idPresentacion, Producto.Piezas, Producto.Color FROM Inventario Inv INNER JOIN Producto ON Inv.idProducto = Producto.idProducto INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones WHERE idInventario = " + idInvetario;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
            {
                if (ReaderBD.Read())
                {
                    int idproductopiezas = 0;
                    int idinventariopiezas = 0;
                    int idinventario = Convert.ToInt32(ReaderBD[0]);
                    int idproducto = Convert.ToInt32(ReaderBD[1]);
                    int existenciaPiezas = 0;
                    int existencia = Convert.ToInt32(ReaderBD[4]);
                    int presentacion = Convert.ToInt32(ReaderBD[5]);
                    int presentacionSub = presentacion - 1;
                    string producto = ReaderBD[2].ToString();
                    int sucursal = Convert.ToInt32(ReaderBD[3]);
                    int piezas = Convert.ToInt32(ReaderBD[6]);
                    string color = ReaderBD[7].ToString();
                    if (presentacion == 3)
                    {
                        if (existencia != 0)
                        {
                            existencia--;


                            Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Existencia, Producto.idPresentacion FROM Inventario Inv INNER JOIN Producto ON Inv.idProducto = Producto.idProducto INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones WHERE Producto = '" + producto + "'  and Color = '" + color + "' and idPresentacion = 2 and Inv.idSucursal = " + sucursal;
                            ReaderBD = ObtenerDatosBD(Query);
                            if (ReaderBD != null)
                            {
                                if (ReaderBD.Read())
                                {

                                    existenciaPiezas = Convert.ToInt32(ReaderBD[4]) + piezas;
                                    idinventariopiezas = Convert.ToInt32(ReaderBD[0]);
                                    Query = "UPDATE inventario SET Existencia = " + existenciaPiezas + " WHERE idInventario = " + idinventariopiezas;
                                    ActualizarDatos(Query);
                                    ret = true;
                                }
                                else
                                {
                                    existenciaPiezas = piezas;
                                    Query = "SELECT idProducto from producto where Producto = '" + producto + "' and Color = '" + color + "'  and idpresentacion = 2 and Activo = 1";
                                    ReaderBD = ObtenerDatosBD(Query);
                                    if (ReaderBD.Read()) //hay entrada producto por pieza
                                    {

                                        idproductopiezas = Convert.ToInt32(ReaderBD[0]);
                                        Query = "INSERT INTO inventario (idProducto, idSucursal, Existencia, ExistenciaMinima) VALUES " +
                                             "(" + idproductopiezas + "," + sucursal + "," + existenciaPiezas + ",20)";
                                        ActualizarDatos(Query);
                                        ret = true;
                                    }
                                }
                                if (ret)
                                {
                                    Query = "UPDATE inventario SET Existencia = " + existencia + " WHERE idInventario = " + idinventario;
                                    ActualizarDatos(Query);
                                }
                            }
                        }
                    }
                }


            }

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ret;
        }

        public Inventario ObtenerInventarioById(int idInventario)
        {
            Inventario Inventario = new Inventario();

            string Query = "SELECT idInventario, Inv.idProducto, Producto.Producto, Inv.idSucursal, Sucursal.Sucursal, Existencia, ExistenciaMinima, Producto.Descripcion, Producto.Codigo, Producto.idPresentacion, Presentaciones.Presentacion, Producto.Color, Producto.Piezas, Producto.PiezasTotales, Producto.CodigoBarras " +//ASR 10-04-2018//ASR 17-05-2018
                "FROM Inventario Inv " +
                "INNER JOIN Producto ON Inv.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Inv.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE idInventario = '" + idInventario + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Inventario = new Inventario(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetInt32(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14));//ASR 10-04-2018//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Inventario;
        }

        public int AgregarInventario(Inventario Inventario)
        {

            string Query = string.Format("INSERT INTO Inventario (idProducto, idSucursal, Existencia, ExistenciaMinima) VALUES ('{0}', '{1}', '{2}', '{3}');", Inventario.idProducto, Inventario.idSucursal, Inventario.Existencia, Inventario.ExistenciaMinima);
            return ActualizarDatos(Query);
        }

        public void ActualizarInventario(Inventario Inventario)
        {

            string Query = string.Format("UPDATE Inventario SET idProducto = '{0}', Existencia = '{1}', ExistenciaMinima = '{2}' WHERE idInventario = '{3}';", Inventario.idProducto, Inventario.Existencia, Inventario.ExistenciaMinima, Inventario.idInventario);
            ActualizarDatos(Query);
        }

        public void EliminarInventario(int idInventario)
        {

            string Query = string.Format("DELETE FROM Inventario WHERE idInventario = '{0}';", idInventario);
            ActualizarDatos(Query);
        }
        
        public int IngestarInventario(int idProducto, int idSucursal, int ExistenciaMinima, int Existencia, bool Nuevo)
        {
            string Query = "SELECT idInventario, Existencia FROM Inventario WHERE idProducto=" + idProducto + " AND idSucursal=" + idSucursal;
            Inventario Inventario = null;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            int ExistenciaOriginal = 0;
            if (ReaderBD.Read())
            {
                ExistenciaOriginal = ReaderBD.GetInt32(1);
                Inventario = new Inventario(ReaderBD.GetInt32(0), idProducto, "", idSucursal, "", Nuevo ? Existencia : ReaderBD.GetInt32(1) + Existencia, ExistenciaMinima, "", "", 1, "", "", 0, 0, "");
            }
            ReaderBD.Close();
            ReaderBD.Dispose();



            if (Inventario != null)
            {
                InventarioMov NuevoMovimiento = new InventarioMov()
                {
                    idInventario = Inventario.idInventario,
                    idSucursal = idSucursal,
                    Movimiento = Nuevo ? "Nuevo Inventario" : "Actualizar",
                    CantidadActual = ExistenciaOriginal,
                    Cantidad = Existencia,
                    CantidadNueva = Inventario.Existencia,
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                };
                AgregarMovimiento(NuevoMovimiento);//ASR 10-06-2018

                ActualizarInventario(Inventario);
            }
            else
            {
                Inventario = new Inventario(0, idProducto, "", idSucursal, "", Existencia, ExistenciaMinima, "", "", 1, "", "", 0, 0, "");
                InventarioMov NuevoMovimiento = new InventarioMov()
                {
                    idInventario = Inventario.idInventario,
                    idSucursal = idSucursal,
                    Movimiento = "Nuevo",
                    CantidadActual = 0,
                    Cantidad = Existencia,
                    CantidadNueva = Existencia,
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                };
                AgregarMovimiento(NuevoMovimiento);//ASR 10-06-2018

                AgregarInventario(Inventario);
            }
            Cerrar_Conexion();
            return 1;
        }

        #endregion

        /**
         * Producto
         **/
        #region Producto
        public List<ProductosCosto> ObtenerProductos()
        {
            List<ProductosCosto> ListaProductos = new List<ProductosCosto>();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones WHERE Activo = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }

        public List<ProductosCosto> ObtenerProductosRosedal()
        {
            List<ProductosCosto> ListaProductos = new List<ProductosCosto>();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones WHERE Activo = 1 AND idPresentacion = 3";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }


        public List<ProductosCosto> ObtenerProductosBusquedaDinamica(string consulta)
        {
            List<ProductosCosto> ListaProductos = new List<ProductosCosto>();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE (Producto LIKE '%" + consulta + "%' " +
                "OR Descripcion LIKE '%" + consulta + "%' " +
                "OR Color LIKE '%" + consulta + "%' " +
                "OR CodigoBarras LIKE '%" + consulta + "%' " +
                "OR Codigo LIKE '%" + consulta + "%') AND Activo = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }

        public List<ProductosCosto> ObtenerBusquedaProductosByProveedor(int idProveedor, string consulta)
        {
            List<ProductosCosto> ListaProductos = new List<ProductosCosto>();

            string Query = "SELECT Producto.idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, Costos.idproveedor, CodigoBarras, PiezasTotales " +
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "INNER JOIN Costos ON Producto.idProducto = Costos.idProducto " +
                "INNER JOIN Proveedor ON Costos.idProveedor = Proveedor.idProveedor " +
                "WHERE " +
                "(Producto LIKE '%" + consulta + "%' " +
                "OR Descripcion LIKE '%" + consulta + "%'  " +
                "OR Color LIKE '%" + consulta + "%' " +
                "OR CodigoBarras LIKE '%" + consulta + "%' " +
                "OR Codigo LIKE '%" + consulta + "%') AND Proveedor.idProveedor = " + idProveedor + " AND  Activo = 1 GROUP BY idProducto;";


            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(11), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(12), ReaderBD.GetString(9)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }


        public ProductosCosto ObtenerProductoCajaInterna(ProductosCosto producto)
        {
            ProductosCosto Producto = new ProductosCosto();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +//ASR 10-04-2018
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Producto = '" + producto.Producto + "' AND Color = '" + producto.Color + "' AND SKU = '" + producto.SKU + "' AND idPresentacion = 2";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Producto = new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Producto;
        }



        public ProductosCosto ObtenerProductoById(int idProducto)
        {
            ProductosCosto Producto = new ProductosCosto();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +//ASR 10-04-2018
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE idProducto = '" + idProducto + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Producto = new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Producto;
        }

        public ProductosCosto ObtenerProductoByCodigo(string Codigo)
        {
            ProductosCosto Producto = new ProductosCosto();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +//ASR 10-04-2018
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Codigo = '" + Codigo + "' AND Activo = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Producto = new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Producto;
        }

        public ProductosCosto ObtenerProductoByCodigoBarras(string Codigo)
        {
            ProductosCosto Producto = new ProductosCosto();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +//ASR 10-04-2018
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE CodigoBarras = '" + Codigo + "' AND Activo = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Producto = new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Producto;
        }



        public ProductosCosto ObtenerProductoByCodigotxt(string Codigo)
        {
            ProductosCosto Producto = new ProductosCosto();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +//ASR 10-04-2018
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "(WHERE Codigo = '" + Codigo + "' AND Producto.idPresentacion = 3 AND Activo = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Producto = new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Producto;
        }

        public ProductosCosto ObtenerProductoByBarras(string Codigo)
        {
            ProductosCosto Producto = new ProductosCosto();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE CodigoBarras = '" + Codigo + "' AND Activo = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Producto = new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Producto;
        }

        public ProductosCosto ObtenerProductoBySKU(string SKU)
        {
            ProductosCosto Producto = new ProductosCosto();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +//ASR 10-04-2018
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE SKU = '" + SKU + "' AND Activo = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Producto = new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Producto;
        }

        public List<ProductosCosto> ObtenerProductosByName(string NombreProducto)
        {
            List<ProductosCosto> ListaProductos = new List<ProductosCosto>();

            string Query = "SELECT idProducto, Producto, Descripcion, Codigo, SKU, Producto.idPresentacion, Presentaciones.Presentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales " +//ASR 10-04-2018
                "FROM Producto INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Producto = '" + NombreProducto + "' AND Activo = 1";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ProductosCosto(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(10), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(11), ReaderBD.GetString(9)));//ASR 10-04-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }

        public int ExisteProducto(string NombreProducto, string Color, string SKU, string CodigoBarras, int idPresentacion)
        {
            int ret = 0;
            if (SKU == null || SKU == "")//ASR 10-01-2019
                return ret;
            string Query = "SELECT idProducto FROM Producto WHERE (CodigoBarras = '" + CodigoBarras + "' OR SKU = '" + SKU + "') AND Activo = 1";


            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                if (ReaderBD.Read())
                    ret = Convert.ToInt32(ReaderBD[0]);

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ret;
        }


        public int AgregarProducto(Productos Producto)
        {
            string Query = string.Format("INSERT INTO Producto (Producto, Descripcion, Codigo, SKU, idPresentacion, imagen, Piezas, Color,CodigoBarras, PiezasTotales) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', {9});", Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.SKU, Producto.idPresentacion, Producto.ImagenP, Producto.Piezas, Producto.Color, Producto.CodigoBarras, Producto.PiezasTotales);//ASR 10-04-2018
            return ActualizarDatos(Query);
        }

        public int AgregarProductoNV(Productos Producto)
        {
            string Query = string.Format("INSERT INTO Producto (Producto, Descripcion, Codigo, SKU, idPresentacion, imagen, Piezas, Color, CodigoBarras, PiezasTotales) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}'.{9});", Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.SKU, Producto.idPresentacion, Producto.ImagenP, Producto.Piezas, Producto.Color, Producto.CodigoBarras, Producto.PiezasTotales);//ASR 10-04-2018
            return ActualizarDatos(Query);
        }

        public void ActualizarProducto(Productos Producto)
        {
            string Query;
            if (Producto.ImagenP != "")
                Query = string.Format("UPDATE Producto SET Producto = '{0}', Descripcion = '{1}', Codigo = '{2}', SKU = '{3}', idPresentacion = '{4}', imagen = '{5}', Piezas = {6}, Color = '{8}', CodigoBarras ='{9}', PiezasTotales = {10} WHERE idProducto = {7};", Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.SKU, Producto.idPresentacion, Producto.ImagenP, Producto.Piezas, Producto.idProducto, Producto.Color, Producto.CodigoBarras, Producto.PiezasTotales);//BRS 17-04-2018
            else
                Query = string.Format("UPDATE Producto SET Producto = '{0}', Descripcion = '{1}', Codigo = '{2}', SKU = '{3}', idPresentacion = '{4}', Piezas = {5}, Color = '{7}', CodigoBarras ='{8}', PiezasTotales = {9} WHERE idProducto = {6};", Producto.Producto, Producto.Descripcion, Producto.Codigo, Producto.SKU, Producto.idPresentacion, Producto.Piezas, Producto.idProducto, Producto.Color, Producto.CodigoBarras, Producto.PiezasTotales);//ASR 10-04-2018
            ActualizarDatos(Query);
        }

        public void EliminarProducto(int idProducto)
        {
            string Query = string.Format("UPDATE Producto SET Activo = 0 WHERE idProducto = '{0}';", idProducto);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Empresa
         **/
        #region Empresa
        public List<Empresa> ObtenerEmpresa()
        {
            List<Empresa> ListaEmpresa = new List<Empresa>();

            string Query = "SELECT idEmpresa, Nombre, RazonSocial, RFC, Calle, Numero, CP, Colonia, Municipio, Empresa.idEstado, NombreContacto, CorreoContacto, Telefono, Estados.Estado, Telefono " +
                "FROM Empresa " +
                "INNER JOIN Estados ON Empresa.idEstado = Estados.idEstados;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaEmpresa.Add(new Empresa(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12),
                        ReaderBD.GetString(13), ReaderBD.GetString(14)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaEmpresa;
        }

        public Empresa ObtenerEmpresaById(int idEmpresa)
        {
            Empresa Empresa = new Empresa();

            string Query = "SELECT idEmpresa, Nombre, RazonSocial, RFC, Calle, Numero, CP, Colonia, Municipio, Empresa.idEstado, NombreContacto, CorreoContacto, Telefono, Estados.Estado, Telefono " +
                "FROM Empresa " +
                "INNER JOIN Estados ON Empresa.idEstado = Estados.idEstados " +
                "WHERE idEmpresa = '" + idEmpresa + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Empresa = new Empresa(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12),
                        ReaderBD.GetString(13), ReaderBD.GetString(14));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Empresa;
        }

        public Empresa ObtenerEmpresaByRFC(string RFC)
        {
            Empresa Empresa = new Empresa();

            string Query = "SELECT idEmpresa, Nombre, RazonSocial, RFC, Calle, Numero, CP, Colonia, Municipio, Empresa.idEstado, NombreContacto, CorreoContacto, Telefono, Estados.Estado, Telefono " +
                "FROM Empresa " +
                "INNER JOIN Estados ON Empresa.idEstado = Estados.idEstados " +
                "WHERE RFC = '" + RFC + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Empresa = new Empresa(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12),
                        ReaderBD.GetString(13), ReaderBD.GetString(14));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Empresa;
        }

        public Empresa ObtenerEmpresaByName(string NombreEmpresa)
        {
            Empresa Empresa = new Empresa();

            string Query = "SELECT idEmpresa, Nombre, RazonSocial, RFC, Calle, Numero, CP, Colonia, Municipio, Empresa.idEstado, NombreContacto, CorreoContacto, Telefono, Estados.Estado, Telefono " +
                "FROM Empresa " +
                "INNER JOIN Estados ON Empresa.idEstado = Estados.idEstados " +
                "WHERE Nombre = '" + NombreEmpresa + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Empresa = new Empresa(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12),
                        ReaderBD.GetString(13), ReaderBD.GetString(14));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Empresa;
        }

        public int AgregarEmpresa(Empresa Empresa)
        {

            /*Empresa Existente = ObtenerEmpresaByRFC(Empresa.RFC);
            if (Existente.Nombre != null && Existente.Nombre != "")
                return 0;*/
            string Query = string.Format("INSERT INTO Empresa (Nombre, RazonSocial, RFC, Calle, Numero, CP, Colonia, Municipio, idEstado, NombreContacto, CorreoContacto, Telefono) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');",
                Empresa.Nombre, Empresa.RazonSocial, Empresa.RFC, Empresa.Calle, Empresa.Numero, Empresa.CP, Empresa.Colonia, Empresa.Municipio, Empresa.idEstado, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.idTelefono);
            return ActualizarDatos(Query);
        }

        public void ActualizarEmpresa(Empresa Empresa)
        {

            string Query = string.Format("UPDATE Empresa SET Nombre = '{0}', RazonSocial = '{1}', RFC = '{2}', Calle = '{3}', Numero = '{4}', CP = '{5}', Colonia = '{6}', Municipio = '{7}', idEstado = '{8}', " +
                "NombreContacto = '{9}', CorreoContacto = '{10}', Telefono = '{11}' WHERE idEmpresa = '{12}';",
                Empresa.Nombre, Empresa.RazonSocial, Empresa.RFC, Empresa.Calle, Empresa.Numero, Empresa.CP, Empresa.Colonia, Empresa.Municipio, Empresa.idEstado, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.idTelefono, Empresa.idEmpresa);
            ActualizarDatos(Query);
        }

        public void EliminarEmpresa(int idEmpresa)
        {

            string Query = string.Format("DELETE FROM Empresa WHERE idEmpresa = '{0}';", idEmpresa);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Sucursal
         **/
        #region Sucursal
        public List<Sucursal> ObtenerSucursal()
        {
            List<Sucursal> ListaSucursal = new List<Sucursal>();

            string Query = "SELECT idSucursal, Sucursal, Calle, Numero, CP, Colonia, Municipio, Sucursal.idEstado, TelefonoSucursal, Matriz, Logotipo, Estados.Estado " +
                "FROM Sucursal " +
                "INNER JOIN Estados ON Sucursal.idEstado = Estados.idEstados;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaSucursal.Add(new Sucursal(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(11), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetBoolean(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaSucursal;
        }

        public List<Sucursal> ObtenerBusquedaSucursal(string consulta)
        {
            List<Sucursal> ListaSucursal = new List<Sucursal>();

            string Query = "SELECT idSucursal, Sucursal, Calle, Numero, CP, Colonia, Municipio, Sucursal.idEstado, TelefonoSucursal, Matriz, Logotipo, Estados.Estado " +
                "FROM Sucursal " +
                "INNER JOIN Estados ON Sucursal.idEstado = Estados.idEstados " +
                "WHERE Sucursal LIKE '%" + consulta + "%' " +
                "OR Calle LIKE '%" + consulta + "%' " +
                "OR Municipio LIKE '%" + consulta + "%' " +
                "OR TelefonoSucursal LIKE '%" + consulta + "%'; ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaSucursal.Add(new Sucursal(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(11), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetBoolean(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaSucursal;
        }

        public Sucursal ObtenerSucursalById(int idSucursal)
        {
            Sucursal Sucursal = new Sucursal();

            string Query = "SELECT idSucursal, Sucursal, Calle, Numero, CP, Colonia, Municipio, Sucursal.idEstado, TelefonoSucursal, Matriz, Logotipo, Estados.Estado " +
                "FROM Sucursal " +
                "INNER JOIN Estados ON Sucursal.idEstado = Estados.idEstados " +
                "WHERE idSucursal = '" + idSucursal + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Sucursal = new Sucursal(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(11), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetBoolean(9), ReaderBD.GetString(10));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Sucursal;
        }

        public Sucursal ObtenerSucursalMatriz()
        {
            Sucursal Sucursal = new Sucursal();
            Sucursal.idSucursal = 0;

            string Query = "SELECT idSucursal, Sucursal, Calle, Numero, CP, Colonia, Municipio, Sucursal.idEstado, TelefonoSucursal, Matriz, Logotipo, Estados.Estado " +
                "FROM Sucursal " +
                "INNER JOIN Estados ON Sucursal.idEstado = Estados.idEstados " +
                "WHERE Matriz = 1";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);

            if (ReaderBD.Read())
                Sucursal = new Sucursal(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                    ReaderBD.GetString(6), ReaderBD.GetString(11), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetBoolean(9), ReaderBD.GetString(10));


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Sucursal;
        }

        public Sucursal ObtenerSucursalByName(string NombreSucursal)
        {
            Sucursal Sucursal = new Sucursal();

            string Query = "SELECT idSucursal, Sucursal, Calle, Numero, CP, Colonia, Municipio, Sucursal.idEstado, TelefonoSucursal, Matriz, Logotipo, Estados.Estado " +
                "FROM Sucursal " +
                "INNER JOIN Estados ON Sucursal.idEstado = Estados.idEstados " +
                "WHERE Sucursal = '" + NombreSucursal + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Sucursal = new Sucursal(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(11), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetBoolean(9), ReaderBD.GetString(10));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Sucursal;
        }

        public int AgregarSucursal(Sucursal Sucursal)
        {

            string Query = string.Format("INSERT INTO Sucursal (Sucursal, Calle, Numero, CP, Colonia, Municipio, idEstado, TelefonoSucursal, Matriz, Logotipo) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
                Sucursal.NSucursal, Sucursal.Calle, Sucursal.Numero, Sucursal.CP, Sucursal.Colonia, Sucursal.Municipio, Sucursal.idEstado, Sucursal.Telefono, Sucursal.Matriz == true ? 1 : 0, Sucursal.Logotipo);
            return ActualizarDatos(Query);
        }

        public void ActualizarSucursal(Sucursal Sucursal)
        {

            string Query = string.Format("UPDATE Sucursal SET Sucursal = '{0}', Calle = '{1}', Numero = '{2}', CP = '{3}', Colonia = '{4}', Municipio = '{5}', idEstado = '{6}', " +
                "TelefonoSucursal = '{7}', Matriz = '{8}', Logotipo = '{9}' WHERE idSucursal = '{10}';",
                Sucursal.NSucursal, Sucursal.Calle, Sucursal.Numero, Sucursal.CP, Sucursal.Colonia, Sucursal.Municipio, Sucursal.idEstado, Sucursal.Telefono, Sucursal.Matriz == true ? 1 : 0, Sucursal.Logotipo, Sucursal.idSucursal);
            ActualizarDatos(Query);
        }

        public void EliminarSucursal(int idSucursal)
        {

            string Query = string.Format("DELETE FROM Sucursal WHERE idSucursal = '{0}';", idSucursal);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Empleado
         **/
        #region Empleado
        public List<Empleado> ObtenerEmpleado()
        {
            List<Empleado> ListaEmpleado = new List<Empleado>();

            string Query = "SELECT idEmpleado, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, Empleado.idSucursal, Usuario, Contra, Puesto, Telefono, Privilegios, Sucursal.Sucursal, Fecha, Privilegios.Privilegio FROM Empleado " + //ASR 24-05-2018
                "INNER JOIN Sucursal ON Empleado.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Privilegios ON Empleado.Privilegios = Privilegios.idPrivilegio;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaEmpleado.Add(new Empleado(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(6),
                        ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetInt32(5), ReaderBD.GetString(11), ReaderBD.GetDateTime(12), ReaderBD.GetString(13)));//ASR 24-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaEmpleado;
        }

        public List<Empleado> ObtenerBusquedaEmpleado(string consulta)
        {
            List<Empleado> ListaEmpleado = new List<Empleado>();

            string Query = "SELECT idEmpleado, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, Empleado.idSucursal, Usuario, Contra, Puesto, Telefono, Privilegios, Sucursal.Sucursal, Fecha, Privilegios.Privilegio FROM Empleado " +//ASR 24-05-2018
                "INNER JOIN Sucursal ON Empleado.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Privilegios ON Empleado.Privilegios = Privilegios.idPrivilegio " +
                "WHERE Nombre LIKE '%" + consulta + "%' " +
                "OR ApellidoPaterno LIKE '%" + consulta + "%' " +
                "OR ApellidoMaterno LIKE '%" + consulta + "%' " +
                "OR Usuario LIKE '%" + consulta + "%' " +
                "OR Correo LIKE '%" + consulta + "%'; ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaEmpleado.Add(new Empleado(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(6),
                        ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetInt32(5), ReaderBD.GetString(11), ReaderBD.GetDateTime(12), ReaderBD.GetString(13)));//ASR 24-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaEmpleado;
        }

        public Empleado ObtenerEmpleadoById(int idEmpleado)
        {
            Empleado Empleado = new Empleado();

            string Query = "SELECT idEmpleado, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, Empleado.idSucursal, Usuario, Contra, Puesto, Telefono, Privilegios, Sucursal.Sucursal, Fecha, Privilegios.Privilegio FROM Empleado " +//ASR 24-05-2018
                "INNER JOIN Sucursal ON Empleado.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Privilegios ON Empleado.Privilegios = Privilegios.idPrivilegio " +
                "WHERE idEmpleado = '" + idEmpleado + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Empleado = new Empleado(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(6),
                        ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetInt32(5), ReaderBD.GetString(11), ReaderBD.GetDateTime(12), ReaderBD.GetString(13));//ASR 24-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Empleado;
        }

        public Empleado ObtenerEmpleadoByName(string NombreEmpleado)
        {
            Empleado Empleado = new Empleado();

            string Query = "SELECT idEmpleado, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, Empleado.idSucursal, Usuario, Contra, Puesto, Telefono, Privilegios, Sucursal.Sucursal, Fecha, Privilegios.Privilegio FROM Empleado " +//ASR 24-05-2018
                "INNER JOIN Sucursal ON Empleado.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Privilegios ON Empleado.Privilegios = Privilegios.idPrivilegio " +
                "WHERE Usuario = '" + NombreEmpleado + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Empleado = new Empleado(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(6),
                        ReaderBD.GetString(7), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetInt32(5), ReaderBD.GetString(11), ReaderBD.GetDateTime(12), ReaderBD.GetString(13));//ASR 24-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Empleado;
        }

        public int AgregarEmpleado(Empleado Empleado)
        {

            DateTime Fecha = DateTime.ParseExact(Empleado.Fecha, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            PasswordHash ph = new PasswordHash(Empleado.Contra);
            string hs = Convert.ToBase64String(ph.ToArray());
            string Query = string.Format("INSERT INTO Empleado (Nombre, ApellidoPaterno, ApellidoMaterno, Correo, idSucursal, Usuario, Contra, Puesto, Telefono, Privilegios, Fecha) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');",
                Empleado.Nombre, Empleado.ApellidoPaterno, Empleado.ApellidoMaterno, Empleado.Correo, Empleado.idSucursal, Empleado.Usuario, hs, Empleado.Puesto, Empleado.Telefono, Empleado.idPrivilegio, Fecha.ToString("yyyy/MM/dd"));//ASR 02-06-2018
            return ActualizarDatos(Query);
        }

        public void ActualizarEmpleado(Empleado Empleado)
        {

            DateTime Fecha = DateTime.ParseExact(Empleado.Fecha, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            string Query = "";

            if (Empleado.Contra == "********") //Sin comtraseña
                Query = string.Format("UPDATE Empleado SET Nombre = '{0}', ApellidoPaterno = '{1}', ApellidoMaterno = '{2}', Correo = '{3}', idSucursal = '{4}', Usuario = '{5}', " +
                "Puesto = '{6}', Telefono = '{7}', Privilegios = '{8}', Fecha = '{9}' WHERE idEmpleado = '{10}';",
                Empleado.Nombre, Empleado.ApellidoPaterno, Empleado.ApellidoMaterno, Empleado.Correo, Empleado.idSucursal, Empleado.Usuario, Empleado.Puesto, Empleado.Telefono, Empleado.idPrivilegio, Fecha.ToString("yyyy/MM/dd"), Empleado.idEmpleado);//ASR 02-06-2018
            else  // Contraseña
                Query = string.Format("UPDATE Empleado SET Nombre = '{0}', ApellidoPaterno = '{1}', ApellidoMaterno = '{2}', Correo = '{3}', idSucursal = '{4}', Usuario = '{5}', " +
                "Puesto = '{6}', Telefono = '{7}', Privilegios = '{8}', Fecha = '{9}', Contra = '{11}'  WHERE idEmpleado = '{10}';",
                Empleado.Nombre, Empleado.ApellidoPaterno, Empleado.ApellidoMaterno, Empleado.Correo, Empleado.idSucursal, Empleado.Usuario, Empleado.Puesto, Empleado.Telefono, Empleado.idPrivilegio, Fecha.ToString("yyyy/MM/dd"), Empleado.idEmpleado, Convert.ToBase64String(new PasswordHash(Empleado.Contra).ToArray()));//BRS 02-06-2018
            ActualizarDatos(Query);
        }

        public void ActualizarPassEmpleado(Empleado Empleado)
        {

            PasswordHash ph = new PasswordHash(Empleado.Contra);
            string hs = Convert.ToBase64String(ph.ToArray());
            string Query = string.Format("UPDATE Empleado SET Contra = '{0}' WHERE Usuario = '{1}';", hs, Empleado.Usuario);
            ActualizarDatos(Query);
        }

        public void EliminarEmpleado(int idEmpleado)
        {

            string Query = string.Format("DELETE FROM Empleado WHERE idEmpleado = '{0}';", idEmpleado);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Cliente
         **/
        #region Cliente
        public List<Cliente> ObtenerCliente()
        {
            List<Cliente> ListaCliente = new List<Cliente>();

            string Query = "SELECT idCliente, Cliente.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Cliente.idBanco, Cliente.idTipodePago, Descuento, NumeroConvenio, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Cliente " +//ASR 17-05-2018
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Cliente.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Cliente.idTipodePago = TipodePago.idTipodePago;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaCliente.Add(new Cliente(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(9), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(10), ReaderBD.GetInt32(6), ReaderBD.GetString(11), ReaderBD.GetDouble(7), ReaderBD.GetString(8), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetString(14)));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaCliente;
        }

        public List<Cliente> ObtenerBusquedaCliente(string consulta)
        {
            List<Cliente> ListaCliente = new List<Cliente>();

            string Query = "SELECT idCliente, Cliente.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Cliente.idBanco, Cliente.idTipodePago, Descuento, NumeroConvenio, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Cliente " +//ASR 17-05-2018
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Cliente.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Cliente.idTipodePago = TipodePago.idTipodePago " +
                "WHERE Empresa.Nombre LIKE '%" + consulta + "%' " +
                "OR NumeroTarjeta LIKE '%" + consulta + "%';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaCliente.Add(new Cliente(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(9), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(10), ReaderBD.GetInt32(6), ReaderBD.GetString(11), ReaderBD.GetDouble(7), ReaderBD.GetString(8), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetString(14)));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaCliente;
        }

        public Cliente ObtenerClienteById(int idCliente)
        {
            Cliente Cliente = new Cliente();

            string Query = "SELECT idCliente, Cliente.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Cliente.idBanco, Cliente.idTipodePago, Descuento, NumeroConvenio, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Cliente " +//ASR 17-05-2018
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Cliente.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Cliente.idTipodePago = TipodePago.idTipodePago " +
                "WHERE idCliente = '" + idCliente + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Cliente = new Cliente(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(9), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(10), ReaderBD.GetInt32(6), ReaderBD.GetString(11), ReaderBD.GetDouble(7), ReaderBD.GetString(8), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetString(14));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Cliente;
        }

        public Cliente ObtenerClienteByConvenio(string Convenio)
        {
            Cliente Cliente = new Cliente();

            string Query = "SELECT idCliente, Cliente.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Cliente.idBanco, Cliente.idTipodePago, Descuento, NumeroConvenio, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Cliente " +//ASR 17-05-2018
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Cliente.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Cliente.idTipodePago = TipodePago.idTipodePago " +
                "WHERE NumeroConvenio = '" + Convenio + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Cliente = new Cliente(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(9), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(10), ReaderBD.GetInt32(6), ReaderBD.GetString(11), ReaderBD.GetDouble(7), ReaderBD.GetString(8), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetString(14));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Cliente;
        }

        public int AgregarCliente(Cliente Cliente)
        {

            /*Validar y mandar mensaje de error CORREGIR
            if (ObtenerClienteByConvenio(Cliente.NumeroConvenio).NumeroConvenio != null )
                return 0;*/
            string Query = string.Format("INSERT INTO Cliente (idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, idBanco, idTipodePago, Descuento, NumeroConvenio) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');",
                Cliente.idEmpresa, Cliente.DiasCredito, Cliente.CantidadCredito, " ", 1, 1, Cliente.Descuento, Cliente.NumeroConvenio);//ASR 10-04-2018
            return ActualizarDatos(Query);
        }

        public void ActualizarCliente(Cliente Cliente)
        {

            string Query = string.Format("UPDATE Cliente SET idEmpresa = '{0}', DiasCredito = '{1}', CantidadCredito = '{2}', NumeroTarjeta = '{3}', idBanco = '{4}', idTipodePago = '{5}', Descuento = '{6}', " +
                "NumeroConvenio = '{7}' WHERE idCliente = '{8}';",
                Cliente.idEmpresa, Cliente.DiasCredito, Cliente.CantidadCredito, " ", 1, 1,/*Cliente.NumeroTarjeta, Cliente.idBanco, Cliente.idTipodePago*/ Cliente.Descuento, Cliente.NumeroConvenio, Cliente.idCliente);
            ActualizarDatos(Query);
        }

        public void EliminarCliente(int idCliente)
        {

            string Query = string.Format("DELETE FROM Cliente WHERE idCliente = '{0}';", idCliente);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Proveedor
         **/
        #region Proveedor
        public List<Proveedor> ObtenerProveedor()
        {
            List<Proveedor> ListaProveedor = new List<Proveedor>();

            string Query = "SELECT idProveedor, Proveedor.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Proveedor.idBanco, Proveedor.idTipoPago, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Proveedor " +//ASR 17-05-2018
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Proveedor.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Proveedor.idTipoPago = TipodePago.idTipodePago;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProveedor.Add(new Proveedor(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(7), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(8), ReaderBD.GetInt32(6), ReaderBD.GetString(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12)));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProveedor;
        }

        public List<Proveedor> ObtenerProveedorBusquedaDinamica(string consulta)
        {
            List<Proveedor> ListaProveedor = new List<Proveedor>();

            string Query = "SELECT idProveedor, Proveedor.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Proveedor.idBanco, Proveedor.idTipoPago, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Proveedor " +//ASR 17-05-2018
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Proveedor.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Proveedor.idTipoPago = TipodePago.idTipodePago " +
                "WHERE Empresa.Nombre LIKE '%" + consulta + "%' " +
                "OR NumeroTarjeta LIKE '%" + consulta + "%';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProveedor.Add(new Proveedor(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(7), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(8), ReaderBD.GetInt32(6), ReaderBD.GetString(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12)));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProveedor;
        }

        public Proveedor ObtenerProveedorById(int idProveedor)
        {
            Proveedor Proveedor = new Proveedor();

            string Query = "SELECT idProveedor, Proveedor.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Proveedor.idBanco, Proveedor.idTipoPago, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Proveedor " +//ASR 17-05-2018
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Proveedor.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Proveedor.idTipoPago = TipodePago.idTipodePago " +
                "WHERE idProveedor = '" + idProveedor + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Proveedor = new Proveedor(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(7), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(8), ReaderBD.GetInt32(6), ReaderBD.GetString(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Proveedor;
        }
        public Proveedor ObtenerProveedorByNombre(string nombre)
        {
            Proveedor Proveedor = new Proveedor();

            string Query = "SELECT idProveedor, Proveedor.idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, Proveedor.idBanco, Proveedor.idTipoPago, " +
                "Empresa.Nombre, Banco.Banco, TipodePago.TipodePago, Empresa.NombreContacto, Empresa.CorreoContacto, Empresa.Telefono FROM Proveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Banco ON Proveedor.idBanco = Banco.idBanco " +
                "INNER JOIN TipodePago ON Proveedor.idTipoPago = TipodePago.idTipodePago " +
                "WHERE Empresa.Nombre = '" + nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Proveedor = new Proveedor(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(7), ReaderBD.GetInt32(2), ReaderBD.GetDouble(3), ReaderBD.GetString(4),
                        ReaderBD.GetInt32(5), ReaderBD.GetString(8), ReaderBD.GetInt32(6), ReaderBD.GetString(9), ReaderBD.GetString(10), ReaderBD.GetString(11), ReaderBD.GetString(12));//ASR 17-05-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Proveedor;
        }

        public int AgregarProveedor(Proveedor Proveedor)
        {

            string Query = string.Format("INSERT INTO Proveedor (idEmpresa, DiasCredito, CantidadCredito, NumeroTarjeta, idBanco, idTipoPago) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');",
                Proveedor.idEmpresa, Proveedor.DiasCredito, Proveedor.CantidadCredito, " ", 1, 1); //ASR 10-04-2018
            return ActualizarDatos(Query);
        }

        public void ActualizarProveedor(Proveedor Proveedor)
        {

            string Query = string.Format("UPDATE Proveedor SET idEmpresa = '{0}', DiasCredito = '{1}', CantidadCredito = '{2}', NumeroTarjeta = '{3}', idBanco = '{4}', idTipoPago = '{5}' WHERE idProveedor = '{6}';",
                Proveedor.idEmpresa, Proveedor.DiasCredito, Proveedor.CantidadCredito, " ", 1, 1, Proveedor.idProveedor);//ASR 10-04-2018
            ActualizarDatos(Query);
        }

        public void EliminarProveedor(int idProveedor)
        {

            string Query = string.Format("DELETE FROM Proveedor WHERE idProveedor = '{0}';", idProveedor);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Tipo de Pago
         **/
        #region TipodePago
        public List<TiposdePago> ObtenerTipodePago()
        {
            List<TiposdePago> ListaTipodePago = new List<TiposdePago>();

            string Query = "SELECT idTipodePago, TipodePago FROM TipodePago";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaTipodePago.Add(new TiposdePago(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaTipodePago;
        }

        public TiposdePago ObtenerTipodePagoById(int idTipodePago)
        {
            TiposdePago Pais = new TiposdePago();

            string Query = "SELECT idTipodePago, TipodePago FROM TipodePago WHERE idTipodePago = '" + idTipodePago + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Pais = new TiposdePago(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Pais;
        }

        public TiposdePago ObtenerTipodePagoByTipo(string NombreTipodePago)
        {
            TiposdePago TipodePago = new TiposdePago();

            string Query = "SELECT idTipodePago, TipodePago FROM TipodePago WHERE TipodePago = '" + NombreTipodePago + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    TipodePago = new TiposdePago(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return TipodePago;
        }

        public int AgregarTipodePago(TiposdePago TipodePago)
        {

            string Query = string.Format("INSERT INTO TipodePago (TipodePago) VALUES ('{0}');", TipodePago.TipodePago);
            return ActualizarDatos(Query);
        }

        public void ActualizarTipodePago(TiposdePago TipodePago)
        {

            string Query = string.Format("UPDATE TipodePago SET TipodePago = '{0}' WHERE idTipodePago = '{1}';", TipodePago.TipodePago, TipodePago.idTipodePago);
            ActualizarDatos(Query);
        }

        public void EliminarTipodePago(int idTipodePago)
        {

            string Query = string.Format("DELETE FROM TipodePago WHERE idTipodePago = '{0}';", idTipodePago);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Precio
         **/
        #region Precio
        public List<Precios> ObtenerPrecios()
        {
            List<Precios> ListaPrecios = new List<Precios>();

            string Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                        ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }
        
        public List<Precios> ObtenerBusquedaPrecios(string consulta)
        {
            List<Precios> ListaPrecios = new List<Precios>();

            string Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE EtiquetaPrecio LIKE '%" + consulta + "%' " +
                "OR Sucursal.Sucursal LIKE '%" + consulta + "%' " + 
                "OR Minimo LIKE '%" + consulta + "%' " +
                "OR Producto.Producto LIKE '%" + consulta + "%' " +
                "OR Maximo LIKE '%" + consulta + "%'; ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                        ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }
        public Precios ObtenerPrecioById(int idPrecios)
        {
            Precios Proveedor = new Precios();

            string Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE idPrecios = '" + idPrecios + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Proveedor = new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                        ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Proveedor;
        }
        
        public Precios ObtenerPrecioMaxById(int idPrecios)
        {
            Precios Proveedor = new Precios();


            int Cantidad = 0;
            string Query = "Select Max(Maximo) From Precios where idProducto = " + idPrecios;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    if (ReaderBD[0] != DBNull.Value)
                        Cantidad = ReaderBD.GetInt32(0);
                }

            Proveedor.Minimo = Cantidad + 1;
            Proveedor.Maxima = Cantidad + 2;

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Proveedor;
        }

        public List<Precios> ObtenerPreciosByEtiqueta(string EtiquetaPrecio)
        {
            List<Precios> ListaPrecios = new List<Precios>();

            string Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE EtiquetaPrecio = '" + EtiquetaPrecio + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                        ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }

        public List<Precios> ObtenerPreciosByEtiquetaandSuc(string EtiquetaPrecio, int idSucursal)
        {
            List<Precios> ListaPrecios = new List<Precios>();

            string Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE EtiquetaPrecio = '" + EtiquetaPrecio + "' AND Precios.idSucursal = '" + idSucursal + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                        ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }

        public List<Precios> ObtenerPreciosByProductoySucursal(int idSucursal, int idProducto)
        {
            List<Precios> ListaPrecios = new List<Precios>();

            string Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Precios.idSucursal = '" + idSucursal + "' AND Precios.idProducto = '" + idProducto + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                        ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }

        public Precios ObtenerPreciosByCantidad(int idSucursal, int idProducto, int cantidad)
        {
            Precios ListaPrecios = new Precios();

            string Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Precios.idSucursal = '" + idSucursal + "' AND Precios.idProducto = '" + idProducto + "' AND Minimo <= '" + cantidad + "' AND '" + cantidad + "' <= Maximo ;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios = new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                        ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10));
            if (ListaPrecios.Precio == 0 && ListaPrecios.EtiquetaPrecio == null)
            {
                Query = "SELECT idPrecios, EtiquetaPrecio, Minimo, Maximo, Precio, Precios.idProducto, Precios.idSucursal, Producto.Producto, Sucursal.Sucursal, Producto.Color, Presentaciones.Presentacion FROM Precios " +
                "INNER JOIN Producto ON Precios.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON Precios.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE Precios.idSucursal = '" + idSucursal + "' AND Precios.idProducto = '" + idProducto + "' AND Maximo = (SELECT MAX(Maximo) FROM Precios);";
                ReaderBD = ObtenerDatosBD(Query);
                if (ReaderBD != null)
                    while (ReaderBD.Read())
                        ListaPrecios = new Precios(ReaderBD.GetInt32(0), ReaderBD.GetInt32(5), ReaderBD.GetString(7), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetInt32(3),
                            ReaderBD.GetDouble(4), ReaderBD.GetInt32(6), ReaderBD.GetString(8), ReaderBD.GetString(9), ReaderBD.GetString(10));
            }

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }

        public int AgregarPrecios(Precios Precios)
        {
            string Query = string.Format("INSERT INTO Precios (EtiquetaPrecio, Minimo, Maximo, Precio, idProducto, idSucursal) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');",
                Precios.EtiquetaPrecio, Precios.Minimo, Precios.Maxima, Precios.Precio, Precios.idProducto, Precios.idSucursal);
            return ActualizarDatos(Query);
        }

        public void ActualizarPrecios(Precios Precios)
        {

            string Query = string.Format("UPDATE Precios SET EtiquetaPrecio = '{0}', Minimo = '{1}', Maximo = '{2}', Precio = '{3}', idProducto = '{4}' WHERE idPrecios = '{5}';",
                Precios.EtiquetaPrecio, Precios.Minimo, Precios.Maxima, Precios.Precio, Precios.idProducto, Precios.idPrecio);
            ActualizarDatos(Query);
        }

        public void EliminarPrecios(int idPrecio)
        {

            string Query = string.Format("DELETE FROM Precios WHERE idPrecios = '{0}';", idPrecio);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * IVA
         **/
        #region IVA
        public List<IVAs> ObtenerIVAs()
        {
            List<IVAs> ListaIVA = new List<IVAs>();

            string Query = "SELECT idIVA, IVA FROM IVA;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaIVA.Add(new IVAs(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaIVA;
        }

        public IVAs ObtenerIVAById(int idIVA)
        {
            IVAs IVA = new IVAs();

            string Query = "SELECT idIVA, IVA FROM IVA WHERE idIVA = '" + idIVA + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    IVA = new IVAs(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return IVA;
        }

        public IVAs ObtenerIVAByIVA(double IVAs)
        {
            IVAs IVA = new IVAs();

            string Query = "SELECT idIVA, IVA FROM IVA WHERE IVA = '" + IVAs + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    IVA = new IVAs(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return IVA;
        }

        public int AgregarIVA(IVAs IVA)
        {

            string Query = string.Format("INSERT INTO IVA (IVA) VALUES ('{0}');", IVA.IVA);
            return ActualizarDatos(Query);
        }

        public void ActualizarIVA(IVAs IVA)
        {

            string Query = string.Format("UPDATE IVA SET IVA = '{0}' WHERE idIVA = '{1}';", IVA.IVA, IVA.idIVA);
            ActualizarDatos(Query);
        }

        public void EliminarIVA(int idIVA)
        {

            string Query = string.Format("DELETE FROM IVA WHERE idIVA = '{0}';", idIVA);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Tipo Cliente
         **/
        #region Tipo Cliente
        public List<TipoClientes> ObtenerTipoClientes()
        {
            List<TipoClientes> ListaTipoClientes = new List<TipoClientes>();

            string Query = "SELECT idTipoCliente, TipoCliente FROM TipoCliente;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaTipoClientes.Add(new TipoClientes(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaTipoClientes;
        }

        public TipoClientes ObtenerTipoClienteById(int idTipoCliente)
        {
            TipoClientes TipoCliente = new TipoClientes();

            string Query = "SELECT idTipoCliente, TipoCliente FROM TipoCliente WHERE idTipoCliente = '" + idTipoCliente + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    TipoCliente = new TipoClientes(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return TipoCliente;
        }

        public TipoClientes ObtenerTipoClienteByTipo(string Tipo)
        {
            TipoClientes TipoCliente = new TipoClientes();

            string Query = "SELECT idTipoCliente, TipoCliente FROM TipoCliente WHERE TipoCliente = '" + Tipo + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    TipoCliente = new TipoClientes(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return TipoCliente;
        }

        public int AgregarTipoCliente(TipoClientes TipoCliente)
        {

            string Query = string.Format("INSERT INTO TipoCliente (TipoCliente) VALUES ('{0}');", TipoCliente.TipoCliente);
            return ActualizarDatos(Query);
        }

        public void ActualizarTipoCliente(TipoClientes TipoCliente)
        {

            string Query = string.Format("UPDATE TipoCliente SET TipoCliente = '{0}' WHERE idTipoCliente = '{1}';", TipoCliente.TipoCliente, TipoCliente.idTipoCliente);
            ActualizarDatos(Query);
        }

        public void EliminarTipoCliente(int idTipoCliente)
        {

            string Query = string.Format("DELETE FROM TipoCliente WHERE idTipoCliente = '{0}';", idTipoCliente);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Operaciones
         **/
        #region Operaciones
        public List<Operaciones> ObtenerOperaciones()
        {
            List<Operaciones> ListaOperaciones = new List<Operaciones>();

            string Query = "SELECT idOperaciones, Operacion FROM Operaciones;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOperaciones.Add(new Operaciones(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOperaciones;
        }

        public Operaciones ObtenerOperacionById(int idOperacion)
        {
            Operaciones TipoCliente = new Operaciones();

            string Query = "SELECT idOperaciones, Operacion FROM Operaciones WHERE idOperaciones = '" + idOperacion + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    TipoCliente = new Operaciones(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return TipoCliente;
        }

        public Operaciones ObtenerOperacionByTipo(string Operaciones)
        {
            Operaciones Operacion = new Operaciones();

            string Query = "SELECT idOperaciones, Operacion FROM Operaciones WHERE Operacion = '" + Operaciones + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Operacion = new Operaciones(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Operacion;
        }

        public int AgregarOperacion(Operaciones Operacion)
        {

            string Query = string.Format("INSERT INTO Operaciones (Operacion) VALUES ('{0}');", Operacion.Operacion);
            return ActualizarDatos(Query);
        }

        public void ActualizarOperacion(Operaciones Operacion)
        {

            string Query = string.Format("UPDATE Operaciones SET Operacion = '{0}' WHERE idOperaciones = '{1}';", Operacion.Operacion, Operacion.idOperacion);
            ActualizarDatos(Query);
        }

        public void EliminarOperacion(int idOperacion)
        {

            string Query = string.Format("DELETE FROM Operaciones WHERE idOperaciones = '{0}';", idOperacion);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * EstadoProceso
         **/
        #region EstadoProceso
        public List<EstadoProceso> ObtenerEstadoProceso()
        {
            List<EstadoProceso> ListaEstadoProceso = new List<EstadoProceso>();

            string Query = "SELECT idEstadoProceso, EstadoProceso FROM EstadoProceso;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaEstadoProceso.Add(new EstadoProceso(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaEstadoProceso;
        }

        public EstadoProceso ObtenerEstatusById(int idEstatus)
        {
            EstadoProceso Estatus = new EstadoProceso();

            string Query = "SELECT idEstadoProceso, EstadoProceso FROM EstadoProceso WHERE idEstadoProceso = '" + idEstatus + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Estatus = new EstadoProceso(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Estatus;
        }

        public EstadoProceso ObtenerEstatusByName(string EstadoProceso)
        {
            EstadoProceso Estatus = new EstadoProceso();

            string Query = "SELECT idEstadoProceso, EstadoProceso FROM EstadoProceso WHERE EstadoProceso = '" + EstadoProceso + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Estatus = new EstadoProceso(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Estatus;
        }

        public int AgregarEstatus(EstadoProceso Estatus)
        {

            string Query = string.Format("INSERT INTO EstadoProceso (EstadoProceso) VALUES ('{0}');", Estatus.Estatus);
            return ActualizarDatos(Query);
        }

        public void ActualizarEstatus(EstadoProceso Estatus)
        {

            string Query = string.Format("UPDATE EstadoProceso SET EstadoProceso = '{0}' WHERE idEstadoProceso = '{1}';", Estatus.Estatus, Estatus.idEstatus);
            ActualizarDatos(Query);
        }

        public void EliminarEstatus(int idEstatus)
        {

            string Query = string.Format("DELETE FROM EstadoProceso WHERE idEstadoProceso = '{0}';", idEstatus);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Factura
         **/
        #region Factura
        public List<Factura> ObtenerFactura()
        {
            List<Factura> ListaFactura = new List<Factura>();

            string Query = "SELECT idFactura, Descripcion, Directorio, idOrdenCVP FROM Factura;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaFactura.Add(new Factura(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaFactura;
        }

        public Factura ObtenerFacturaById(int idFactura)
        {
            Factura Factura = new Factura();

            string Query = "SELECT idFactura, Descripcion, Directorio, idOrdenCVP FROM Factura WHERE idFactura = '" + idFactura + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Factura = new Factura(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Factura;
        }

        public Factura ObtenerFacturaByOrden(int idOrden)
        {
            Factura Factura = new Factura();

            string Query = "SELECT idFactura, Descripcion, Directorio, idOrdenCVP FROM Factura WHERE idOrdenCVP = '" + idOrden + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Factura = new Factura(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Factura;
        }

        public int AgregarFactura(Factura Factura)
        {

            string Query = string.Format("INSERT INTO Factura (Descripcion, Directorio, idOrdenCVP) VALUES ('{0}', '{1}', '{2}');", Factura.Descripcion, Factura.Directorio, Factura.idOrdenCVP);
            return ActualizarDatos(Query);
        }

        public void ActualizarFactura(Factura Factura)
        {

            string Query = string.Format("UPDATE Factura SET Descripcion = '{0}', Directorio = '{1}', idOrdenCVP = '{2}' WHERE idFactura = '{3}';", Factura.Descripcion, Factura.Directorio, Factura.idOrdenCVP, Factura.idFactura);
            ActualizarDatos(Query);
        }

        public void EliminarFactura(int idFactura)
        {

            string Query = string.Format("DELETE FROM Factura WHERE idFactura = '{0}';", idFactura);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Serie
         **/
        #region Serie
        public List<Serie> ObtenerSerie()
        {
            List<Serie> ListaSerie = new List<Serie>();

            string Query = "SELECT idSerie, Prefijo, NumeroSerie FROM Serie;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaSerie.Add(new Serie(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDouble(2)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaSerie;
        }

        public Serie ObtenerSerieById(int idSerie)
        {
            Serie Serie = new Serie();

            string Query = "SELECT idSerie, Prefijo, NumeroSerie FROM Serie WHERE idSerie = '" + idSerie + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Serie = new Serie(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDouble(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Serie;
        }

        public List<Serie> ObtenerSerieByPrefijo(string Prefijo)
        {
            List<Serie> ListaSerie = new List<Serie>();

            string Query = "SELECT idSerie, Prefijo, NumeroSerie FROM Serie WHERE Prefijo = '" + Prefijo + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaSerie.Add(new Serie(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDouble(2)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaSerie;
        }

        public int AgregarSerie(Serie Serie)
        {

            string Query = string.Format("INSERT INTO Serie (Prefijo, NumeroSerie) VALUES ('{0}', '{1}');", Serie.Prefijo, Serie.Numero);
            return ActualizarDatos(Query);
        }

        public void ActualizarSerie(Serie Serie)
        {

            string Query = string.Format("UPDATE Serie SET Prefijo = '{0}', NumeroSerie = '{1}' WHERE idSerie = '{2}';", Serie.Prefijo, Serie.Numero, Serie.idSerie);
            ActualizarDatos(Query);
        }

        public void EliminarSerie(int idSerie)
        {

            string Query = string.Format("DELETE FROM Serie WHERE idSerie = '{0}';", idSerie);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Recepcion Mercancia
         **/
        #region Recepcion Mercancia
        public List<RecepcionMerca> ObtenerRecepcionMerca()
        {
            List<RecepcionMerca> ListaRecepcionMerca = new List<RecepcionMerca>();

            string Query = "SELECT idRecepcionMercancia, idOrdenCVP, TipoDocumento FROM RecepcionMercancia;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaRecepcionMerca.Add(new RecepcionMerca(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaRecepcionMerca;
        }

        public RecepcionMerca ObtenerRecepcionById(int idRecepcion)
        {
            RecepcionMerca Recepcion = new RecepcionMerca();

            string Query = "SELECT idRecepcionMercancia, idOrdenCVP, TipoDocumento FROM RecepcionMercancia WHERE idRecepcionMercancia = '" + idRecepcion + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Recepcion = new RecepcionMerca(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Recepcion;
        }

        public RecepcionMerca ObtenerRecepcionByOrden(int idOrdenCVP)
        {
            RecepcionMerca Recepcion = new RecepcionMerca();

            string Query = "SELECT idRecepcionMercancia, idOrdenCVP, TipoDocumento FROM RecepcionMercancia WHERE idOrdenCVP = '" + idOrdenCVP + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Recepcion = new RecepcionMerca(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Recepcion;
        }

        public int AgregarRecepcion(RecepcionMerca Recepcion)
        {

            string Query = string.Format("INSERT INTO RecepcionMercancia (idOrdenCVP, TipoDocumento) VALUES ('{0}', '{1}');", Recepcion.idOrdenCVP, Recepcion.TipoDocumento);
            return ActualizarDatos(Query);
        }

        public void ActualizarRecepcion(RecepcionMerca Recepcion)
        {

            string Query = string.Format("UPDATE RecepcionMercancia SET idOrdenCVP = '{0}', TipoDocumento = '{1}' WHERE idRecepcionMercancia = '{2}';", Recepcion.idOrdenCVP, Recepcion.TipoDocumento, Recepcion.idRecepcion);
            ActualizarDatos(Query);
        }

        public void EliminarRecepcion(int idRecepcion)
        {

            string Query = string.Format("DELETE FROM RecepcionMercancia WHERE idRecepcionMercancia = '{0}';", idRecepcion);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * Lista Productos
         **/
        #region Lista Productos
        public List<ListaProductos> ObtenerListaProductos()
        {
            List<ListaProductos> ListaProductos = new List<ListaProductos>();

            string Query = "SELECT idListaProductos, LP.idProducto, Producto.Producto, LP.idPresentacion, LP.Cantidad, CostoPrecio, ImporteTotal, IEPS, " +
                "LP.idOrdenCompraV, Producto.Codigo, Presentaciones.Presentacion FROM ListaProductos LP " +
                "INNER JOIN Producto ON LP.idProducto = Producto.idProducto " +
                "INNER JOIN Presentaciones ON LP.idPresentacion = Presentaciones.idPresentaciones;";


            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ListaProductos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetString(9), ReaderBD.GetInt32(3), ReaderBD.GetString(10),
                        ReaderBD.GetInt32(4), ReaderBD.GetDouble(5), ReaderBD["idOrdenCompraV"] == DBNull.Value ? 0 : ReaderBD.GetDouble(6), ReaderBD.GetDouble(7), ReaderBD["idOrdenCompraV"] == DBNull.Value ? 0 : ReaderBD.GetInt32(8), 0));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }

        public List<ListaProductos> ObtenerProductosVendidos()
        {
            List<ListaProductos> ListaProductos = new List<ListaProductos>();
            OrdenesCVP OrdenVenta = new OrdenesCVP();

            string Query = "SELECT idListaProductos, LP.idProducto, Producto.Producto, LP.idPresentacion, SUM(LP.Cantidad), CostoPrecio, ImporteTotal, IEPS, " +
                "LP.idOrdenCompraV, Producto.Codigo, Presentaciones.Presentacion, OrdenesCVP.idOperacion FROM ListaProductos LP " +
                "INNER JOIN Producto ON LP.idProducto = Producto.idProducto " +
                "INNER JOIN Presentaciones ON LP.idPresentacion = Presentaciones.idPresentaciones " +
                "INNER JOIN OrdenesCVP ON LP.idOrdenCompraV = OrdenesCVP.idOrdenesCVP " +
                "WHERE OrdenesCVP.idOperacion = 3 GROUP BY LP.idProducto DESC;";


            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ListaProductos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetString(9), ReaderBD.GetInt32(3), ReaderBD.GetString(10),
                        ReaderBD.GetInt32(4), ReaderBD.GetDouble(5), ReaderBD.GetDouble(6), ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), 0));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }

        public ListaProductos ObtenerListaProductoById(int idListaProducto)
        {
            ListaProductos ListaProducto = new ListaProductos();

            string Query = "SELECT idListaProductos, LP.idProducto, Producto.Producto, LP.idPresentacion, LP.Cantidad, CostoPrecio, ImporteTotal, IEPS, " +
                "LP.idOrdenCompraV, Producto.Codigo, Presentaciones.Presentacion FROM ListaProductos LP " +
                "INNER JOIN Producto ON LP.idProducto = Producto.idProducto " +
                "INNER JOIN Presentaciones ON LP.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE idListaProductos = '" + idListaProducto + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProducto = new ListaProductos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetString(9), ReaderBD.GetInt32(3), ReaderBD.GetString(10),
                        ReaderBD.GetInt32(4), ReaderBD.GetDouble(5), ReaderBD.GetDouble(6), ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), 0);

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProducto;
        }

        public List<ListaProductos> ObtenerProductosByOrden(int OrdenCVP)
        {
            List<ListaProductos> ListaProductos = new List<ListaProductos>();

            string Query = "SELECT idListaProductos, LP.idProducto, Producto.Producto, LP.idPresentacion, LP.Cantidad, CostoPrecio, ImporteTotal, IEPS, " +
                "LP.idOrdenCompraV, Producto.Codigo, Presentaciones.Presentacion, Producto.Color FROM ListaProductos LP " +//ASR 02-06-2018
                "INNER JOIN Producto ON LP.idProducto = Producto.idProducto " +
                "INNER JOIN Presentaciones ON LP.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE idOrdenCompraV = '" + OrdenCVP + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ListaProductos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetString(9), ReaderBD.GetInt32(3), ReaderBD.GetString(10),
                        ReaderBD.GetInt32(4), ReaderBD.GetDouble(5), ReaderBD.GetDouble(6), ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), 0, ReaderBD.GetString(11)));//ASR 02-06-2018

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }

        public List<ListaProductos> ObtenerProductosByTransfer(int idTransfer)
        {
            List<ListaProductos> ListaProductos = new List<ListaProductos>();

            string Query = "SELECT idListaProductos, LP.idProducto, Producto.Producto, LP.idPresentacion, LP.Cantidad, CostoPrecio, ImporteTotal, IEPS, " +
                "LP.idTransferencia, Producto.Codigo, Presentaciones.Presentacion, Producto.Color " +
                "FROM ListaProductos LP " +
                "INNER JOIN Producto ON LP.idProducto = Producto.idProducto " +
                "INNER JOIN Presentaciones ON LP.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE idTransferencia = '" + idTransfer + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ListaProductos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetString(9), ReaderBD.GetInt32(3), ReaderBD.GetString(10),
                        ReaderBD.GetInt32(4), ReaderBD.GetDouble(5), ReaderBD.GetDouble(6), ReaderBD.GetDouble(7), 0, ReaderBD.GetInt32(8), ReaderBD.GetString(11)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }

        public int AgregarListaProducto(ListaProductos ListaProducto)
        {

            string Query = string.Format("INSERT INTO ListaProductos (idProducto, NombreProducto, idPresentacion, Cantidad, CostoPrecio, ImporteTotal, IEPS, idOrdenCompraV) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');",
                ListaProducto.idProducto, ListaProducto.Producto, ListaProducto.idPresentacion, ListaProducto.Cantidad, ListaProducto.CostoPrecio, ListaProducto.ImporteTotal, ListaProducto.IEPS, ListaProducto.idOrdenCVP);
            return ActualizarDatos(Query);
        }

        public int AgregarListaProductoTransferencia(ListaProductos ListaProducto)
        {

            ListaProducto.Producto = ObtenerProductoById(ListaProducto.idProducto).Producto;
            ListaProducto.idPresentacion = ObtenerProductoById(ListaProducto.idProducto).idPresentacion;
            string Query = string.Format("INSERT INTO ListaProductos (idProducto, NombreProducto, idPresentacion, Cantidad, CostoPrecio, ImporteTotal, IEPS, idTransferencia) VALUES ('{0}', '{1}', '{2}', '{3}', 0, 0, 0, '{4}');",
                ListaProducto.idProducto, ListaProducto.Producto, ListaProducto.idPresentacion, ListaProducto.Cantidad, ListaProducto.idTransferencia);
            return ActualizarDatos(Query);
        }

        public void ActualizarListaProducto(ListaProductos ListaProducto)
        {

            string Query = string.Format("UPDATE ListaProductos SET idProducto = '{0}', NombreProducto = '{1}', idPresentacion = '{2}', Cantidad = '{3}', CostoPrecio = '{4}', ImporteTotal = '{5}', IEPS = '{6}', idOrdenCompraV = '{7}' WHERE idListaProductos = '{8}';",
                ListaProducto.idProducto, ListaProducto.Producto, ListaProducto.idPresentacion, ListaProducto.Cantidad, ListaProducto.CostoPrecio, ListaProducto.ImporteTotal, ListaProducto.IEPS, ListaProducto.idOrdenCVP, ListaProducto.idListaProductos);
            ActualizarDatos(Query);
        }

        public void EliminarListaProducto(int idListaProductos)
        {

            string Query = string.Format("DELETE FROM ListaProductos WHERE idListaProductos = '{0}';", idListaProductos);
            ActualizarDatos(Query);
        }
        #endregion

        /**
         * OrdenesCVP
         **/
        #region OrdenesCVP
        public List<OrdenesCVP> ObtenerOrdenesC()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idProveedor, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Proveedor ON OrdenesCVP.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 1 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), 0, ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ""));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerOrdenesCP()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idProveedor, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Proveedor ON OrdenesCVP.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 1 AND Disponible = 1 AND (OrdenesCVP.idEstadoProceso = 1 OR OrdenesCVP.idEstadoProceso = 2 OR OrdenesCVP.idEstadoProceso = 3)";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), 0, ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ""));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerOrdenesCR()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idProveedor, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Proveedor ON OrdenesCVP.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 1 AND Disponible = 1 AND (OrdenesCVP.idEstadoProceso = 5  OR OrdenesCVP.idEstadoProceso = 3)";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), 0, ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ""));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerOrdenesCPR()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idProveedor, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Proveedor ON OrdenesCVP.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 1 AND Disponible = 1 AND (OrdenesCVP.idEstadoProceso = 4  OR OrdenesCVP.idEstadoProceso = 2)";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), 0, ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ""));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerBusquedaOrdenesC(string consulta)
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idProveedor, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Proveedor ON OrdenesCVP.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE Empresa.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 1 AND Disponible = 1 " +
                "OR Sucursal.Sucursal LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 1 AND Disponible = 1 " +
                "OR OrdenesCVP.Fecha LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 1 AND Disponible = 1 " +
                "OR OrdenesCVP.Comentarios LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 1 AND Disponible = 1 " +
                "OR E2.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 1 AND Disponible = 1 " +
                "OR E1.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 1 AND Disponible = 1; ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), 0, ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ""));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public OrdenesCVP ObtenerOrdenCVPById(int idOrdenCVP)
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT idOrdenesCVP, idEstadoProceso, idOperacion " +
                "FROM OrdenesCVP " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP() { idOrdenCVP = ReaderBD.GetInt32(0), idEstatus = ReaderBD.GetInt32(1), idOperacion = ReaderBD.GetInt32(2) };

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }

        public OrdenesCVP ObtenerOrdenCById(int idOrdenCVP)
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idProveedor, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Proveedor ON OrdenesCVP.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "' " +
                "AND OrdenesCVP.idOperacion = 1 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), 0, ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), "");

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }

        public List<OrdenesCVP> ObtenerOrdenesV()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 3 AND Disponible = 1;";
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }
        
        public List<OrdenesCVP> ObtenerOrdenesVP()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 3 AND Disponible = 1 AND ( OrdenesCVP.idEstadoProceso = 1 OR OrdenesCVP.idEstadoProceso = 2 OR OrdenesCVP.idEstadoProceso = 3);";
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerOrdenesVE()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 3 AND Disponible = 1 AND (OrdenesCVP.idEstadoProceso = 5 OR OrdenesCVP.idEstadoProceso = 3);";
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerOrdenesVPE()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 3 AND Disponible = 1 AND (OrdenesCVP.idEstadoProceso = 4 OR OrdenesCVP.idEstadoProceso = 2);";
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerBusquedaOrdenesV(string consulta)
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE Empresa.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 3 AND Disponible = 1 " +
                "OR Sucursal.Sucursal LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 3 AND Disponible = 1 " +
                "OR OrdenesCVP.Fecha LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 3 AND Disponible = 1 " +
                "OR OrdenesCVP.Comentarios LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 3 AND Disponible = 1 " +
                "OR E2.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 3 AND Disponible = 1 " +
                "OR E1.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 3 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public OrdenesCVP ObtenerOrdenVById(int idOrdenCVP)
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "' " +
                "AND OrdenesCVP.idOperacion = 3 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }

        public List<OrdenesCVP> ObtenerCotizaciones()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 2 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerCotizaciones(string estado)
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 2 AND Disponible = 1 and OrdenesCVP.idEstadoProceso=" + estado;

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerBusquedaCotizaciones(string consulta)
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE Empresa.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 2 AND Disponible = 1 " +
                "OR Sucursal.Sucursal LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 2 AND Disponible = 1 " +
                "OR OrdenesCVP.Fecha LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 2 AND Disponible = 1 " +
                "OR OrdenesCVP.Comentarios LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 2 AND Disponible = 1 " +
                "OR E1.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 2 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public OrdenesCVP ObtenerCotizacionById(int idOrdenCVP)
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "' " +
                "AND OrdenesCVP.idOperacion = 2 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }

        public int AgregarOrdenC(OrdenesCVP OrdenCVP)
        {

            string Folio = "0";
            string Query = "Select Max(Folio) From OrdenesCVP where idOperacion = 1";
            OrdenCVP.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            OrdenCVP.idEstatus = 1;
            OrdenCVP.idEmpleadoAprobar = OrdenCVP.idEmpleado;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    if (ReaderBD[0] != DBNull.Value)
                        Folio = ReaderBD.GetString(0);
                }
            Folio = (Convert.ToInt32(Folio) + 1).ToString();
            OrdenCVP.Folio = Folio.PadLeft(5, '0');


            Query = string.Format("INSERT INTO OrdenesCVP (idProveedor, idSucursal, idSerie, idMoneda, DiasCredito, Fecha, Comentarios, idEmpleado, Descuento, idOperacion, idIVA, idEstadoProceso, ValorDescuento, ValorIVA, Subtotal, Total, Folio, idEmpleadoAprobar, Disponible) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '0', '1', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', 1);",
                OrdenCVP.idProveedor, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, /*OrdenCVP.Descuento,*/
                OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.Folio, OrdenCVP.idEmpleadoAprobar);
            ReaderBD.Dispose();
            ReaderBD.Close();
            return ActualizarDatos(Query);
        }

        public int AgregarCotizacion(OrdenesCVP OrdenCVP)
        {

            string Folio = "0";
            string Query = "Select Max(Folio) From OrdenesCVP where idOperacion = 2";
            OrdenCVP.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            OrdenCVP.idEstatus = 1;
            OrdenCVP.idEmpleadoAprobar = OrdenCVP.idEmpleado;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    if (ReaderBD[0] != DBNull.Value)
                        Folio = ReaderBD.GetString(0);
                }
            Folio = (Convert.ToInt32(Folio) + 1).ToString();
            OrdenCVP.Folio = Folio.PadLeft(5, '0');


            Query = string.Format("INSERT INTO OrdenesCVP (idCliente, idSucursal, idSerie, idMoneda, DiasCredito, Fecha, Comentarios, idEmpleado, Descuento, idOperacion, idIVA, idEstadoProceso, idEmpleadoAprobar, ValorDescuento, ValorIVA, Subtotal, Total, Folio, Disponible, Dirigido) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '2', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', 1, '{17}');",
                OrdenCVP.idCliente, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, OrdenCVP.Descuento,
                OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.idEmpleadoAprobar, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.Folio, OrdenCVP.Dirigido);
            ReaderBD.Dispose();
            ReaderBD.Close();
            return ActualizarDatos(Query);
        }

        public int AgregarOrdenV(OrdenesCVP OrdenCVP)
        {

            string Folio = "0";
            string Query = "Select Max(Folio) From OrdenesCVP where idOperacion = 3";
            OrdenCVP.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            OrdenCVP.idEstatus = 1;
            OrdenCVP.idEmpleadoAprobar = OrdenCVP.idEmpleado;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    if (ReaderBD[0] != DBNull.Value)
                        Folio = ReaderBD.GetString(0);
                }
            Folio = (Convert.ToInt32(Folio) + 1).ToString();
            OrdenCVP.Folio = Folio.PadLeft(5, '0');

            Query = string.Format("INSERT INTO OrdenesCVP (idCliente, idSucursal, idSerie, idMoneda, DiasCredito, Fecha, Comentarios, idEmpleado, Descuento, idOperacion, idIVA, idEstadoProceso, idEmpleadoAprobar, ValorDescuento, ValorIVA, Subtotal, Total, Folio, Disponible, Dirigido) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '3', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', 1, '{17}');",
                OrdenCVP.idCliente, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, OrdenCVP.Descuento,
                OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.idEmpleadoAprobar, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.Folio, OrdenCVP.Dirigido);
            ReaderBD.Dispose();
            ReaderBD.Close();
            return ActualizarDatos(Query);
        }

        public void ActualizarOrdenC(OrdenesCVP OrdenCVP)
        {

            string Query = string.Format("UPDATE OrdenesCVP SET idProveedor = '{0}', idSucursal = '{1}', idSerie = '{2}', idMoneda = '{3}', DiasCredito = '{4}', Fecha = '{5}', Comentarios = '{6}', " +
                "idEmpleado = '{7}', Descuento ='{8}', idOperacion = '{9}', idIVA = '{10}', idEstadoProceso = '{11}', idEmpleadoAprobar = '{12}', ValorDescuento = '{13}', ValorIVA = '{14}', Subtotal = '{15}', Total ='{16}' WHERE idOrdenesCVP = '{17}';",
                OrdenCVP.idProveedor, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, OrdenCVP.Descuento,
                OrdenCVP.idOperacion, OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.idEmpleadoAprobar, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.idOrdenCVP);
            ActualizarDatos(Query);
        }

        public void ActualizarCotizacion(OrdenesCVP OrdenCVP)
        {

            string Query = string.Format("UPDATE OrdenesCVP SET idCliente = '{0}', idSucursal = '{1}', idSerie = '{2}', idMoneda = '{3}', DiasCredito = '{4}', Fecha = '{5}', Comentarios = '{6}', " +
                "idEmpleado = '{7}', Descuento ='{8}', idOperacion = '{9}', idIVA = '{10}', idEstadoProceso = '{11}', idEmpleadoAprobar = '{12}', ValorDescuento = '{13}', ValorIVA = '{14}', Subtotal = '{15}', Total ='{16}' WHERE idOrdenesCVP = '{17}';",
                OrdenCVP.idCliente, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, OrdenCVP.Descuento,
                OrdenCVP.idOperacion, OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.idEmpleadoAprobar, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.idOrdenCVP);
            ActualizarDatos(Query);
        }

        public void ActualizarOrdenV(OrdenesCVP OrdenCVP)
        {

            string Query = string.Format("UPDATE OrdenesCVP SET idCliente = '{0}', idSucursal = '{1}', idSerie = '{2}', idMoneda = '{3}', DiasCredito = '{4}', Fecha = '{5}', Comentarios = '{6}', " +
                "idEmpleado = '{7}', Descuento ='{8}', idOperacion = '{9}', idIVA = '{10}', idEstadoProceso = '{11}', idEmpleadoAprobar = '{12}', ValorDescuento = '{13}', ValorIVA = '{14}', Subtotal = '{15}', Total ='{16}' WHERE idOrdenesCVP = '{17}';",
                OrdenCVP.idCliente, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, OrdenCVP.Descuento,
                OrdenCVP.idOperacion, OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.idEmpleadoAprobar, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.idOrdenCVP);
            ActualizarDatos(Query);
        }

        public void EliminarOrdenCVP(int idOrdenCVP)
        {

            string Query = string.Format("UPDATE OrdenesCVP SET Disponible = 0 WHERE idOrdenesCVP = '{0}';", idOrdenCVP); 
            ActualizarDatos(Query);
        }

        public void AprobarOrdenCVP(int idOrdenCVP, int idEmpleadoAprobar)
        {

            string Query = string.Format("UPDATE OrdenesCVP SET idEstadoProceso = 2, idEmpleadoAprobar = '{0}' WHERE idOrdenesCVP = '{1}';", idEmpleadoAprobar, idOrdenCVP);
            ActualizarDatos(Query);
        }

        public void PagarOrdenCVP(int idOrdenCVP)
        {

            OrdenesCVP OrdenCVP = ObtenerOrdenCVPById(idOrdenCVP);
            string Query = "";
            if (OrdenCVP.idEstatus == 3)
                Query = string.Format("UPDATE OrdenesCVP SET idEstadoProceso = 5 WHERE idOrdenesCVP = '{0}';", idOrdenCVP);
            if (OrdenCVP.idEstatus == 2)
                Query = string.Format("UPDATE OrdenesCVP SET idEstadoProceso = 4 WHERE idOrdenesCVP = '{0}';", idOrdenCVP);
            ActualizarDatos(Query);
        }

        public int ActualizarSaldoCuenta(int idOrdenCVP, int idCuenta)
        {
            OrdenesCVP OrdenCompra = ObtenerOrdenCVPById(idOrdenCVP).idOperacion == 1 ? ObtenerOrdenCById(idOrdenCVP) : ObtenerOrdenVById(idOrdenCVP);
            Cuenta CuentaPagar = ObtenerCuenta(idCuenta);
            if (OrdenCompra.idOperacion == 1)
                CuentaPagar.Saldo -= OrdenCompra.Total;
            else
                CuentaPagar.Saldo += OrdenCompra.Total;
            ActualizarCuenta(CuentaPagar);

            DateTime Fecha = DateTime.Now;
            string Query = string.Format("INSERT INTO MovimientosCuenta (idOrdenCVP, idCuenta, Cantidad, Fecha) VALUES ('{0}', '{1}', '{2}', '{3}') ;", idOrdenCVP, idCuenta, OrdenCompra.Total, Fecha.ToString("yyyy-MM-dd"));
            return ActualizarDatos(Query);
        }

        public int CotizacionToVenta(int idOrdenCVP)
        {

            OrdenesCVP OrdenCotizacion = ObtenerCotizacionById(idOrdenCVP);
            int idOrdenVenta = AgregarOrdenV(OrdenCotizacion);
            OrdenesCVP OrdenVenta = ObtenerOrdenVById(idOrdenVenta);
            List<ListaProductos> ProductosCotizacion = new List<ListaProductos>();
            ProductosCotizacion = ObtenerProductosByOrden(idOrdenCVP);
            foreach (ListaProductos ListaVenta in ProductosCotizacion)
            {
                ListaVenta.idOrdenCVP = idOrdenVenta;
                AgregarListaProducto(ListaVenta);
            }
            OrdenCotizacion.idEstatus = 2;
            ActualizarCotizacion(OrdenCotizacion);
            LlenarFactura(OrdenVenta);
            Cerrar_Conexion();
            return idOrdenVenta;
        }

        public OrdenesCVP ObtenerOrdenCompraCompletaById(int idOrdenCVP)//ASR 27-11-2018
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idProveedor, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Proveedor ON OrdenesCVP.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "' " +
                "AND OrdenesCVP.idOperacion = 1 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), 0, ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), "", ObtenerProductosByOrden(ReaderBD.GetInt32(0)));
            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }//ASR 27-11-2018

        public OrdenesCVP ObtenerOrdenVCompletaById(int idOrdenCVP)//ASR 27-11-2018
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "' " +
                "AND OrdenesCVP.idOperacion = 3 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29), ObtenerProductosByOrdenEditar(ReaderBD.GetInt32(0)));
            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }//ASR 27-11-2018

        public OrdenesCVP ObtenerCotizacionCompletaById(int idOrdenCVP)//ASR 27-11-2018
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio, OrdenesCVP.Dirigido " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "' " +
                "AND OrdenesCVP.idOperacion = 2 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ReaderBD.GetString(29), ObtenerProductosByOrden(ReaderBD.GetInt32(0)));
            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }//ASR 27-11-2018

        public List<ListaProductos> ObtenerProductosByOrdenEditar(int OrdenCVP)//ASR 27-11-2018
        {
            List<ListaProductos> ListaProductos = new List<ListaProductos>();

            string Query = "SELECT idListaProductos, LP.idProducto, Producto.Producto, LP.idPresentacion, LP.Cantidad, CostoPrecio, ImporteTotal, IEPS, " +
                "LP.idOrdenCompraV, Producto.Codigo, Presentaciones.Presentacion, Producto.Color, Producto.PiezasTotales FROM ListaProductos LP " +
                "INNER JOIN Producto ON LP.idProducto = Producto.idProducto " +
                "INNER JOIN Presentaciones ON LP.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE idOrdenCompraV = '" + OrdenCVP + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaProductos.Add(new ListaProductos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetString(9), ReaderBD.GetInt32(3), ReaderBD.GetString(10),
                        ReaderBD.GetInt32(4), ReaderBD.GetDouble(5), ReaderBD.GetDouble(6), ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), 0, ReaderBD.GetString(11), ReaderBD.GetInt32(12)));
            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaProductos;
        }//ASR 27-11-2018
        #endregion

        #region Cotizacion X Pieza
        public List<OrdenesCVP> ObtenerCotizacionesXpz()
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE OrdenesCVP.idOperacion = 4 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ""));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public List<OrdenesCVP> ObtenerBusquedaCotizacionesXpz(string consulta)
        {
            List<OrdenesCVP> ListaOrdenesCVP = new List<OrdenesCVP>();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE Empresa.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 4 AND Disponible = 1 " +
                "OR Sucursal.Sucursal LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 4 AND Disponible = 1 " +
                "OR OrdenesCVP.Fecha LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 4 AND Disponible = 1 " +
                "OR OrdenesCVP.Comentarios LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 4 AND Disponible = 1 " +
                "OR E1.Nombre LIKE '%" + consulta + "%' AND OrdenesCVP.idOperacion = 4 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaOrdenesCVP.Add(new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), ""));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaOrdenesCVP;
        }

        public OrdenesCVP ObtenerCotizacionByIdXpz(int idOrdenCVP)
        {
            OrdenesCVP OrdenCVP = new OrdenesCVP();

            string Query = "SELECT OrdenesCVP.idOrdenesCVP, " +
                "OrdenesCVP.idCliente, Empresa.Nombre, OrdenesCVP.idSucursal, Sucursal.Sucursal, OrdenesCVP.idSerie, Serie.Prefijo, Serie.NumeroSerie, OrdenesCVP.idMoneda, Moneda.Moneda, " +
                "OrdenesCVP.DiasCredito, OrdenesCVP.Fecha, OrdenesCVP.Comentarios, OrdenesCVP.idEmpleado, E1.Nombre, OrdenesCVP.Descuento, OrdenesCVP.idOperacion, Operaciones.Operacion, " +
                "OrdenesCVP.idIVA, IVA.IVA, OrdenesCVP.idEstadoProceso, EstadoProceso.EstadoProceso, OrdenesCVP.idEmpleadoAprobar, E2.Nombre AS NombreAprobar, OrdenesCVP.ValorDescuento, OrdenesCVP.ValorIVA, OrdenesCVP.Subtotal, OrdenesCVP.Total, OrdenesCVP.Folio " +
                "FROM OrdenesCVP " +
                "INNER JOIN Cliente ON OrdenesCVP.idCliente = Cliente.idCliente " +
                "INNER JOIN Empresa ON Cliente.idEmpresa = Empresa.idEmpresa " +
                "INNER JOIN Sucursal ON OrdenesCVP.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Serie ON OrdenesCVP.idSerie = Serie.idSerie " +
                "INNER JOIN Moneda ON OrdenesCVP.idMoneda = Moneda.idMoneda " +
                "INNER JOIN Empleado E1 ON OrdenesCVP.idEmpleado = E1.idEmpleado " +
                "INNER JOIN Operaciones ON OrdenesCVP.idOperacion = Operaciones.idOperaciones " +
                "INNER JOIN IVA ON OrdenesCVP.idIVA = IVA.idIVA " +
                "INNER JOIN Empleado E2 ON OrdenesCVP.idEmpleadoAprobar = E2.idEmpleado " +
                "INNER JOIN EstadoProceso ON OrdenesCVP.idEstadoProceso = EstadoProceso.idEstadoProceso " +
                "WHERE idOrdenesCVP = '" + idOrdenCVP + "' " +
                "AND OrdenesCVP.idOperacion = 4 AND Disponible = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    OrdenCVP = new OrdenesCVP(ReaderBD.GetInt32(0), 0, ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6),
                        ReaderBD.GetDouble(7), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetDateTime(11), ReaderBD.GetString(12), ReaderBD.GetInt32(13), ReaderBD.GetString(14),
                        ReaderBD.GetDouble(15), ReaderBD.GetInt32(16), ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetDouble(19), ReaderBD.GetInt32(20), ReaderBD.GetString(21), ReaderBD["idEmpleadoAprobar"] == DBNull.Value ? 0 : ReaderBD.GetInt32(22), ReaderBD["NombreAprobar"] == DBNull.Value ? " " : ReaderBD.GetString(23),
                        ReaderBD.GetDouble(24), ReaderBD.GetDouble(25), ReaderBD.GetDouble(26), ReaderBD.GetDouble(27), ReaderBD.GetString(28), "");

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return OrdenCVP;
        }

        public int AgregarCotizacionXpz(OrdenesCVP OrdenCVP)
        {

            string Folio = "0";
            string Query = "Select Max(Folio) From OrdenesCVP where idOperacion = 4";
            OrdenCVP.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            OrdenCVP.idEstatus = 5;
            OrdenCVP.idEmpleadoAprobar = OrdenCVP.idEmpleado;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    if (ReaderBD[0] != DBNull.Value)
                        Folio = ReaderBD.GetString(0);
                }
            Folio = (Convert.ToInt32(Folio) + 1).ToString();
            OrdenCVP.Folio = Folio.PadLeft(5, '0');


            Query = string.Format("INSERT INTO OrdenesCVP (idCliente, idSucursal, idSerie, idMoneda, DiasCredito, Fecha, Comentarios, idEmpleado, Descuento, idOperacion, idIVA, idEstadoProceso, idEmpleadoAprobar, ValorDescuento, ValorIVA, Subtotal, Total, Folio, Disponible) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '4', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', 1);",
                OrdenCVP.idCliente, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, OrdenCVP.Descuento,
                OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.idEmpleadoAprobar, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.Folio);
            ReaderBD.Dispose();
            ReaderBD.Close();
            return ActualizarDatos(Query);
        }

        public void ActualizarCotizacionXpz(OrdenesCVP OrdenCVP)
        {

            string Query = string.Format("UPDATE OrdenesCVP SET idCliente = '{0}', idSucursal = '{1}', idSerie = '{2}', idMoneda = '{3}', DiasCredito = '{4}', Fecha = '{5}', Comentarios = '{6}', " +
                "idEmpleado = '{7}', Descuento ='{8}', idOperacion = '{9}', idIVA = '{10}', idEstadoProceso = '{11}', idEmpleadoAprobar = '{12}', ValorDescuento = '{13}', ValorIVA = '{14}', Subtotal = '{15}', Total ='{16}' WHERE idOrdenesCVP = '{17}';",
                OrdenCVP.idCliente, OrdenCVP.idSucursal, OrdenCVP.idSerie, OrdenCVP.idMoneda, OrdenCVP.DiasCredito, OrdenCVP.Fecha, OrdenCVP.Comentarios, OrdenCVP.idEmpleado, OrdenCVP.Descuento,
                OrdenCVP.idOperacion, OrdenCVP.idIVA, OrdenCVP.idEstatus, OrdenCVP.idEmpleadoAprobar, OrdenCVP.ValorDescuento, OrdenCVP.ValorIVA, OrdenCVP.Subtotal, OrdenCVP.Total, OrdenCVP.idOrdenCVP);
            ActualizarDatos(Query);
        }
        
        #endregion

        /**
         * Transferencias Sucursales
         **/
        #region Transferencias Sucursales
        public List<TransferSuc> ObtenerTransferencias()
        {
            List<TransferSuc> ListaTransferencias = new List<TransferSuc>();

            string Query = "SELECT TS.idTransferenciaSuc, TS.idSucTx, TS.idSucRx, SucTx.Sucursal, " +
                "SucRx.Sucursal,TS.idEmpleado, E.Nombre, TS.Comentarios, TS.Fecha " +
                "FROM TransferenciaSuc TS " +
                "INNER JOIN Sucursal SucTx ON TS.idSucTx = SucTx.idSucursal " +
                "INNER JOIN Sucursal SucRx ON TS.idSucRx = SucRx.idSucursal " +
                "INNER JOIN Empleado E ON TS.idEmpleado = E.idEmpleado;";


            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaTransferencias.Add(new TransferSuc(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8)));


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaTransferencias;
        }

        public List<TransferSuc> ObtenerBusquedaTransferencias(string consulta)
        {
            List<TransferSuc> ListaTransferencias = new List<TransferSuc>();

            string Query = "SELECT TS.idTransferenciaSuc, TS.idSucTx, TS.idSucRx, SucTx.Sucursal, " +
                "SucRx.Sucursal,TS.idEmpleado, E.Nombre, TS.Comentarios, TS.Fecha " +
                "FROM TransferenciaSuc TS " +
                "INNER JOIN Sucursal SucTx ON TS.idSucTx = SucTx.idSucursal " +
                "INNER JOIN Sucursal SucRx ON TS.idSucRx = SucRx.idSucursal " +
                "INNER JOIN Empleado E ON TS.idEmpleado = E.idEmpleado " +
                "WHERE SucTx.Sucursal LIKE '%" + consulta + "%' " +
                "OR SucRx.Sucursal LIKE '%" + consulta + "%' " +
                "OR E.Nombre LIKE '%" + consulta + "%' " +
                "OR TS.Comentarios LIKE '%" + consulta + "%';";


            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaTransferencias.Add(new TransferSuc(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8)));


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaTransferencias;
        }

        public TransferSuc ObtenerTransferenciaById(int idTransferenciaSuc)
        {
            TransferSuc Transferencia = new TransferSuc();

            string Query = "SELECT TS.idTransferenciaSuc, TS.idSucTx, TS.idSucRx, SucTx.Sucursal, " +
                "SucRx.Sucursal,TS.idEmpleado, E.Nombre, TS.Comentarios, TS.Fecha " +
                "FROM TransferenciaSuc TS " +
                "INNER JOIN Sucursal SucTx ON TS.idSucTx = SucTx.idSucursal " +
                "INNER JOIN Sucursal SucRx ON TS.idSucRx = SucRx.idSucursal " +
                "INNER JOIN Empleado E ON TS.idEmpleado = E.idEmpleado " +
                "WHERE idTransferenciaSuc = '" + idTransferenciaSuc + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Transferencia = new TransferSuc(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5),
                        ReaderBD.GetString(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Transferencia;
        }

        public int AgregarTransfer(TransferSuc Transferencia)
        {
            Transferencia.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            string Query = string.Format("INSERT INTO TransferenciaSuc (idSucTx, idSucRx, idEmpleado, Comentarios, Fecha) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');",
                Transferencia.idSucTx, Transferencia.idSucRx, Transferencia.idEmpleado, Transferencia.Comentarios, Transferencia.Fecha);
            return ActualizarDatos(Query);
        }

        public void ActualizarTransfer(TransferSuc Transferencia)
        {

            string Query = string.Format("UPDATE TransferenciaSuc SET idSucTx = '{0}', idSucRx = '{1}', idEmpleado = '{2}', Comentarios = '{3}' WHERE idTransferenciaSuc = '{4}';",
                Transferencia.idSucTx, Transferencia.idSucRx, Transferencia.idEmpleado, Transferencia.Comentarios, Transferencia.idTransferencia);
            ActualizarDatos(Query);
        }

        public void EliminarTransfer(int idTransferencia)
        {

            string Query = string.Format("DELETE FROM TransferenciaSuc WHERE idTransferenciaSuc = '{0}';", idTransferencia);
            ActualizarDatos(Query);
        }
        #endregion

        #region Costos
        public List<Costos> ObtenerCostos()
        {
            List<Costos> ListaCostos = new List<Costos>();

            string Query = "SELECT Costos.idCostos, Costos.idProducto, Producto.Producto, Costos.idProveedor, Empresa.Nombre, Costos.Costo " +
                "FROM Costos " +
                "INNER JOIN Producto ON Costos.idProducto = Producto.idProducto " +
                "INNER JOIN Proveedor ON Costos.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa;";


            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaCostos.Add(new Costos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetDouble(5)));


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaCostos;
        }

        public Costos ObtenerCostoById(int idCosto)
        {
            Costos Costo = new Costos();

            string Query = "SELECT Costos.idCostos, Costos.idProducto, Producto.Producto, Costos.idProveedor, Empresa.Nombre, Costos.Costo " +
                "FROM Costos " +
                "INNER JOIN Producto ON Costos.idProducto = Producto.idProducto " +
                "INNER JOIN Proveedor ON Costos.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "WHERE idCostos = '" + idCosto + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Costo = new Costos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetDouble(5));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Costo;
        }

        public Costos ObtenerCostoByIdProducto(int idProducto)
        {
            Costos Costo = new Costos();

            string Query = "SELECT Costos.idCostos, Costos.idProducto, Producto.Producto, Costos.idProveedor, Empresa.Nombre, Costos.Costo " +
                "FROM Costos " +
                "INNER JOIN Producto ON Costos.idProducto = Producto.idProducto " +
                "INNER JOIN Proveedor ON Costos.idProveedor = Proveedor.idProveedor " +
                "INNER JOIN Empresa ON Proveedor.idEmpresa = Empresa.idEmpresa " +
                "WHERE  Costos.idProducto = '" + idProducto + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Costo = new Costos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetDouble(5));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Costo;
        }

        public int AgregarCosto(Costos Costo)
        {


            string Query = string.Format("INSERT INTO Costos (idProducto, idProveedor, Costo) VALUES ('{0}', '{1}', '{2}');",
                Costo.idProducto, Costo.idProveedor, Costo.Costo);
            return ActualizarDatos(Query);
        }



        public void ActualizarCosto(Costos Costo)
        {


            string Query = "SELECT idCostos FROM Costos WHERE idProducto = " + Costo.idProducto;

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (!ReaderBD.Read())
            {
                Query = string.Format("INSERT INTO Costos (idProducto, idProveedor, Costo) VALUES ('{0}', '{1}', '{2}');",
                    Costo.idProducto, Costo.idProveedor, Costo.Costo);
                ActualizarDatos(Query);
            }
            else
            {
                Query = string.Format("UPDATE Costos SET idProducto = '{0}', idProveedor = '{1}', Costo = '{2}' WHERE idCostos = '{3}';",
                    Costo.idProducto, Costo.idProveedor, Costo.Costo, ReaderBD[0].ToString());
                ActualizarDatos(Query);
            }
            ReaderBD.Close();
            ReaderBD.Dispose();
            Cerrar_Conexion();
        }

        public void EliminarCosto(int idCosto)
        {

            string Query = string.Format("DELETE FROM Costos WHERE idCostos = '{0}';", idCosto);
            ActualizarDatos(Query);
        }
        #endregion

        #region Cuentas
        public List<Cuenta> ObtenerCuentas()
        {

            List<Cuenta> lista = new List<Cuenta>();

            string query = "SELECT cuentas.idCuenta, cuentas.NumeroCuenta, cuentas.Saldo, cuentas.idBanco,  banco.Banco as Banco, cuentas.idTipoCuenta, tipocuenta.TipoCuenta as TipoCuenta FROM cuentas inner join banco on cuentas.idBanco = banco.idBanco inner join tipocuenta on cuentas.idTipoCuenta = tipocuenta.idTipoCuenta";

            MySqlDataReader ReaderBD = ObtenerDatosBD(query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    lista.Add(new Cuenta(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(4), ReaderBD.GetInt32(3), ReaderBD.GetString(6), ReaderBD.GetInt32(5), ReaderBD.GetDouble(2)));




            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return lista;


        }

        public List<Cuenta> ObtenerBusquedaCuentas(string consulta)
        {

            List<Cuenta> lista = new List<Cuenta>();

            string query = "SELECT cuentas.idCuenta, cuentas.NumeroCuenta, cuentas.Saldo, cuentas.idBanco,  banco.Banco as Banco, cuentas.idTipoCuenta, tipocuenta.TipoCuenta as TipoCuenta " +
                "FROM cuentas inner join banco on cuentas.idBanco = banco.idBanco inner join tipocuenta on cuentas.idTipoCuenta = tipocuenta.idTipoCuenta " +
                "WHERE cuentas.NumeroCuenta LIKE '%" + consulta + "%' " +
                "OR Banco LIKE '%" + consulta + "%' " +
                "OR cuentas.Saldo LIKE '%" + consulta + "%'; ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    lista.Add(new Cuenta(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(4), ReaderBD.GetInt32(3), ReaderBD.GetString(6), ReaderBD.GetInt32(5), ReaderBD.GetDouble(2)));




            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return lista;
        }

        public Cuenta ObtenerCuenta(int idCuenta)
        {
            Cuenta cuenta = null;

            string query = String.Format("SELECT cuentas.idCuenta, cuentas.NumeroCuenta, cuentas.Saldo, cuentas.idBanco,  banco.Banco as Banco, cuentas.idTipoCuenta, tipocuenta.TipoCuenta as TipoCuenta FROM cuentas inner join banco on cuentas.idBanco = banco.idBanco inner join tipocuenta on cuentas.idTipoCuenta = tipocuenta.idTipoCuenta where idCuenta = {0}", idCuenta);

            MySqlDataReader ReaderBD = ObtenerDatosBD(query);
            if (ReaderBD != null)
            {
                if (ReaderBD.Read())
                    cuenta = new Cuenta(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(4), ReaderBD.GetInt32(3), ReaderBD.GetString(6), ReaderBD.GetInt32(5), ReaderBD.GetDouble(2));
            }


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return cuenta;
        }

        public Cuenta ObtenerCuenta(string numeroCuenta, int idBanco)
        {
            Cuenta cuenta = null;

            string query = String.Format("SELECT cuentas.idCuenta, cuentas.NumeroCuenta, cuentas.Saldo, cuentas.idBanco,  banco.Banco as Banco, cuentas.idTipoCuenta, tipocuenta.TipoCuenta as TipoCuenta FROM cuentas inner join banco on cuentas.idBanco = banco.idBanco inner join tipocuenta on cuentas.idTipoCuenta = tipocuenta.idTipoCuenta where NumeroCuenta = '{0}' and idBanco = {1}", numeroCuenta, idBanco);

            MySqlDataReader ReaderBD = ObtenerDatosBD(query);
            if (ReaderBD != null)
                if (ReaderBD.Read())
                    cuenta = new Cuenta(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(4), ReaderBD.GetInt32(3), ReaderBD.GetString(6), ReaderBD.GetInt32(5), ReaderBD.GetDouble(2));



            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return cuenta;
        }

        public int AgregarCuenta(Cuenta cuenta)
        {
            int ret = -1;
            if (cuenta != null)
            {


                string query = String.Format("INSERT INTO cuentas (NumeroCuenta, idBanco, Saldo, idTipoCuenta) VALUES ('{0}', {1}, {2}, {3})", cuenta.NumeroCuenta, cuenta.idBanco, cuenta.Saldo, cuenta.IdTipoCuenta);
                ret = ActualizarDatos(query);



            }
            return ret;
        }

        public void ActualizarCuenta(Cuenta cuenta)
        {
            if (cuenta != null)
            {



                string query = string.Format("UPDATE cuentas SET NumeroCuenta = '{0}', idBanco = {1}, Saldo = {2}, idTipoCuenta = {3} WHERE idCuenta = {4}", cuenta.NumeroCuenta, cuenta.idBanco, cuenta.Saldo, cuenta.IdTipoCuenta, cuenta.idCuenta);

                ActualizarDatos(query);



            }
        }

        public void ActualizarCuenta(int idCuenta, double Saldo)
        {
            if (idCuenta > 0)
            {



                string query = string.Format("UPDATE cuentas SET Saldo = {0} WHERE idCuenta = {1}", Saldo, idCuenta);

                ActualizarDatos(query);



            }
        }

        public double DepositarCuenta(int idCuenta, double deposito)
        {
            double saldo = -1;
            if (idCuenta > 0)
            {



                string query = string.Format("SELECT Saldo From cuentas WHERE  idCuenta = {0}", idCuenta);
                MySqlDataReader ReaderBD = ObtenerDatosBD(query);
                if (ReaderBD != null)
                    if (ReaderBD.Read())
                        saldo = ReaderBD.GetDouble(0);

                if (saldo >= 0)
                {
                    saldo += deposito;
                    query = string.Format("UPDATE cuentas SET Saldo = {0} WHERE idCuenta = {1}", saldo, idCuenta);

                    ActualizarDatos(query);
                }
                ReaderBD.Dispose();
                ReaderBD.Close();


            }
            return saldo;
        }
        public double RetirarCuenta(int idCuenta, double retiro)
        {
            double saldo = -1;
            if (idCuenta > 0)
            {



                string query = string.Format("SELECT Saldo From cuentas WHERE  idCuenta = {0}", idCuenta);
                MySqlDataReader ReaderBD = ObtenerDatosBD(query);
                if (ReaderBD != null)
                    if (ReaderBD.Read())
                        saldo = ReaderBD.GetDouble(0);

                if (saldo >= 0)
                {
                    saldo -= retiro;
                    query = string.Format("UPDATE cuentas SET Saldo = {0} WHERE idCuenta = {1}", saldo, idCuenta);

                    ActualizarDatos(query);
                }

                ReaderBD.Dispose();
                ReaderBD.Close();

            }
            return saldo;
        }

        public void EliminarCuenta(int idCuenta)
        {
            if (idCuenta > 0)
            {



                string query = string.Format("DELETE FROM cuentas  WHERE idCuenta = {0}", idCuenta);

                ActualizarDatos(query);



            }
        }


        #endregion

        #region Tipo de Cuentas
        public List<TipoCuenta> ObtenerTipoCuentas()
        {

            List<TipoCuenta> lista = new List<TipoCuenta>();

            string query = "SELECT idTipoCuenta, TipoCuenta FROM TipoCuenta;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    lista.Add(new TipoCuenta(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return lista;


        }

        public TipoCuenta ObtenerTipoCuenta(int idCuenta)
        {
            TipoCuenta cuenta = null;

            string query = String.Format("SELECT idTipoCuenta, TipoCuenta FROM TipoCuenta WHERE idTipoCuenta = {0}", idCuenta);

            MySqlDataReader ReaderBD = ObtenerDatosBD(query);
            if (ReaderBD != null)
            {
                if (ReaderBD.Read())
                    cuenta = new TipoCuenta(ReaderBD.GetInt32(0), ReaderBD.GetString(1));
            }


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return cuenta;
        }

        public int AgregarTipoCuenta(TipoCuenta cuenta)
        {
            int ret = -1;
            if (cuenta != null)
            {

                string query = String.Format("INSERT INTO TipoCuenta (TipoCuenta) VALUES ('{0}')", cuenta.TipodeCuenta);
                ret = ActualizarDatos(query);



            }
            return ret;
        }
        public void ActualizarTipoCuenta(TipoCuenta cuenta)
        {
            if (cuenta != null)
            {



                string query = string.Format("UPDATE TipoCuenta SET TipoCuenta = '{0}' WHERE idTipoCuenta = {1}", cuenta.TipodeCuenta, cuenta.idTipoCuenta);

                ActualizarDatos(query);



            }
        }

        public void EliminarTipoCuenta(int idCuenta)
        {
            if (idCuenta > 0)
            {



                string query = string.Format("DELETE FROM TipoCuenta  WHERE idTipoCuenta = {0}", idCuenta);

                ActualizarDatos(query);



            }
        }


        #endregion

        #region PreciosHistoricos

        public List<PreciosHistoricos> ObtenerPreciosH()
        {
            List<PreciosHistoricos> ListaPrecios = new List<PreciosHistoricos>();

            string Query = "SELECT idprecioshistoricos, precioshistoricos.idproducto, producto.Producto, precio,  precioshistoricos.idsucursal,  "
                + "sucursal.Sucursal, fecha FROM Precioshistoricos INNER JOIN Producto ON Precioshistoricos.idProducto = Producto.idProducto INNER JOIN Sucursal "
                + "ON Precioshistoricos.idSucursal = Sucursal.idSucursal";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new PreciosHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }

        public List<PreciosHistoricos> ObtenerPreciosH(int idProducto)
        {
            List<PreciosHistoricos> ListaPrecios = new List<PreciosHistoricos>();

            string Query = "SELECT idprecioshistoricos, precioshistoricos.idproducto, producto.Producto, precio,  precioshistoricos.idsucursal,  "
                + "sucursal.Sucursal, fecha FROM Precioshistoricos INNER JOIN Producto ON Precioshistoricos.idProducto = Producto.idProducto INNER JOIN Sucursal "
                + "ON Precioshistoricos.idSucursal = Sucursal.idSucursal WHERE precioshistoricos.idProducto = " + idProducto + "ORDER BY Fecha";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new PreciosHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }


        public PreciosHistoricos ObtenerPrecioH(int idPrecios)
        {
            PreciosHistoricos Proveedor = new PreciosHistoricos();

            string Query = "SELECT idprecioshistoricos, precioshistoricos.idproducto, producto.Producto, precio,  precioshistoricos.idsucursal,  "
                + "sucursal.Sucursal, fecha FROM Precioshistoricos INNER JOIN Producto ON Precioshistoricos.idProducto = Producto.idProducto INNER JOIN Sucursal "
                + "ON Precioshistoricos.idSucursal = Sucursal.idSucursal WHERE precioshistoricos.idPrecioshistoricos = " + idPrecios;

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Proveedor = new PreciosHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Proveedor;
        }


        public int AgregarPreciosH(PreciosHistoricos Precios)
        {

            string Query = string.Format("INSERT INTO precioshistoricos (precio, fecha, idProducto, idSucursal) VALUES ({0}, '{1}', {2}, {3})",
                Precios.Precio, Precios.Fecha, Precios.idProducto, Precios.idSucursal);
            return ActualizarDatos(Query);
        }

        public void ActualizarPreciosH(PreciosHistoricos Precios)
        {

            string Query = string.Format("UPDATE precioshistoricos SET precio = {0}, fecha = '{1}', idProducto = {2}, idSucursal = {3} WHERE idprecioshistoricos = {4}", Precios.Precio, Precios.Fecha, Precios.idProducto, Precios.idSucursal, Precios.idPrecio);
            ActualizarDatos(Query);
        }

        public void EliminarPreciosH(int idPrecio)
        {

            string Query = string.Format("DELETE FROM PreciosHistoricos WHERE idPreciosHistoricos = '{0}';", idPrecio);
            ActualizarDatos(Query);
        }
        #endregion

        #region CostosHistoricos
        public List<CostosHistoricos> ObtenerCostosH()
        {
            List<CostosHistoricos> ListaPrecios = new List<CostosHistoricos>();

            string Query = "SELECT idcostoshistoricos, costoshistoricos.idproducto, producto.Producto, costo,  costoshistoricos.idproveedor, empresa.Nombre as Empresa, fecha "
                + "FROM costoshistoricos INNER JOIN Producto ON costoshistoricos.idProducto = Producto.idProducto INNER JOIN Proveedor ON costoshistoricos.idProveedor = proveedor.idProveedor "
                + "INNER JOIN Empresa ON proveedor.idEmpresa = empresa.idEmpresa";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new CostosHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }

        public string GetString(MySqlDataReader reader, string colName)
        {
            if (reader[colName] == DBNull.Value)
                return string.Empty;
            else
                return (string)reader[colName];
        }

        public List<CostosHistoricos> ObtenerCostosH(int idProducto)
        {
            List<CostosHistoricos> ListaPrecios = new List<CostosHistoricos>();

            string Query = "SELECT idcostoshistoricos, producto.idproducto, producto.Producto, costo,  costoshistoricos.idproveedor, empresa.Nombre as Empresa, fecha "
                + "FROM costoshistoricos INNER JOIN Producto ON costoshistoricos.idProducto = Producto.idProducto INNER JOIN Proveedor ON costoshistoricos.idProveedor = proveedor.idProveedor "
                + "INNER JOIN Empresa ON proveedor.idEmpresa = empresa.idEmpresa WHERE costoshistoricos.idProducto = " + idProducto + "ORDER BY Fecha";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios.Add(new CostosHistoricos(
                        ReaderBD["idcostoshistoricos"] == DBNull.Value ? 0 : (int)ReaderBD["idcostoshistoricos"],
                        ReaderBD["idproducto"] == DBNull.Value ? 0 : (int)ReaderBD["idproducto"],
                        ReaderBD["Producto"] == DBNull.Value ? " " : (string)ReaderBD["Producto"],
                        ReaderBD["costo"] == DBNull.Value ? 0 : (double)ReaderBD["costo"],
                        ReaderBD["idproveedor"] == DBNull.Value ? 0 : (int)ReaderBD["idproveedor"],
                        ReaderBD["Nombre"] == DBNull.Value ? " " : (string)ReaderBD["Nombre"],
                        ReaderBD["fecha"] == DBNull.Value ? new DateTime(2018, 01, 01) : ReaderBD.GetDateTime(6)
                        ));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }

        public CostosHistoricos ObtenerCostosHbyProduct(int idProducto)
        {
            CostosHistoricos ListaPrecios = new CostosHistoricos();

            //BRS
            string Query = "SELECT idcostoshistoricos, producto.idproducto, producto.Producto, costo,  costoshistoricos.idproveedor, empresa.Nombre as Empresa, fecha "
                + "FROM costoshistoricos INNER JOIN Producto ON costoshistoricos.idProducto = Producto.idProducto INNER JOIN Proveedor ON costoshistoricos.idProveedor = proveedor.idProveedor "
                + "INNER JOIN Empresa ON proveedor.idEmpresa = empresa.idEmpresa WHERE idcostoshistoricos in (Select MAX(idcostoshistoricos) from costoshistoricos WHERE idProducto = " + idProducto + ")";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    ListaPrecios = new CostosHistoricos(
                        ReaderBD["idcostoshistoricos"] == DBNull.Value ? 0 : (int)ReaderBD["idcostoshistoricos"],
                        ReaderBD["idproducto"] == DBNull.Value ? 0 : (int)ReaderBD["idproducto"],
                        ReaderBD["Producto"] == DBNull.Value ? " " : (string)ReaderBD["Producto"],
                        ReaderBD["costo"] == DBNull.Value ? 0 : (double)ReaderBD["costo"],
                        ReaderBD["idproveedor"] == DBNull.Value ? 0 : (int)ReaderBD["idproveedor"],
                        ReaderBD["Empresa"] == DBNull.Value ? "Proveedor" : (string)ReaderBD["Empresa"],
                        ReaderBD["fecha"] == DBNull.Value ? new DateTime(2018, 01, 01) : ReaderBD.GetDateTime(6)
                        );

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return ListaPrecios;
        }



        public CostosHistoricos ObtenerCostoH(int idCostos)
        {
            CostosHistoricos Proveedor = new CostosHistoricos();

            string Query = "SELECT idcostoshistoricos, costoshistoricos.idproducto, producto.Producto, costo,  costoshistoricos.idproveedor, empresa.Nombre as Empresa, fecha "
                + "FROM costoshistoricos INNER JOIN Producto ON costoshistoricos.idProducto = Producto.idProducto INNER JOIN Proveedor ON costoshistoricos.idProveedor = proveedor.idProveedor "
                + "INNER JOIN Empresa ON proveedor.idEmpresa = empresa.idEmpresa WHERE costoshistoricos.idCosto = " + idCostos;

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Proveedor = new CostosHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Proveedor;
        }


        public int AgregarCostosH(CostosHistoricos Costos)
        {

            string Query = string.Format("INSERT INTO costoshistoricos (costo, fecha, idProducto, idProveedor) VALUES ({0}, '{1}', {2}, {3})",
                Costos.Costo, Costos.Fecha, Costos.idProducto, Costos.idProveedor);
            return ActualizarDatos(Query);
        }

        public void ActualizarCostosH(CostosHistoricos Costos)
        {

            string Query = string.Format("UPDATE costoshistoricos SET costo = {0}, fecha = '{1}', idProducto = {2}, idproveedor = {3} WHERE idcostoshistoricos = {4}", Costos.Costo, Costos.Fecha, Costos.idProducto, Costos.idProveedor, Costos.idCosto);
            ActualizarDatos(Query);
        }

        public void EliminarCostosH(int idCosto)
        {

            string Query = string.Format("DELETE FROM CostosHistoricos WHERE idCostosHistoricos = '{0}';", idCosto);
            ActualizarDatos(Query);
        }

        #endregion

        #region Facturas

        public void LlenarFactura(OrdenesCVP OrdenV)
        {
            try
            {
                OrdenesCVP OrdenFactura = ObtenerOrdenVById(OrdenV.idOrdenCVP);
                List<ListaProductos> ListaFactura = ObtenerProductosByOrden(OrdenFactura.idOrdenCVP);
                Cliente Cliente = ObtenerClienteById(OrdenFactura.idCliente);
                Empresa Empresa = ObtenerEmpresaById(Cliente.idEmpresa);
                Sucursal Sucursal = ObtenerSucursalById(OrdenFactura.idSucursal);
                string Archivo = OrdenFactura.Prefijo + OrdenFactura.Folio;
                string pdfTemplate = @"PlantillaPDF2.pdf";
                string newFile = Archivo + ".pdf";

                Directory.CreateDirectory("Ventas");

                Factura Factura = new Factura(0, OrdenFactura.Comentarios, "Ventas/" + newFile, OrdenFactura.idOrdenCVP);
                Factura Existe = ObtenerFacturaByOrden(OrdenFactura.idOrdenCVP);

                if (Existe.Directorio != null)
                {
                    Factura.idFactura = Existe.idFactura;
                    ActualizarFactura(Factura);
                }
                else
                    AgregarFactura(Factura);

                PdfReader pdfReader = new PdfReader(pdfTemplate);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(@"Ventas/" + newFile, FileMode.Create));

                AcroFields pdfFormFields = pdfStamper.AcroFields;
                iTextSharp.text.Rectangle rect = pdfStamper.AcroFields.GetFieldPositions("Logotipo")[0].position;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@"Logotipos/logo400.png");
                img.ScaleAbsolute(rect.Width, rect.Height);
                img.SetAbsolutePosition(rect.Left, rect.Bottom);
                pdfStamper.GetOverContent(1).AddImage(img);

                DateTime Now = DateTime.Today;
                string DatosCliente = Empresa.Nombre +  //ASR 03-07-2018
                    "\nRFC: " + Empresa.RFC +
                    "\nDirección: " + Empresa.Calle + " #" + Empresa.Numero +
                    "\nCol. " + Empresa.Colonia +
                    "\nDel. " + Empresa.Municipio +
                    "\n" + Empresa.Estado + ", CP: " + Empresa.CP;
                string Atencion = "Atención a: " + OrdenFactura.Dirigido; //BRS 03-07-2018

                //BRS 20/07 Se genera factura con la primera sucursal matriz que se encuentre, si no se encuentra sucursal, se usará la sucursal donde se realice el pedido
                Sucursal Matriz = ObtenerSucursalMatriz();
                string TipoSucursal;
                string DatosSucursal;
                if (Matriz.idSucursal == 0)
                {
                    TipoSucursal = Sucursal.Matriz == true ? "Matriz" : "Sucursal";
                    DatosSucursal = Sucursal.NSucursal +
                        "\nDirección: " + Sucursal.Calle + " #" + Sucursal.Numero +
                        "\nCol. " + Sucursal.Colonia +
                        "\nDel. " + Sucursal.Municipio +
                        "\n" + Sucursal.Estado + ", CP: " + Sucursal.CP;
                }
                else
                {
                    TipoSucursal = "Matriz";
                    DatosSucursal = Matriz.NSucursal +
                        "\nDirección: " + Matriz.Calle + " #" + Matriz.Numero +
                        "\nCol. " + Matriz.Colonia +
                        "\nDel. " + Matriz.Municipio +
                        "\n" + Matriz.Estado + ", CP: " + Matriz.CP;

                }

                string Orden = "Orden de Venta";//"Orden: " + OrdenFactura.Operacion;

                /*
                string Vendedor = "Elaborado por: " +
                    "\nAprobado por: " +
                    "\nEstatus: ";

                string Aprobado = OrdenFactura.idEstatus == 2 ? OrdenFactura.EmpleadoAprobar : " ";

                string DatosVendedor = OrdenFactura.Empleado +
                    "\n" + Aprobado +
                    "\n" + OrdenFactura.Estatus;
                */
                string Resumen = "Subtotal: " +
                    "\nDescuento: " +
                    "\nI.V.A. " + OrdenFactura.IVA + "%: \t" +
                    "\n" +
                    "\nTotal: ";

                string DatosResumen = OrdenFactura.Subtotal.ToString("C2") +
                    "\n" + OrdenFactura.ValorDescuento.ToString("C2") +
                    "\n" + OrdenFactura.ValorIVA.ToString("C2") +
                    "\n" +
                    "\n" + OrdenFactura.Total.ToString("C2");

                pdfFormFields.SetField("TipoSucursal", TipoSucursal);
                pdfFormFields.SetField("Sucursal", DatosSucursal);
                pdfFormFields.SetField("TipoOrden", Orden);
                pdfFormFields.SetField("Folio", Archivo);
                pdfFormFields.SetField("Fecha", OrdenFactura.Fecha);
                /*
                pdfFormFields.SetField("Vendedor", Vendedor);
                pdfFormFields.SetField("DatosVendedor", DatosVendedor);
                */
                Productos Producto = new Productos();
                pdfFormFields.SetField("Cliente", DatosCliente);
                pdfFormFields.SetField("Atencion", Atencion);
                string Articulo = "", Nombre = "", Umed = "", Unidades = "", Precio = "", Importe = "", Color = "", PiezasTotales = "";//ASR 02-06-2018
                foreach (var ListaFactur in ListaFactura)
                {
                    Producto = ObtenerProductoById(ListaFactur.idProducto);//ASR 02-06-2018
                    Articulo += "\n" + ListaFactur.Codigo.ToString();//ASR 02-06-2018
                    Nombre += "\n" + ListaFactur.Producto;
                    Color += "\n" + ListaFactur.Color;//ASR 02-06-2018
                    PiezasTotales += "\n" + (ListaFactur.Cantidad * Producto.PiezasTotales).ToString();//ASR 02-06-2018
                    Umed += "\n" + ListaFactur.Presentacion;
                    Unidades += "\n" + ListaFactur.Cantidad.ToString();
                    Precio += "\n" + ListaFactur.CostoPrecio.ToString("C2");
                    Importe += "\n" + ListaFactur.ImporteTotal.ToString("C2");
                }
                pdfFormFields.SetField("Articulo", Articulo);
                pdfFormFields.SetField("Nombre", Nombre);
                pdfFormFields.SetField("Umed", Umed);
                pdfFormFields.SetField("Color", Color);//ASR 02-06-2018
                pdfFormFields.SetField("Unidades", Unidades);
                pdfFormFields.SetField("UnidadesTotales", PiezasTotales);//ASR 02-06-2018
                pdfFormFields.SetField("Precio", Precio);
                pdfFormFields.SetField("Importe", Importe);
                pdfFormFields.SetField("Resumen", Resumen);
                pdfFormFields.SetField("DatosResumen", DatosResumen);

                pdfStamper.FormFlattening = true;
                pdfStamper.Close();
            }
            catch (Exception e)
            {

                StreamWriter fs = File.AppendText("log.txt");
                fs.WriteLine(e.Message);
                fs.Close();
                fs.Dispose();
            }
            Cerrar_Conexion();
        }


        public void FacturaCompas(OrdenesCVP OrdenC)
        {
            try
            {
                OrdenesCVP OrdenFactura = ObtenerOrdenCById(OrdenC.idOrdenCVP);
                List<ListaProductos> ListaFactura = ObtenerProductosByOrden(OrdenFactura.idOrdenCVP);
                Proveedor Proveedor = ObtenerProveedorById(OrdenFactura.idProveedor);
                Empresa Empresa = ObtenerEmpresaById(Proveedor.idEmpresa);
                Sucursal Sucursal = ObtenerSucursalById(OrdenFactura.idSucursal);
                string Archivo = OrdenFactura.Prefijo + OrdenFactura.Folio;
                string pdfTemplate = @"PlantillaPDFProvee.pdf";
                string newFile = Archivo + ".pdf";

                Directory.CreateDirectory("Compras");

                Factura Factura = new Factura(0, OrdenFactura.Comentarios, "Compras/" + newFile, OrdenFactura.idOrdenCVP);
                Factura Existe = ObtenerFacturaByOrden(OrdenFactura.idOrdenCVP);

                if (Existe.Directorio != null)
                {
                    Factura.idFactura = Existe.idFactura;
                    ActualizarFactura(Factura);
                }
                else
                    AgregarFactura(Factura);

                string directorioActual = Directory.GetCurrentDirectory();
                PdfReader pdfReader = new PdfReader(pdfTemplate);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(@"Compras/" + newFile, FileMode.Create));

                AcroFields pdfFormFields = pdfStamper.AcroFields;
                iTextSharp.text.Rectangle rect = pdfStamper.AcroFields.GetFieldPositions("Logotipo")[0].position;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@"Logotipos/logo400.png");
                img.ScaleAbsolute(rect.Width, rect.Height);
                img.SetAbsolutePosition(rect.Left, rect.Bottom);
                pdfStamper.GetOverContent(1).AddImage(img);

                DateTime Now = DateTime.Today;
                string DatosCliente = Empresa.Nombre +
                    "\nRFC: " + Empresa.RFC +
                    "\nDirección: " + Empresa.Calle + " #" + Empresa.Numero +
                    "\nCol. " + Empresa.Colonia +
                    "\nDel. " + Empresa.Municipio +
                    "\n" + Empresa.Estado + ", CP: " + Empresa.CP;

                string TipoSucursal = Sucursal.Matriz == true ? "Matriz" : "Sucursal";
                string DatosSucursal = Sucursal.NSucursal +
                    "\nDirección: " + Sucursal.Calle + " #" + Sucursal.Numero +
                    "\nCol. " + Sucursal.Colonia +
                    "\nDel. " + Sucursal.Municipio +
                    "\n" + Sucursal.Estado + ", CP: " + Sucursal.CP;

                string Orden = "Orden de Compra";//"Orden: " + OrdenFactura.Operacion;
                /*
                string Vendedor = "Elaborado por: " +
                    "\nAprobado por: " +
                    "\nEstatus: ";

                string Aprobado = OrdenFactura.idEstatus == 2 ? OrdenFactura.EmpleadoAprobar : " ";

                string DatosVendedor = OrdenFactura.Empleado +
                    "\n" + Aprobado +
                    "\n" + OrdenFactura.Estatus;
                */
                string Resumen = "Subtotal: " +
                    "\nDescuento: " +
                    "\nI.V.A. " + OrdenFactura.IVA + "%: \t" +
                    "\n" +
                    "\nTotal: ";

                string DatosResumen = OrdenFactura.Subtotal.ToString("C2") +
                    "\n" + OrdenFactura.ValorDescuento.ToString("C2") +
                    "\n" + OrdenFactura.ValorIVA.ToString("C2") +
                    "\n" +
                    "\n" + OrdenFactura.Total.ToString("C2");

                pdfFormFields.SetField("TipoSucursal", TipoSucursal);
                pdfFormFields.SetField("Sucursal", DatosSucursal);
                pdfFormFields.SetField("TipoOrden", Orden);
                pdfFormFields.SetField("Folio", Archivo);
                pdfFormFields.SetField("Fecha", OrdenFactura.Fecha);
                /*
                pdfFormFields.SetField("Vendedor", Vendedor);
                pdfFormFields.SetField("DatosVendedor", DatosVendedor);
                */
                Productos Producto = new Productos();
                pdfFormFields.SetField("Cliente", DatosCliente);
                string Articulo = "", Nombre = "", Umed = "", Unidades = "", Precio = "", Importe = "", Color = "", PiezasTotales = "";//ASR 02-06-2018
                foreach (var ListaFactur in ListaFactura)
                {
                    Producto = ObtenerProductoById(ListaFactur.idProducto);//ASR 02-06-2018
                    Articulo += "\n" + ListaFactur.Codigo.ToString();//ASR 02-06-2018
                    Nombre += "\n" + ListaFactur.Producto;
                    Color += "\n" + ListaFactur.Color;//ASR 02-06-2018
                    PiezasTotales += "\n" + (ListaFactur.Cantidad * Producto.PiezasTotales).ToString();//ASR 02-06-2018
                    Umed += "\n" + ListaFactur.Presentacion;
                    Unidades += "\n" + ListaFactur.Cantidad.ToString();
                    Precio += "\n" + ListaFactur.CostoPrecio.ToString("C2");
                    Importe += "\n" + ListaFactur.ImporteTotal.ToString("C2");
                }
                pdfFormFields.SetField("Articulo", Articulo);
                pdfFormFields.SetField("Nombre", Nombre);
                pdfFormFields.SetField("Umed", Umed);
                pdfFormFields.SetField("Color", Color);//ASR 02-06-2018
                pdfFormFields.SetField("Unidades", Unidades);
                pdfFormFields.SetField("UnidadesTotales", PiezasTotales);//ASR 02-06-2018
                pdfFormFields.SetField("Precio", Precio);
                pdfFormFields.SetField("Importe", Importe);
                pdfFormFields.SetField("Resumen", Resumen);
                pdfFormFields.SetField("DatosResumen", DatosResumen);

                pdfStamper.FormFlattening = true;
                pdfStamper.Close();
            }
            catch (Exception e)
            {

                StreamWriter fs = File.AppendText("log.txt");
                fs.WriteLine(e.Message);
                fs.Close();
                fs.Dispose();
            }
            Cerrar_Conexion();
        }

        public void FacturaCotizacion(OrdenesCVP OrdenV)
        {
            try
            {
                OrdenesCVP OrdenFactura = ObtenerCotizacionById(OrdenV.idOrdenCVP);
                List<ListaProductos> ListaFactura = ObtenerProductosByOrden(OrdenFactura.idOrdenCVP);
                Cliente Cliente = ObtenerClienteById(OrdenFactura.idCliente);
                Empresa Empresa = ObtenerEmpresaById(Cliente.idEmpresa);
                Sucursal Sucursal = ObtenerSucursalById(OrdenFactura.idSucursal);
                string Archivo = OrdenFactura.Prefijo + OrdenFactura.Folio;
                string pdfTemplate = @"PlantillaPDF2.pdf";
                string newFile = Archivo + ".pdf";

                Directory.CreateDirectory("Cotizacion");

                Factura Factura = new Factura(0, OrdenFactura.Comentarios, "Cotizacion/" + newFile, OrdenFactura.idOrdenCVP);
                Factura Existe = ObtenerFacturaByOrden(OrdenFactura.idOrdenCVP);

                if (Existe.Directorio != null)
                {
                    Factura.idFactura = Existe.idFactura;
                    ActualizarFactura(Factura);
                }
                else
                    AgregarFactura(Factura);

                PdfReader pdfReader = new PdfReader(pdfTemplate);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(@"Cotizacion/" + newFile, FileMode.Create));

                AcroFields pdfFormFields = pdfStamper.AcroFields;
                iTextSharp.text.Rectangle rect = pdfStamper.AcroFields.GetFieldPositions("Logotipo")[0].position;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@"Logotipos/logo400.png");
                img.ScaleAbsolute(rect.Width, rect.Height);
                img.SetAbsolutePosition(rect.Left, rect.Bottom);
                pdfStamper.GetOverContent(1).AddImage(img);

                DateTime Now = DateTime.Today;
                string DatosCliente = Empresa.Nombre +  //ASR 03-07-2018
                    "\nRFC: " + Empresa.RFC +
                    "\nDirección: " + Empresa.Calle + " #" + Empresa.Numero +
                    "\nCol. " + Empresa.Colonia +
                    "\nDel. " + Empresa.Municipio +
                    "\n" + Empresa.Estado + ", CP: " + Empresa.CP;

                string Atencion = "Atención a: " + OrdenFactura.Dirigido; //BRS 03-07-2018

                //BRS 20/07 Se genera factura con la primera sucursal matriz que se encuentre, si no se encuentra sucursal, se usará la sucursal donde se realice el pedido
                Sucursal Matriz = ObtenerSucursalMatriz();
                string TipoSucursal;
                string DatosSucursal;
                if (Matriz.idSucursal == 0)
                {
                    TipoSucursal = Sucursal.Matriz == true ? "Matriz" : "Sucursal";
                    DatosSucursal = Sucursal.NSucursal +
                        "\nDirección: " + Sucursal.Calle + " #" + Sucursal.Numero +
                        "\nCol. " + Sucursal.Colonia +
                        "\nDel. " + Sucursal.Municipio +
                        "\n" + Sucursal.Estado + ", CP: " + Sucursal.CP;
                }
                else
                {
                    TipoSucursal = "Matriz";
                    DatosSucursal = Matriz.NSucursal +
                        "\nDirección: " + Matriz.Calle + " #" + Matriz.Numero +
                        "\nCol. " + Matriz.Colonia +
                        "\nDel. " + Matriz.Municipio +
                        "\n" + Matriz.Estado + ", CP: " + Matriz.CP;

                }

                string Orden = "Cotización";//"Orden: " + OrdenFactura.Operacion;
                /*
                string Vendedor = "Elaborado por: " +
                    "\nAprobado por: " +
                    "\nEstatus: ";

                string Aprobado = OrdenFactura.idEstatus == 2 ? OrdenFactura.EmpleadoAprobar : " ";

                string DatosVendedor = OrdenFactura.Empleado +
                    "\n" + Aprobado +
                    "\n" + OrdenFactura.Estatus;
                */
                string Resumen = "Subtotal: " +
                    "\nDescuento: " +
                    "\nI.V.A. " + OrdenFactura.IVA + "%: \t" +
                    "\n" +
                    "\nTotal: ";

                string DatosResumen = OrdenFactura.Subtotal.ToString("C2") +
                    "\n" + OrdenFactura.ValorDescuento.ToString("C2") +
                    "\n" + OrdenFactura.ValorIVA.ToString("C2") +
                    "\n" +
                    "\n" + OrdenFactura.Total.ToString("C2");

                pdfFormFields.SetField("TipoSucursal", TipoSucursal);
                pdfFormFields.SetField("Sucursal", DatosSucursal);
                pdfFormFields.SetField("TipoOrden", Orden);
                pdfFormFields.SetField("Folio", Archivo);
                pdfFormFields.SetField("Fecha", OrdenFactura.Fecha);
                /*
                pdfFormFields.SetField("Vendedor", Vendedor);
                pdfFormFields.SetField("DatosVendedor", DatosVendedor);
                */
                Productos Producto = new Productos();
                pdfFormFields.SetField("Cliente", DatosCliente);
                pdfFormFields.SetField("Atencion", Atencion);
                string Articulo = "", Nombre = "", Umed = "", Unidades = "", Precio = "", Importe = "", Color = "", PiezasTotales = "";//ASR 02-06-2018
                foreach (var ListaFactur in ListaFactura)
                {
                    Producto = ObtenerProductoById(ListaFactur.idProducto);//ASR 02-06-2018
                    Articulo += "\n" + ListaFactur.Codigo.ToString();//ASR 02-06-2018
                    Nombre += "\n" + ListaFactur.Producto;
                    Color += "\n" + ListaFactur.Color;//ASR 02-06-2018
                    PiezasTotales += "\n" + (ListaFactur.Cantidad * Producto.PiezasTotales).ToString();//ASR 02-06-2018
                    Umed += "\n" + ListaFactur.Presentacion;
                    Unidades += "\n" + ListaFactur.Cantidad.ToString();
                    Precio += "\n" + ListaFactur.CostoPrecio.ToString("C2");
                    Importe += "\n" + ListaFactur.ImporteTotal.ToString("C2");
                }
                pdfFormFields.SetField("Articulo", Articulo);
                pdfFormFields.SetField("Nombre", Nombre);
                pdfFormFields.SetField("Umed", Umed);
                pdfFormFields.SetField("Color", Color);//ASR 02-06-2018
                pdfFormFields.SetField("Unidades", Unidades);
                pdfFormFields.SetField("UnidadesTotales", PiezasTotales);//ASR 02-06-2018
                pdfFormFields.SetField("Precio", Precio);
                pdfFormFields.SetField("Importe", Importe);
                pdfFormFields.SetField("Resumen", Resumen);
                pdfFormFields.SetField("DatosResumen", DatosResumen);

                pdfStamper.FormFlattening = true;
                pdfStamper.Close();
            }
            catch (Exception e)
            {

                StreamWriter fs = File.AppendText("log.txt");
                fs.WriteLine(e.Message);
                fs.Close();
                fs.Dispose();
            }
            Cerrar_Conexion();
        }

        public void FacturaCotizacionXpz(OrdenesCVP OrdenV)//ASR 22-06-2018
        {
            try
            {
                OrdenesCVP OrdenFactura = ObtenerCotizacionByIdXpz(OrdenV.idOrdenCVP);
                List<ListaProductos> ListaFactura = ObtenerProductosByOrden(OrdenFactura.idOrdenCVP);
                Cliente Cliente = ObtenerClienteById(OrdenFactura.idCliente);
                Empresa Empresa = ObtenerEmpresaById(Cliente.idEmpresa);
                Sucursal Sucursal = ObtenerSucursalById(OrdenFactura.idSucursal);
                string Archivo = OrdenFactura.Prefijo + OrdenFactura.Folio;
                string pdfTemplate = @"PlantillaPDF2.pdf";
                string newFile = Archivo + ".pdf";

                Directory.CreateDirectory("Cotizacionpz");

                Factura Factura = new Factura(0, OrdenFactura.Comentarios, "Cotizacionpz/" + newFile, OrdenFactura.idOrdenCVP);
                Factura Existe = ObtenerFacturaByOrden(OrdenFactura.idOrdenCVP);

                if (Existe.Directorio != null)
                {
                    Factura.idFactura = Existe.idFactura;
                    ActualizarFactura(Factura);
                }
                else
                    AgregarFactura(Factura);

                PdfReader pdfReader = new PdfReader(pdfTemplate);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(@"Cotizacionpz/" + newFile, FileMode.Create));

                AcroFields pdfFormFields = pdfStamper.AcroFields;
                iTextSharp.text.Rectangle rect = pdfStamper.AcroFields.GetFieldPositions("Logotipo")[0].position;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@"Logotipos/logo400.png");
                img.ScaleAbsolute(rect.Width, rect.Height);
                img.SetAbsolutePosition(rect.Left, rect.Bottom);
                pdfStamper.GetOverContent(1).AddImage(img);

                DateTime Now = DateTime.Today;
                string DatosCliente = Empresa.Nombre +
                    "\nRFC: " + Empresa.RFC +
                    "\nDirección: " + Empresa.Calle + " #" + Empresa.Numero +
                    "\nCol. " + Empresa.Colonia +
                    "\nDel. " + Empresa.Municipio +
                    "\n" + Empresa.Estado + ", CP: " + Empresa.CP;

                //BRS 20/07 Se genera factura con la primera sucursal matriz que se encuentre, si no se encuentra sucursal, se usará la sucursal donde se realice el pedido
                Sucursal Matriz = ObtenerSucursalMatriz();
                string TipoSucursal;
                string DatosSucursal;
                if (Matriz.idSucursal == 0)
                {
                    TipoSucursal = Sucursal.Matriz == true ? "Matriz" : "Sucursal";
                    DatosSucursal = Sucursal.NSucursal +
                        "\nDirección: " + Sucursal.Calle + " #" + Sucursal.Numero +
                        "\nCol. " + Sucursal.Colonia +
                        "\nDel. " + Sucursal.Municipio +
                        "\n" + Sucursal.Estado + ", CP: " + Sucursal.CP;
                }
                else
                {
                    TipoSucursal = "Matriz";
                    DatosSucursal = Matriz.NSucursal +
                        "\nDirección: " + Matriz.Calle + " #" + Matriz.Numero +
                        "\nCol. " + Matriz.Colonia +
                        "\nDel. " + Matriz.Municipio +
                        "\n" + Matriz.Estado + ", CP: " + Matriz.CP;

                }

                string Orden = "Cotización";

                string Resumen = "Subtotal: " +
                    "\nDescuento: " +
                    "\nI.V.A. " + OrdenFactura.IVA + "%: \t" +
                    "\n" +
                    "\nTotal: ";

                string DatosResumen = OrdenFactura.Subtotal.ToString("C2") +
                    "\n" + OrdenFactura.ValorDescuento.ToString("C2") +
                    "\n" + OrdenFactura.ValorIVA.ToString("C2") +
                    "\n" +
                    "\n" + OrdenFactura.Total.ToString("C2");

                pdfFormFields.SetField("TipoSucursal", TipoSucursal);
                pdfFormFields.SetField("Sucursal", DatosSucursal);
                pdfFormFields.SetField("TipoOrden", Orden);
                pdfFormFields.SetField("Folio", Archivo);
                pdfFormFields.SetField("Fecha", OrdenFactura.Fecha);

                Productos Producto = new Productos();
                pdfFormFields.SetField("Cliente", DatosCliente);
                string Articulo = "", Nombre = "", Umed = "", Unidades = "", Precio = "", Importe = "", Color = "", PiezasTotales = "";
                foreach (var ListaFactur in ListaFactura)
                {
                    Producto = ObtenerProductoById(ListaFactur.idProducto);
                    Articulo += "\n" + ListaFactur.Codigo.ToString();
                    Nombre += "\n" + ListaFactur.Producto;
                    Color += "\n" + ListaFactur.Color;
                    PiezasTotales += "\n" + ListaFactur.Cantidad.ToString();
                    Umed += "\n" + ListaFactur.Presentacion;
                    Unidades += "\n" + ListaFactur.Cantidad.ToString();
                    Precio += "\n" + ListaFactur.CostoPrecio.ToString("C2");
                    Importe += "\n" + ListaFactur.ImporteTotal.ToString("C2");
                }
                pdfFormFields.SetField("Articulo", Articulo);
                pdfFormFields.SetField("Nombre", Nombre);
                pdfFormFields.SetField("Umed", Umed);
                pdfFormFields.SetField("Color", Color);
                pdfFormFields.SetField("Unidades", Unidades);
                pdfFormFields.SetField("UnidadesTotales", PiezasTotales);
                pdfFormFields.SetField("Precio", Precio);
                pdfFormFields.SetField("Importe", Importe);
                pdfFormFields.SetField("Resumen", Resumen);
                pdfFormFields.SetField("DatosResumen", DatosResumen);

                pdfStamper.FormFlattening = true;
                pdfStamper.Close();
            }
            catch (Exception e)
            {

                StreamWriter fs = File.AppendText("log.txt");
                fs.WriteLine(e.Message);
                fs.Close();
                fs.Dispose();
            }
            Cerrar_Conexion();
        }
        #endregion

        #region Dash
        public DASH ObtenerDatosDash()
        {
            DASH Datos = new DASH();


            string Query = "SELECT count(*) AS Cantidad FROM Producto;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.CantidadProductos = ReaderBD.GetInt32(0);



            Query = "SELECT count(*) AS Cantidad FROM Proveedor;";

            ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.CantidadProveedores = ReaderBD.GetInt32(0);



            Query = "SELECT count(*) AS Cantidad FROM Cliente;";

            ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.CantidadClientes = ReaderBD.GetInt32(0);



            Query = "SELECT count(*) AS Cantidad FROM OrdenesCVP WHERE OrdenesCVP.idOperacion = 3;";

            ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.CantidadVentas = ReaderBD.GetInt32(0);



            Query = "SELECT count(*) AS Cantidad FROM OrdenesCVP WHERE OrdenesCVP.idOperacion = 1;";

            ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.CantidadCompras = ReaderBD.GetInt32(0);



            Query = "SELECT count(*) AS Cantidad FROM OrdenesCVP WHERE OrdenesCVP.idOperacion = 2;";

            ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.CantidadCotizaciones = ReaderBD.GetInt32(0);


            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        #endregion

        #region Privilegios 
        public List<Privilegios> ObtenerPrivilegios()
        {
            List<Privilegios> Lista = new List<Privilegios>();

            string Query = "SELECT idPrivilegio, Privilegio FROM Privilegios;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Privilegios(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }
        #endregion

        #region Movimientos Inventario
        public List<InventarioMov> ObtenerMovimientos(int id) 
        {
            List<InventarioMov> Lista = new List<InventarioMov>();

            string Query = "SELECT IM.idInventarioMov, IM.idInventario, Inventario.idProducto, Producto.Producto, IM.idSucursal, Sucursal.Sucursal, Producto.Color, IM.Cantidad, IM.CantidadActual, IM.CantidadNueva, IM.Movimiento, IM.Fecha, Producto.idPresentacion, Presentaciones.Presentacion " +
                "FROM InventarioMov IM " +
                "INNER JOIN Inventario ON IM.idInventario = Inventario.idInventario " +
                "INNER JOIN Producto ON Inventario.idProducto = Producto.idProducto " +
                "INNER JOIN Sucursal ON IM.idSucursal = Sucursal.idSucursal " +
                "INNER JOIN Presentaciones ON Producto.idPresentacion = Presentaciones.idPresentaciones " +
                "WHERE IM.idInventario = '" + id + "' ORDER BY Fecha ASC;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new InventarioMov(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetString(6),
                        ReaderBD.GetInt32(7), ReaderBD.GetInt32(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetDateTime(11), ReaderBD.GetInt32(12), ReaderBD.GetString(13)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public void AgregarMovimiento(InventarioMov Datos)
        {

            Datos.Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string Query = string.Format("INSERT INTO InventarioMov (idInventario, idSucursal, Cantidad, CantidadActual, CantidadNueva, Movimiento, Fecha) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                Datos.idInventario, Datos.idSucursal, Datos.Cantidad, Datos.CantidadActual, Datos.CantidadNueva, Datos.Movimiento, Datos.Fecha);
            ActualizarDatos(Query);
        }
        #endregion




        /// Recicladora
        /// 

        #region Almacen
        public List<Almacen> ObtenerAlmacenes()
        {
            List<Almacen> Lista = new List<Almacen>();

            string Query = "SELECT idalmacen, nombreAlmacen FROM almacen;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Almacen(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Almacen ObtenerAlmacenById(int id)
        {
            Almacen Datos = new Almacen();

            string Query = "SELECT idalmacen, nombreAlmacen FROM almacen WHERE idalmacen = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Almacen(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public List<Almacen> ObtenerAlmacenByProceso(int id)
        {
            List<Almacen> Lista = new List<Almacen>();

            string Query = "SELECT idalmacen, nombreAlmacen FROM almacen WHERE idProceso = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Almacen(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Almacen ObtenerAlmacenByName(string Nombre)
        {
            Almacen Datos = new Almacen();

            string Query = "SELECT idalmacen, nombreAlmacen FROM almacen WHERE nombreAlmacen = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Almacen(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarAlmacen(Almacen Datos)
        {

            string Query = string.Format("INSERT INTO almacen (nombreAlmacen, idProceso) VALUES ('{0}', '{1}');", Datos.NombreAlmacen, Datos.idProceso);
            return ActualizarDatos(Query);
        }

        public void ActualizarAlmacen(Almacen Datos)
        {

            string Query = string.Format("UPDATE almacen SET nombreAlmacen = '{0}' WHERE idalmacen = '{1}';", Datos.NombreAlmacen, Datos.idAlmacen);
            ActualizarDatos(Query);
        }

        public void EliminarAlmacen(int id)
        {

            string Query = string.Format("DELETE FROM almacen WHERE idalmacen = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region TipoMateriales
        public List<TipoMateriales> ObtenerTipoMateriales()
        {
            List<TipoMateriales> Lista = new List<TipoMateriales>();

            string Query = "SELECT idTipoMaterial, TipoMaterial FROM tipoMaterial;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new TipoMateriales(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }
        #endregion

        #region Bascula
        public List<Bascula> ObtenerBasculas()
        {
            List<Bascula> Lista = new List<Bascula>();

            string Query = "SELECT idbascula, modelo, pesoMaximo, ultimoMantenimiento FROM bascula;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Bascula(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDouble(2), ReaderBD.GetDateTime(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Bascula ObtenerBasculaById(int id)
        {
            Bascula Datos = new Bascula();

            string Query = "SELECT idbascula, modelo, pesoMaximo, ultimoMantenimiento FROM bascula WHERE idbascula = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Bascula(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDouble(2), ReaderBD.GetDateTime(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Bascula ObtenerBasculaByName(string Nombre)
        {
            Bascula Datos = new Bascula();

            string Query = "SELECT idbascula, modelo, pesoMaximo, ultimoMantenimiento FROM bascula WHERE modelo = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Bascula(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDouble(2), ReaderBD.GetDateTime(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarBascula(Bascula Datos)
        {
            string Query = string.Format("INSERT INTO bascula (modelo, pesoMaximo, ultimoMantenimiento) VALUES ('{0}', '{1}', '{2}');", Datos.Modelo, Datos.PesoMaximo, Datos.FechaMantenimiento);
            return ActualizarDatos(Query);
        }

        public void ActualizarBascula(Bascula Datos)
        {

            string Query = string.Format("UPDATE bascula SET modelo = '{0}', pesoMaximo = '{1}', ultimoMantenimiento = '{2}' WHERE idbascula = '{3}';", Datos.Modelo, Datos.PesoMaximo, Datos.FechaMantenimiento, Datos.idBascula);
            ActualizarDatos(Query);
        }

        public void EliminarBascula(int id)
        {

            string Query = string.Format("DELETE FROM bascula WHERE idbascula = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Material
        public List<Material> ObtenerMateriales()
        {
            List<Material> Lista = new List<Material>();

            string Query = "SELECT idmaterial, nombreMaterial, color, material.idTipoMaterial, tipoMaterial.TipoMaterial FROM material " +
                "INNER JOIN tipoMaterial ON material.idTipoMaterial = tipoMaterial.idTipoMaterial;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Material(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Material ObtenerMaterialById(int id)
        {
            Material Datos = new Material();

            string Query = "SELECT idmaterial, nombreMaterial, color, material.idTipoMaterial, tipoMaterial.TipoMaterial FROM material " +
                "INNER JOIN tipoMaterial ON material.idTipoMaterial = tipoMaterial.idTipoMaterial " +
                "WHERE idmaterial = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Material(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Material ObtenerMaterialByName(string Nombre)
        {
            Material Datos = new Material();

            string Query = "SELECT idmaterial, nombreMaterial, color, material.idTipoMaterial, tipoMaterial.TipoMaterial FROM material " +
                "INNER JOIN tipoMaterial ON material.idTipoMaterial = tipoMaterial.idTipoMaterial " + 
                "WHERE nombreMaterial = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Material(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarMaterial(Material Datos)
        {

            string Query = string.Format("INSERT INTO material (nombreMaterial, color, idTipoMaterial) VALUES ('{0}', '{1}', '{2}');", Datos.NombreMaterial, Datos.Color, Datos.idTipoMaterial);
            return ActualizarDatos(Query);
        }

        public void ActualizarMaterial(Material Datos)
        {

            string Query = string.Format("UPDATE material SET nombreMaterial = '{0}', color = '{2}', idTipoMaterial = '{3}' WHERE idmaterial = '{1}';", Datos.NombreMaterial, Datos.idMaterial, Datos.Color, Datos.idTipoMaterial);
            ActualizarDatos(Query);
        }

        public void EliminarMaterial(int id)
        {

            string Query = string.Format("DELETE FROM material WHERE idmaterial = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region OperacionRecepcion
        public List<OperacionRecepcion> ObtenerOperacionesRecepcion()
        {
            List<OperacionRecepcion> Lista = new List<OperacionRecepcion>();

            string Query = "SELECT idoperacionRecepcion, nombreOperacion FROM operacionRecepcion;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new OperacionRecepcion(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public OperacionRecepcion ObtenerOperacionRecepcionById(int id)
        {
            OperacionRecepcion Datos = new OperacionRecepcion();

            string Query = "SELECT idoperacionRecepcion, nombreOperacion FROM operacionRecepcion WHERE idoperacionRecepcion = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new OperacionRecepcion(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public OperacionRecepcion ObtenerOperacionRecepcionByName(string Nombre)
        {
            OperacionRecepcion Datos = new OperacionRecepcion();

            string Query = "SELECT idoperacionRecepcion, nombreOperacion FROM operacionRecepcion WHERE nombreOperacion = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new OperacionRecepcion(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarOperacionRecepcion(OperacionRecepcion Datos)
        {

            string Query = string.Format("INSERT INTO operacionRecepcion (nombreOperacion) VALUES ('{0}');", Datos.NombreOperacion);
            return ActualizarDatos(Query);
        }

        public void ActualizarOperacionRecepcion(OperacionRecepcion Datos)
        {

            string Query = string.Format("UPDATE operacionRecepcion SET nombreOperacion = '{0}' WHERE idoperacionRecepcion = '{1}';", Datos.NombreOperacion, Datos.idOperacionRecepcion);
            ActualizarDatos(Query);
        }

        public void EliminarOperacionRecepcion(int id)
        {

            string Query = string.Format("DELETE FROM operacionRecepcion WHERE idoperacionRecepcion = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Paquete
        public List<Paquete> ObtenerPaquetes()
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Paquete ObtenerPaqueteById(int id)
        {
            Paquete Datos = new Paquete();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE idpaquete = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente);

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Paquete ObtenerPaqueteCompletoById(int id)
        {
            Paquete Datos = new Paquete();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE idpaquete = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente, ObtenerPaquetesByPaqueteSuperior(ReaderBD.GetInt32(0)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public List<Paquete> ObtenerPaquetesByPaqueteSuperior(int id)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idPaqueteSuperior = '" + id + "' ORDER BY fechaPesado ASC, codigoPaq ASC;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerPaquetesByRecepcion(int id)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idrecepcion = '" + id + "' ORDER BY fechaPesado ASC, codigoPaq ASC;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerPaquetesProduccionByRecepcionProceso(int idRecepcion, int idProcesoProduccion)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, " +
                "bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, " +
                "proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, " +
                "material.color, PesoSupRestante, paquete.idProcesoProduccion, P2.nombreProceso, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN tipoMaterial ON material.idTipoMaterial = tipoMaterial.idTipoMaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN proceso P2 ON paquete.idprocesoProduccion = P2.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idrecepcion = '" + idRecepcion + "' AND paquete.idProcesoProduccion = '" + idProcesoProduccion + "' AND TipoMaterial.idTipoMaterial = 1 " +
                "ORDER BY fechaPesado ASC, codigoPaq ASC;";

            Paquete Agregar;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    Agregar = new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(28), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente);
                    Agregar.idProcesoProduccion = ReaderBD.GetInt32(26);
                    Agregar.NombreProcesoProduccion = ReaderBD.GetString(27);
                    Lista.Add(Agregar);
                }

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerPaquetesBasuraByRecepcionProceso(int idRecepcion, int idProcesoProduccion)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, sum(peso), paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, " +
                "bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, " +
                "proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, " +
                "material.color, PesoSupRestante, paquete.idProcesoProduccion, P2.nombreProceso, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN tipoMaterial ON material.idTipoMaterial = tipoMaterial.idTipoMaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN proceso P2 ON paquete.idprocesoProduccion = P2.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idrecepcion = '" + idRecepcion + "' AND paquete.idProcesoProduccion = '" + idProcesoProduccion + "' AND TipoMaterial.idTipoMaterial = 2 " +
                "GROUP BY material.idTipoMaterial " + 
                "ORDER BY fechaPesado ASC, codigoPaq ASC;";

            Paquete Agregar;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    Agregar = new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(28), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente);
                    Agregar.idProcesoProduccion = ReaderBD.GetInt32(26);
                    Agregar.NombreProcesoProduccion = ReaderBD.GetString(27);
                    Lista.Add(Agregar);
                }

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerPaquetesEntradaProduccionByRecepcionProceso(int idRecepcion, int idProceso)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, " +
                "bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, " +
                "proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, " +
                "material.color, PesoSupRestante, paquete.idProcesoProduccion, P2.nombreProceso, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN tipoMaterial ON material.idTipoMaterial = tipoMaterial.idTipoMaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN proceso P2 ON paquete.idprocesoProduccion = P2.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idrecepcion = '" + idRecepcion + "' AND paquete.idProceso = '" + idProceso + "' AND TipoMaterial.idTipoMaterial = 1 AND idEstatus != 1 AND idEstatus != 4 " +
                "ORDER BY fechaPesado ASC, codigoPaq ASC;";

            Paquete Agregar;
            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                {
                    Agregar = new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(28), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente);
                    Agregar.idProcesoProduccion = ReaderBD.GetInt32(26);
                    Agregar.NombreProcesoProduccion = ReaderBD.GetString(27);
                    Lista.Add(Agregar);
                }

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerPaquetesDisponibles()
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN tipoMaterial ON material.idTipoMaterial = tipoMaterial.idTipoMaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idEstatus = '1' AND material.idTipoMaterial = '1' ORDER BY fechaPesado ASC, codigoPaq ASC;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerPaquetesParaTriturar()
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idProceso = '3' AND paquete.idEstatus = '2' ORDER BY fechaPesado ASC, codigoPaq ASC;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerPaquetesParaProcesar(int idProceso)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idProceso = '" + idProceso + "' AND paquete.idEstatus = '2' ORDER BY fechaPesado ASC, codigoPaq ASC;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerProduccionbyProcesoGeneral(int idProcesoProduccion, string fecha/*, string codigo*/)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT paquete.idProcesoProduccion, paquete.fechaPesado, sum(peso) as pesototal, paquete.idMaquina, maquina.nombre, turno_idturno, turno.tipo FROM paquete " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "WHERE fechaPesado LIKE concat('" + fecha + "','%') " +
                "AND paquete.idProcesoProduccion = '" + idProcesoProduccion + "' " +
                "GROUP BY paquete.turno_idturno; ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetDouble(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerProduccionbyProcesoGeneralByTurno(int idProcesoProduccion, string fecha, int idTurno)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT paquete.idProcesoProduccion, paquete.fechaPesado, sum(peso) as pesototal, paquete.idMaquina, maquina.nombre, turno_idturno, turno.tipo FROM paquete " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "WHERE fechaPesado LIKE concat('" + fecha + "','%') " +
                "AND paquete.idProcesoProduccion = '" + idProcesoProduccion + "' " +
                "AND paquete.turno_idturno = '" + idTurno + "' " +
                "GROUP BY idMaquina;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetDouble(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerProduccionbyMaquinaFechaTurnoProceso(int idProcesoProduccion, int idTurno, int idMaquina, string Fecha)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT paquete.idProcesoProduccion, paquete.fechaPesado, codigoPaq, paquete.idmaterial, material.nombreMaterial, material.color, peso, paquete.idrecepcion, paquete.idMaquina, maquina.nombre, turno_idturno, turno.tipo, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "WHERE fechaPesado LIKE '" + Fecha + "%' " +
                "AND paquete.idProcesoProduccion = '" + idProcesoProduccion + "' " +
                "AND paquete.turno_idturno = '" + idTurno + "' " +
                "AND paquete.idMaquina = '" + idMaquina + "' " +
                "ORDER BY fechaPesado ASC;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(0, ReaderBD.GetDouble(6), "", 0, "", ReaderBD.GetInt32(7), 0, "", ReaderBD.GetDateTime(1),
                        0, "", ReaderBD.GetInt32(3), ReaderBD.GetString(4), "", 0, "", ReaderBD.GetInt32(10),
                        ReaderBD.GetString(11), ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetString(2), 0, "", 0, ReaderBD.GetString(5), ReaderBD.GetInt32(12), ReaderBD.GetDouble(13)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }
        
        
        public List<Paquete> ObtenerProduccionbyTurno(int id)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idEstatus = '1' AND codigoPaq LIKE '%TR%' AND fechaPesado LIKE concat(curdate(),'%') AND paquete.turno_idturno= '" + id + "' ORDER BY turno_idturno ASC, codigoPaq ASC; ";

            if (id == 0)
            {
                Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.idEstatus, EstatusPaquete.Estatus, idPaqueteSuperior, material.color, PesoSupRestante, Precio FROM paquete " +
                "INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
                "INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
                "INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
                "INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
                "INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
                "INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
                "INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN EstatusPaquete ON paquete.idEstatus = EstatusPaquete.idEstatusPaquete " +
                "WHERE paquete.idEstatus = '1' AND codigoPaq LIKE '%TR%' AND fechaPesado LIKE concat(curdate(),'%') ORDER BY turno_idturno ASC, codigoPaq ASC; ";
            }

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Paquete ObtenerProduccionbyProcesoFinal(int idRecepcion, int idProcesoProduccion)
        {
            Paquete Datos = new Paquete();

            string Query = "SELECT paquete.idProcesoProduccion, idrecepcion, paquete.fechaPesado, sum(peso) as pesototal FROM paquete " +
                "INNER JOIN Material ON paquete.idmaterial = Material.idMaterial " +
                "INNER JOIN TipoMaterial ON Material.idTipoMaterial = TipoMaterial.idTipoMaterial " +
                "WHERE paquete.idProcesoProduccion = '" + idProcesoProduccion + "' " +
                "AND paquete.idRecepcion = '" + idRecepcion + "' " +
                "AND material.idTipoMaterial = 1;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Paquete()
                    {
                        idProcesoProduccion = ReaderBD["idProcesoProduccion"] != DBNull.Value ? ReaderBD.GetInt32(0) : 0,
                        idRecepcion = ReaderBD["idrecepcion"] != DBNull.Value ? ReaderBD.GetInt32(1) : 0,
                        Peso = ReaderBD["pesototal"] != DBNull.Value ? ReaderBD.GetDouble(3) : 0,
                    };

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Paquete ObtenerTodaBasuraByidRecepcion(int idRecepcion)
        {
            Paquete Datos = new Paquete();

            string Query = "SELECT paquete.idrecepcion, paquete.fechaPesado, sum(peso) as pesototal FROM paquete " +
                "INNER JOIN Material ON paquete.idmaterial = Material.idMaterial " +
                "INNER JOIN TipoMaterial ON Material.idTipoMaterial = TipoMaterial.idTipoMaterial " +
                "WHERE paquete.idRecepcion = '" + idRecepcion + "' " +
                "AND material.idTipoMaterial = 2 ";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Paquete()
                    {
                        idRecepcion = ReaderBD["idrecepcion"] != DBNull.Value ? ReaderBD.GetInt32(0) : 0,
                        Peso = ReaderBD["pesototal"] != DBNull.Value ? ReaderBD.GetDouble(2) : 0,
                    };

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public List<Paquete> ObtenerPaquetesByAlmacen(int id)
        {
            List<Paquete> Lista = new List<Paquete>();

            //string Query = "SELECT idpaquete, peso, paquete.embarque, paquete.idAlmacen, almacen.nombreAlmacen, paquete.idrecepcion, paquete.idbascula, bascula.modelo, fechaPesado, paquete.operador, empleado.Nombre, paquete.idmaterial, material.nombreMaterial, paquete.descripcion, paquete.idproceso, proceso.nombreProceso, paquete.turno_idturno, turno.tipo, paquete.idMaquina, maquina.nombre, codigoPaq, paquete.Estatus, idPaqueteSuperior, material.color FROM paquete " +
            //"INNER JOIN recepcion ON paquete.idrecepcion = recepcion.idrecepcion " +
            //"INNER JOIN bascula ON paquete.idbascula = bascula.idbascula " +
            //"INNER JOIN empleado ON paquete.operador = empleado.idEmpleado " +
            //"INNER JOIN material ON paquete.idmaterial = material.idmaterial " +
            //"INNER JOIN proceso ON paquete.idproceso = proceso.idproceso " +
            //"INNER JOIN turno ON paquete.turno_idturno = turno.idturno " +
            //"INNER JOIN almacen ON paquete.idAlmacen = almacen.idalmacen " +
            //"INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
            //"WHERE paquete.idAlmacen = '" + id + "';";
            string Query = "CALL PAQUETES_ALMACENES(" + id + ");";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete(ReaderBD.GetInt32(0), ReaderBD.GetDouble(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7), ReaderBD.GetDateTime(8),
                        ReaderBD.GetInt32(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetString(13), ReaderBD.GetInt32(14), ReaderBD.GetString(15), ReaderBD.GetInt32(16),
                        ReaderBD.GetString(17), ReaderBD.GetInt32(18), ReaderBD.GetString(19), ReaderBD.GetString(20), ReaderBD.GetInt32(21), ReaderBD.GetString(22), ReaderBD.GetInt32(23), ReaderBD.GetString(24), ReaderBD.GetInt32(25), ReaderBD.GetDouble(26), ObtenerRecepcionRecicladoraById(ReaderBD.GetInt32(5)).Cliente));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public int AgregarPaquete(Paquete Datos)
        {

            string Query = string.Format("INSERT INTO paquete (peso, embarque, idAlmacen, idrecepcion, idbascula, fechaPesado, operador, idmaterial, descripcion, idproceso, turno_idturno, idMaquina, codigoPaq, idEstatus, idPaqueteSuperior, idProcesoProduccion, PesoSupRestante, Precio) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}');",
                Datos.Peso, Datos.Embarque, Datos.idAlmacen, Datos.idRecepcion, Datos.idBascula, Datos.FechaPesado, Datos.idOperador, Datos.idMaterial, Datos.Descripcion, Datos.idProceso, Datos.idTurno, Datos.idMaquina, Datos.CodigoPaquete, Datos.idEstatus, Datos.idPaqueteSuperior, Datos.idProcesoProduccion, Datos.PesoSuperiorRestante, Datos.Precio);
            return ActualizarDatos(Query);
        }

        public void ActualizarPaquete(Paquete Datos)
        {

            string Query = string.Format("UPDATE paquete SET peso = '{0}', embarque = '{1}', idAlmacen = '{2}', idrecepcion = '{3}', idbascula = '{4}', fechaPesado = '{5}', operador = '{6}', idmaterial = '{7}', descripcion = '{8}', idproceso = '{9}', turno_idturno = '{10}', idMaquina = '{11}', codigoPaq = '{12}', PesoSupRestante = '{13}', Precio = '{14}' WHERE idpaquete = '{15}';",
                Datos.Peso, Datos.Embarque, Datos.idAlmacen, Datos.idRecepcion, Datos.idBascula, Datos.FechaPesado, Datos.idOperador, Datos.idMaterial, Datos.Descripcion, Datos.idProceso, Datos.idTurno, Datos.idMaquina, Datos.CodigoPaquete, Datos.PesoSuperiorRestante, Datos.Precio, Datos.idPaquete);
            ActualizarDatos(Query);
        }

        public int ActualizarEstatusPaquete(Paquete Datos, int Estatus)
        {
            string Query = string.Format("UPDATE paquete SET idEstatus = '{0}' WHERE idpaquete = '{1}';",
                Estatus, Datos.idPaquete);
            return ActualizarDatos(Query);
        }

        public void ActualizarProcesoPaquete(Paquete Datos)
        {
            string Query = string.Format("UPDATE paquete SET idProceso = '{0}' WHERE idpaquete = '{1}';",
                Datos.idProceso, Datos.idPaquete);
            ActualizarDatos(Query);
        }

        public void EliminarPaquete(int id)
        {

            string Query = string.Format("DELETE FROM paquete WHERE idpaquete = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Proceso
        public List<Proceso> ObtenerProcesos()
        {
            List<Proceso> Lista = new List<Proceso>();

            string Query = "SELECT idproceso, nombreProceso, codigo, PrecioKG FROM proceso;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Proceso(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Proceso ObtenerProcesoById(int id)
        {
            Proceso Datos = new Proceso();

            string Query = "SELECT idproceso, nombreProceso, codigo, PrecioKG FROM proceso WHERE idproceso = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Proceso(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Proceso ObtenerProcesoByName(string Nombre)
        {
            Proceso Datos = new Proceso();

            string Query = "SELECT idproceso, nombreProceso, codigo, PrecioKG FROM proceso WHERE nombreProceso = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Proceso(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetDouble(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarProceso(Proceso Datos)
        {
            string Query = string.Format("INSERT INTO proceso (nombreProceso, codigo, PrecioKG) VALUES ('{0}', '{1}', '{2}');", Datos.NombreProceso, Datos.Codigo, Datos.PrecioKG);
            return ActualizarDatos(Query);
        }

        public void ActualizarProceso(Proceso Datos)
        {
            string Query = string.Format("UPDATE proceso SET nombreProceso = '{0}', codigo = '{1}', PrecioKG = '{2}' WHERE idproceso = '{3}';", Datos.NombreProceso, Datos.Codigo, Datos.PrecioKG, Datos.idProceso);
            ActualizarDatos(Query);
        }

        public void EliminarProceso(int id)
        {
            string Query = string.Format("DELETE FROM proceso WHERE idproceso = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Recepcion
        public List<Recepcion> ObtenerRecepciones()
        {
            List<Recepcion> Lista = new List<Recepcion>();

            string Query = "SELECT idrecepcion, embarque, recepcion.idalmacen, almacen.nombreAlmacen, recepcion.idmaterial, material.nombreMaterial, recepcion.idCliente, empresa.Nombre, recepcion.operador, EMP1.Nombre, recepcion.turno, turno.tipo, pesoPresentado, pesoInterno, Folio, fechaRecep, transportista, recepcion.idoperacionRecepcion, operacionRecepcion.nombreOperacion, recepcion.personaRecep, EMP2.Nombre, recepcion.descripcion, placas, chofer, recepcion.personaEntrega, EMP3.Nombre FROM recepcion " +
                "INNER JOIN almacen ON recepcion.idalmacen = almacen.idalmacen " +
                "INNER JOIN material ON recepcion.idmaterial = material.idmaterial " +
                "INNER JOIN empleado EMP1 ON recepcion.operador = EMP1.idEmpleado " +
                "INNER JOIN operacionRecepcion ON recepcion.idoperacionRecepcion = operacionRecepcion.idoperacionRecepcion " +
                "INNER JOIN empleado EMP2 ON recepcion.personaRecep = EMP2.idEmpleado " +
                "INNER JOIN empleado EMP3 ON recepcion.personaEntrega = EMP3.idEmpleado " +
                "INNER JOIN cliente ON recepcion.idCliente = cliente.idCliente " +
                "INNER JOIN empresa ON cliente.idEmpresa = empresa.idEmpresa " +
                "INNER JOIN turno ON recepcion.turno = turno.idturno;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Recepcion(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7),
                        ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetString(11), ReaderBD.GetDouble(12), ReaderBD.GetDouble(13), ReaderBD.GetString(14), ReaderBD.GetDateTime(15), ReaderBD.GetString(16),
                        ReaderBD.GetInt32(17), ReaderBD.GetString(18), ReaderBD.GetInt32(19), ReaderBD.GetString(20), ReaderBD.GetString(21), ReaderBD.GetString(22), ReaderBD.GetString(23), ReaderBD.GetInt32(24), ReaderBD.GetString(25)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Recepcion ObtenerRecepcionRecicladoraById(int id)
        {
            Recepcion Datos = new Recepcion();

            string Query = "SELECT idrecepcion, embarque, recepcion.idalmacen, almacen.nombreAlmacen, recepcion.idmaterial, material.nombreMaterial, recepcion.idCliente, empresa.Nombre, recepcion.operador, EMP1.Nombre, recepcion.turno, turno.tipo, pesoPresentado, pesoInterno, Folio, fechaRecep, transportista, recepcion.idoperacionRecepcion, operacionRecepcion.nombreOperacion, recepcion.personaRecep, EMP2.Nombre, recepcion.descripcion, placas, chofer, recepcion.personaEntrega, EMP3.Nombre FROM recepcion " +
                "INNER JOIN almacen ON recepcion.idalmacen = almacen.idalmacen " +
                "INNER JOIN material ON recepcion.idmaterial = material.idmaterial " +
                "INNER JOIN empleado EMP1 ON recepcion.operador = EMP1.idEmpleado " +
                "INNER JOIN operacionRecepcion ON recepcion.idoperacionRecepcion = operacionRecepcion.idoperacionRecepcion " +
                "INNER JOIN empleado EMP2 ON recepcion.personaRecep = EMP2.idEmpleado " +
                "INNER JOIN empleado EMP3 ON recepcion.personaEntrega = EMP3.idEmpleado " +
                "INNER JOIN cliente ON recepcion.idCliente = cliente.idCliente " +
                "INNER JOIN turno ON recepcion.turno = turno.idturno " +
                "INNER JOIN empresa ON cliente.idEmpresa = empresa.idEmpresa WHERE idrecepcion = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Recepcion(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetInt32(6), ReaderBD.GetString(7),
                        ReaderBD.GetInt32(8), ReaderBD.GetString(9), ReaderBD.GetInt32(10), ReaderBD.GetString(11), ReaderBD.GetDouble(12), ReaderBD.GetDouble(13), ReaderBD.GetString(14), ReaderBD.GetDateTime(15), ReaderBD.GetString(16),
                        ReaderBD.GetInt32(17), ReaderBD.GetString(18), ReaderBD.GetInt32(19), ReaderBD.GetString(20), ReaderBD.GetString(21), ReaderBD.GetString(22), ReaderBD.GetString(23), ReaderBD.GetInt32(24), ReaderBD.GetString(25));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarRecepcion(Recepcion Datos)
        {
            //Datos.FechaRecepcion = DateTime.Now.ToString("yyyy-MM-dd");
            Datos.idOperacionRecepcion = 1;
            string Query = string.Format("INSERT INTO recepcion (embarque, idalmacen, idmaterial, operador, turno, pesoPresentado, pesoInterno, Folio, fechaRecep, transportista, idoperacionRecepcion, personaRecep, descripcion, placas, chofer, personaEntrega, idCliente) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');",
                Datos.Embarque, Datos.idAlmacen, Datos.idMaterial, Datos.idOperador, Datos.idTurno, Datos.PesoPresentado, Datos.PesoInterno, Datos.Folio, Datos.FechaRecepcion, Datos.Transportista, Datos.idOperacionRecepcion, Datos.idPersonaRecepcion, Datos.Descripcion, Datos.Placas, Datos.Chofer, Datos.idPersonaEntrega, Datos.idCliente);
            return ActualizarDatos(Query);
        }

        public void ActualizarRecepcion(Recepcion Datos)
        {

            string Query = string.Format("UPDATE recepcion SET embarque = '{0}', idalmacen = '{1}', idmaterial = '{2}', operador = '{3}', turno = '{4}', pesoPresentado = '{5}', pesoInterno = '{6}', Folio = '{7}', transportista = '{8}', idoperacionRecepcion = '{9}', personaRecep = '{10}', descripcion = '{11}', placas = '{12}', chofer = '{13}', personaEntrega = '{14}', idCliente = '{15}' WHERE idrecepcion = '{16}';",
                Datos.Embarque, Datos.idAlmacen, Datos.idMaterial, Datos.idOperador, Datos.idTurno, Datos.PesoPresentado, Datos.PesoInterno, Datos.Folio, Datos.Transportista, Datos.idOperacionRecepcion, Datos.idPersonaRecepcion, Datos.Descripcion, Datos.Placas, Datos.Chofer, Datos.idPersonaEntrega, Datos.idCliente, Datos.idRecepcion);
            ActualizarDatos(Query);
        }

        public void ActualizarFechaRecepcion(Recepcion Datos)
        {

            string Query = string.Format("UPDATE recepcion SET fechaRecep = '{0}' WHERE idrecepcion = '{1}';",
                Datos.FechaRecepcion, Datos.idRecepcion);
            ActualizarDatos(Query);
        }

        public void EliminarRecepcionReci(int id)
        {

            string Query = string.Format("DELETE FROM recepcion WHERE idrecepcion = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region TipoMaquinas
        public List<TipoMaquinas> ObtenerTipoMaquinas()
        {
            List<TipoMaquinas> Lista = new List<TipoMaquinas>();

            string Query = "SELECT idTiposMaquina, tipo FROM tiposMaquina;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new TipoMaquinas(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public TipoMaquinas ObtenerTipoMaquinaById(int id)
        {
            TipoMaquinas Datos = new TipoMaquinas();

            string Query = "SELECT idTiposMaquina, tipo FROM tiposMaquina WHERE idTiposMaquina = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new TipoMaquinas(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public TipoMaquinas ObtenerTipoMaquinaByName(string Nombre)
        {
            TipoMaquinas Datos = new TipoMaquinas();

            string Query = "SELECT idTiposMaquina, tipo FROM tiposMaquina WHERE tipo = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new TipoMaquinas(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarTipoMaquina(TipoMaquinas Datos)
        {

            string Query = string.Format("INSERT INTO tiposMaquina (tipo) VALUES ('{0}');", Datos.TipoMaquina);
            return ActualizarDatos(Query);
        }

        public void ActualizarTipoMaquina(TipoMaquinas Datos)
        {

            string Query = string.Format("UPDATE tiposMaquina SET tipo = '{0}' WHERE idTiposMaquina = '{1}';", Datos.TipoMaquina, Datos.idTipoMaquina);
            ActualizarDatos(Query);
        }

        public void EliminarTipoMaquina(int id)
        {

            string Query = string.Format("DELETE FROM tiposMaquina WHERE idTiposMaquina = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Puestos
        public List<Puestos> ObtenerPuestos()
        {
            List<Puestos> Lista = new List<Puestos>();

            string Query = "SELECT idpuesto, puesto FROM puesto;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Puestos(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Puestos ObtenerPuestoById(int id)
        {
            Puestos Datos = new Puestos();

            string Query = "SELECT idpuesto, puesto FROM puesto WHERE idpuesto = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Puestos(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Puestos ObtenerPuestoByName(string Nombre)
        {
            Puestos Datos = new Puestos();

            string Query = "SELECT idpuesto, puesto FROM puesto WHERE puesto = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Puestos(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarPuesto(Puestos Datos)
        {

            string Query = string.Format("INSERT INTO puesto (puesto) VALUES ('{0}');", Datos.Puesto);
            return ActualizarDatos(Query);
        }

        public void ActualizarPuesto(Puestos Datos)
        {

            string Query = string.Format("UPDATE puesto SET puesto = '{0}' WHERE idpuesto = '{1}';", Datos.Puesto, Datos.idPuesto);
            ActualizarDatos(Query);
        }

        public void EliminarPuestos(int id)
        {

            string Query = string.Format("DELETE FROM puesto WHERE idpuesto = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region EstadoMaquina
        public List<EstadoMaquina> ObtenerEstadosMaquina()
        {
            List<EstadoMaquina> Lista = new List<EstadoMaquina>();

            string Query = "SELECT idestadoMaquina, nombre FROM estadoMaquina;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new EstadoMaquina(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public EstadoMaquina ObtenerEstadoMaquinaById(int id)
        {
            EstadoMaquina Datos = new EstadoMaquina();

            string Query = "SELECT idestadoMaquina, nombre FROM estadoMaquina WHERE idestadoMaquina = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new EstadoMaquina(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public EstadoMaquina ObtenerEstadoMaquinaByName(string Nombre)
        {
            EstadoMaquina Datos = new EstadoMaquina();

            string Query = "SELECT idestadoMaquina, nombre FROM estadoMaquina WHERE nombre = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new EstadoMaquina(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }
        
        public int AgregarEstadoMaquina(EstadoMaquina Datos)
        {
            if (VerificarExistencia("estadoMaquina", "nombre", Datos.NombreEstado))
                return 0;

            string Query = string.Format("INSERT INTO estadoMaquina (nombre) VALUES ('{0}');", Datos.NombreEstado);
            return ActualizarDatos(Query);
        }

        public void ActualizarEstadoMaquina(EstadoMaquina Datos)
        {

            string Query = string.Format("UPDATE estadoMaquina SET nombre = '{0}' WHERE idestadoMaquina = '{1}';", Datos.NombreEstado, Datos.idEstadoMaquina);
            ActualizarDatos(Query);
        }

        public void EliminarEstadoMaquina(int id)
        {

            string Query = string.Format("DELETE FROM estadoMaquina WHERE idestadoMaquina = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Grupos
        public List<Grupos> ObtenerGrupos()
        {
            List<Grupos> Lista = new List<Grupos>();

            string Query = "SELECT idgrupo, grupo FROM grupo;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Grupos(ReaderBD.GetInt32(0), ReaderBD.GetString(1)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Grupos ObtenerGrupoById(int id)
        {
            Grupos Datos = new Grupos();

            string Query = "SELECT idgrupo, grupo FROM grupo " +
                "WHERE idgrupo = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Grupos(ReaderBD.GetInt32(0), ReaderBD.GetString(1));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Grupos ObtenerGrupoCompleto(int id)
        {
            Grupos Datos = new Grupos();

            string Query = "SELECT idgrupo, grupo FROM grupo " +
                "WHERE idgrupo = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Grupos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ObtenerPuestosGrupoByIdGrupo(ReaderBD.GetInt32(0)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarGrupo(Grupos Datos)
        {

            string Query = string.Format("INSERT INTO grupo (grupo) VALUES ('{0}');", Datos.Grupo);
            return ActualizarDatos(Query);
        }

        public void ActualizarGrupo(Grupos Datos)
        {

            string Query = string.Format("UPDATE grupo SET grupo = '{0}' WHERE idgrupo = '{1}';", Datos.Grupo, Datos.idGrupo);
            ActualizarDatos(Query);
        }

        public void EliminarGrupo(int id)
        {
            string Query = string.Format("DELETE FROM grupo WHERE idgrupo = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Grupos Historicos
        public List<GruposHistoricos> ObtenerGruposHistoricos()
        {
            List<GruposHistoricos> Lista = new List<GruposHistoricos>();

            string Query = "SELECT idgrupoHistorico, fecha, grupo FROM grupoHistorico;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new GruposHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetString(2)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public GruposHistoricos ObtenerGrupoHistoricoById(int id)
        {
            GruposHistoricos Datos = new GruposHistoricos();

            string Query = "SELECT idgrupoHistorico, fecha, grupo FROM grupoHistorico " +
                "WHERE idgrupoHistorico = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new GruposHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetString(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public GruposHistoricos ObtenerGrupoHistoricoByFecha(DateTime fecha)
        {
            GruposHistoricos Datos = new GruposHistoricos();

            string Query = "SELECT idgrupoHistorico, fecha, grupo FROM grupoHistorico " +
                "WHERE fecha <= '" + fecha + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new GruposHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetString(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarGrupoHistorico(GruposHistoricos Datos)
        {
            string Query = string.Format("INSERT INTO grupoHistorico (fecha, grupo) VALUES ('{0}', '{1}');", Datos.Fecha, Datos.Grupo);
            return ActualizarDatos(Query);
        }

        public int AgregarGrupoHistoricoByGrupo(Grupos Datos)
        {
            int idGrupoHistorico = 0;

            string Query = string.Format("INSERT INTO grupoHistorico (fecha, grupo, idGrupo) VALUES ('{0}', '{1}', '{2}');", DateTime.Now.ToString("yyyy-MM-dd"), Datos.Grupo, Datos.idGrupo);
            idGrupoHistorico = ActualizarDatos(Query);

            return idGrupoHistorico;
        }

        public void ActualizarGrupoHistorico(GruposHistoricos Datos)
        {

            string Query = string.Format("UPDATE grupoHistorico SET fecha = '{0}', grupo = '{1}' WHERE idgrupoHistorico = '{2}';", Datos.Fecha, Datos.Grupo, Datos.idGrupoHistorico);
            ActualizarDatos(Query);
        }

        public void EliminarGrupoHistorico(int id)
        {

            string Query = string.Format("DELETE FROM grupoHistorico WHERE idgrupoHistorico = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region PuestosGrupo
        public List<PuestosGrupo> ObtenerPuestosGrupos()
        {
            List<PuestosGrupo> Lista = new List<PuestosGrupo>();

            string Query = "SELECT idPuestosGrupo, empleado_idEmpleado, empleado.Nombre, puesto_idPuesto, puesto.puesto, grupo_idgrupo FROM puestosGrupo " +
                "INNER JOIN empleado ON puestosGrupo.empleado_idEmpleado = empleado.idEmpleado " +
                "INNER JOIN puesto ON puestosGrupo.puesto_idpuesto = puesto.idpuesto " +
                "INNER JOIN grupo ON puestosGrupo.grupo_idgrupo = grupo.idgrupo;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new PuestosGrupo(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<PuestosGrupo> ObtenerPuestosGrupoByIdGrupo(int id)
        {
            List<PuestosGrupo> Datos = new List<PuestosGrupo>();

            string Query = "SELECT idPuestosGrupo, empleado_idEmpleado, empleado.Nombre, puesto_idPuesto, puesto.puesto, grupo_idgrupo FROM puestosGrupo " +
                "INNER JOIN empleado ON puestosGrupo.empleado_idEmpleado = empleado.idEmpleado " +
                "INNER JOIN puesto ON puestosGrupo.puesto_idpuesto = puesto.idpuesto " +
                "INNER JOIN grupo ON puestosGrupo.grupo_idgrupo = grupo.idgrupo " +
                "WHERE grupo_idgrupo = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.Add(new PuestosGrupo(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarPuestosGrupo(PuestosGrupo Datos)
        {

            string Query = string.Format("INSERT INTO puestosGrupo (empleado_idEmpleado, puesto_idPuesto, grupo_idgrupo) VALUES ('{0}', '{1}', '{2}');", Datos.idEmpleado, Datos.idPuesto, Datos.idGrupo);
            return ActualizarDatos(Query);
        }

        public void ActualizarPuestosGrupo(PuestosGrupo Datos)
        {

            string Query = string.Format("UPDATE puestosGrupo SET empleado_idEmpleado = '{0}', puesto_idPuesto = '{1}', grupo_idgrupo = '{2}' WHERE idPuestosGrupo = '{3}';", Datos.idEmpleado, Datos.idPuesto, Datos.idGrupo, Datos.idPuestoGrupo);
            ActualizarDatos(Query);
        }

        public void EliminarPuestosGrupo(int id)
        {

            string Query = string.Format("DELETE FROM puestosGrupo WHERE idPuestosGrupo = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region PuestosGrupoHistoricos
        public List<PuestosGrupoHistoricos> ObtenerPuestosGruposHistoricos()
        {
            List<PuestosGrupoHistoricos> Lista = new List<PuestosGrupoHistoricos>();

            string Query = "SELECT idpuestosgrupoHistorico, empleado_idEmpleado, empleado.Nombre, puesto_idPuesto, puesto.puesto, grupo_idgrupoHistorico FROM puestosgrupoHistorico " +
                "INNER JOIN empleado ON puestosgrupoHistorico.empleado_idEmpleado = empleado.idEmpleado " +
                "INNER JOIN puesto ON puestosgrupoHistorico.puesto_idpuesto = puesto.idpuesto " +
                "INNER JOIN grupoHistorico ON puestosgrupoHistorico.grupo_idgrupoHistorico = grupoHistorico.idgrupoHistorico;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new PuestosGrupoHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<PuestosGrupoHistoricos> ObtenerPuestosGrupoHistoricoByIdGrupoHistorico(int id)
        {
            List<PuestosGrupoHistoricos> Datos = new List<PuestosGrupoHistoricos>();

            string Query = "SELECT idpuestosgrupoHistorico, empleado_idEmpleado, empleado.Nombre, puesto_idPuesto, puesto.puesto, grupo_idgrupoHistorico FROM puestosgrupoHistorico " +
                "INNER JOIN empleado ON puestosgrupoHistorico.empleado_idEmpleado = empleado.idEmpleado " +
                "INNER JOIN puesto ON puestosgrupoHistorico.puesto_idpuesto = puesto.idpuesto " +
                "INNER JOIN grupoHistorico ON puestosgrupoHistorico.grupo_idgrupoHistorico = grupoHistorico.idgrupoHistorico " +
                "WHERE grupo_idgrupoHistorico = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.Add(new PuestosGrupoHistoricos(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetString(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarPuestosGrupoHistorico(PuestosGrupoHistoricos Datos)
        {
            string Query = string.Format("INSERT INTO puestosgrupoHistorico (empleado_idEmpleado, puesto_idPuesto, grupo_idgrupoHistorico) VALUES ('{0}', '{1}', '{2}');", Datos.idEmpleado, Datos.idPuesto, Datos.idGrupoHistorico);
            return ActualizarDatos(Query);
        }

        public int AgregarPuestosGrupoHistoricoByPuestosGrupo(PuestosGrupo Datos)
        {
            string Query = string.Format("INSERT INTO puestosgrupoHistorico (empleado_idEmpleado, puesto_idPuesto, grupo_idgrupoHistorico) VALUES ('{0}', '{1}', '{2}');", Datos.idEmpleado, Datos.idPuesto, Datos.idGrupo);
            return ActualizarDatos(Query);
        }

        public void ActualizarPuestosGrupoHistorico(PuestosGrupoHistoricos Datos)
        {

            string Query = string.Format("UPDATE puestosgrupoHistorico SET empleado_idEmpleado = '{0}', puesto_idPuesto = '{1}', grupo_idgrupoHistorico = '{2}' WHERE idpuestosgrupoHistorico = '{3}';", Datos.idEmpleado, Datos.idPuesto, Datos.idGrupoHistorico, Datos.idPuestoGrupo);
            ActualizarDatos(Query);
        }

        public void EliminarPuestosGrupoHistorico(int id)
        {

            string Query = string.Format("DELETE FROM puestosgrupoHistorico WHERE idpuestosgrupoHistorico = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Registros
        public List<Registro> ObtenerRegistros()
        {
            List<Registro> Lista = new List<Registro>();

            string Query = "SELECT idregistro, registro.fecha, pesoTotal, registro.idEmpleado, empleado.Nombre, registro.idMaquina, maquina.nombre FROM registro " +
                "INNER JOIN empleado ON registro.idEmpleado = empleado.idEmpleado " +
                "INNER JOIN maquina ON registro.idMaquina = maquina.idMaquina;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Registro(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetDouble(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Registro> ObtenerRegistrosCompletos()
        {
            List<Registro> Lista = new List<Registro>();

            string Query = "SELECT idregistro, registro.fecha, pesoTotal, registro.idEmpleado, empleado.Nombre, registro.idMaquina, maquina.nombre FROM registro " +
                "INNER JOIN empleado ON registro.idEmpleado = empleado.idEmpleado " +
                "INNER JOIN maquina ON registro.idMaquina = maquina.idMaquina;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Registro(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetDouble(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5),
                        ReaderBD.GetString(6), ObtenerListaRegistroByIdRegistro(ReaderBD.GetInt32(0))));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Registro> ObtenerRegistroByIdEmpleado(int id)
        {
            List<Registro> Datos = new List<Registro>();

            string Query = "SELECT idregistro, registro.fecha, pesoTotal, registro.idEmpleado, empleado.Nombre, registro.idMaquina, maquina.nombre FROM registro " +
                "INNER JOIN empleado ON registro.idEmpleado = empleado.idEmpleado " +
                "INNER JOIN maquina ON registro.idMaquina = maquina.idMaquina " +
                "WHERE registro.idEmpleado = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.Add(new Registro(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetDouble(2), ReaderBD.GetInt32(3), ReaderBD.GetString(4), ReaderBD.GetInt32(5), ReaderBD.GetString(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarRegistro(Registro Datos)
        {

            string Query = string.Format("INSERT INTO registro (fecha, pesoTotal, idEmpleado, idMaquina) VALUES ('{0}', '{1}', '{2}', '{3}');", Datos.Fecha, Datos.PesoTotal, Datos.idEmpleado, Datos.idMaquina);
            return ActualizarDatos(Query);
        }

        public void ActualizarRegistro(Registro Datos)
        {

            string Query = string.Format("UPDATE registro SET fecha = '{0}', pesoTotal = '{1}', idEmpleado = '{2}', idMaquina = '{3}' WHERE idregistro = '{4}';", Datos.Fecha, Datos.PesoTotal, Datos.idEmpleado, Datos.idMaquina, Datos.idRegistro);
            ActualizarDatos(Query);
        }

        public void EliminarRegistro(int id)
        {

            string Query = string.Format("DELETE FROM registro WHERE idregistro = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region ListaRegistro
        public List<ListaRegistro> ObtenerListaRegistros()
        {
            List<ListaRegistro> Lista = new List<ListaRegistro>();

            string Query = "SELECT idlistaRegistro, listaRegistro.idregistro, listaRegistro.idmaterial, material.nombreMaterial, material.color, codigo, peso FROM listaRegistro " +
                "INNER JOIN registro ON listaRegistro.idregistro = registro.idregistro " +
                "INNER JOIN material ON listaRegistro.idmaterial = material.idmaterial;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new ListaRegistro(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5), ReaderBD.GetDouble(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<ListaRegistro> ObtenerListaRegistroByIdRegistro(int id)
        {
            List<ListaRegistro> Datos = new List<ListaRegistro>();

            string Query = "SELECT idlistaRegistro, listaRegistro.idregistro, listaRegistro.idmaterial, material.nombreMaterial, material.color, codigo, peso FROM listaRegistro " +
                "INNER JOIN registro ON listaRegistro.idregistro = registro.idregistro " +
                "INNER JOIN material ON listaRegistro.idmaterial = material.idmaterial " +
                "WHERE listaRegistro.idregistro = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos.Add(new ListaRegistro(ReaderBD.GetInt32(0), ReaderBD.GetInt32(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetString(5), ReaderBD.GetDouble(6)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarListaRegistro(ListaRegistro Datos)
        {

            string Query = string.Format("INSERT INTO listaRegistro (idregistro, idmaterial, codigo, peso) VALUES ('{0}', '{1}', '{2}', '{3}');", Datos.idRegistro, Datos.idMaterial, Datos.Codigo, Datos.Peso);
            return ActualizarDatos(Query);
        }

        public void ActualizarListaRegistro(ListaRegistro Datos)
        {

            string Query = string.Format("UPDATE listaRegistro SET idregistro = '{0}', idmaterial = '{1}', codigo = '{2}', peso = '{3}' WHERE idlistaRegistro = '{4}';", Datos.idRegistro, Datos.idMaterial, Datos.Codigo, Datos.Peso, Datos.idListaRegistro);
            ActualizarDatos(Query);
        }

        public void EliminarListaRegistro(int id)
        {

            string Query = string.Format("DELETE FROM listaRegistro WHERE idlistaRegistro = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Maquinas
        public List<Maquinas> ObtenerMaquinas()
        {
            List<Maquinas> Lista = new List<Maquinas>();

            string Query = "SELECT idMaquina, maquina.nombre, ultimoMantenimiento, modelo, marca, fechaAdquisicion, descripcion, maquina.idTiposMaquina, tiposMaquina.tipo, maquina.idestadoMaquina, estadoMaquina.nombre FROM maquina " + 
                "INNER JOIN tiposMaquina ON maquina.idTiposMaquina = tiposMaquina.idTiposMaquina " +
                "INNER JOIN estadoMaquina ON maquina.idestadoMaquina = estadoMaquina.idestadoMaquina;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Maquinas(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetDateTime(5), ReaderBD.GetString(6), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Maquinas ObtenerMaquinaById(int id)
        {
            Maquinas Datos = new Maquinas();

            string Query = "SELECT idMaquina, maquina.nombre, ultimoMantenimiento, modelo, marca, fechaAdquisicion, descripcion, maquina.idTiposMaquina, tiposMaquina.tipo, maquina.idestadoMaquina, estadoMaquina.nombre FROM maquina " +
                "INNER JOIN tiposMaquina ON maquina.idTiposMaquina = tiposMaquina.idTiposMaquina " +
                "INNER JOIN estadoMaquina ON maquina.idestadoMaquina = estadoMaquina.idestadoMaquina " +
                "WHERE idMaquina = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Maquinas(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetDateTime(5), ReaderBD.GetString(6), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Maquinas ObtenerMaquinaByName(string Nombre)
        {
            Maquinas Datos = new Maquinas();

            string Query = "SELECT idMaquina, maquina.nombre, ultimoMantenimiento, modelo, marca, fechaAdquisicion, descripcion, maquina.idTiposMaquina, tiposMaquina.tipo, maquina.idestadoMaquina, estadoMaquina.nombre FROM maquina " +
                "INNER JOIN tiposMaquina ON maquina.idTiposMaquina = tiposMaquina.idTiposMaquina " +
                "INNER JOIN estadoMaquina ON maquina.idestadoMaquina = estadoMaquina.idestadoMaquina " +
                "WHERE maquina.nombre = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Maquinas(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetDateTime(5), ReaderBD.GetString(6), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public List<Maquinas> ObtenerMaquinasByProceso(int id)
        {
            List<Maquinas> Lista = new List<Maquinas>();

            string Query = "SELECT idMaquina, maquina.nombre, ultimoMantenimiento, modelo, marca, fechaAdquisicion, descripcion, maquina.idTiposMaquina, tiposMaquina.tipo, maquina.idestadoMaquina, estadoMaquina.nombre FROM maquina " +
                "INNER JOIN tiposMaquina ON maquina.idTiposMaquina = tiposMaquina.idTiposMaquina " +
                "INNER JOIN estadoMaquina ON maquina.idestadoMaquina = estadoMaquina.idestadoMaquina " +
                "WHERE maquina.idProceso = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Maquinas(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetString(3), ReaderBD.GetString(4), ReaderBD.GetDateTime(5), ReaderBD.GetString(6), ReaderBD.GetInt32(7), ReaderBD.GetString(8), ReaderBD.GetInt32(9), ReaderBD.GetString(10)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public int AgregarMaquina(Maquinas Datos)
        {

            string Query = string.Format("INSERT INTO maquina (nombre, ultimoMantenimiento, modelo, marca, fechaAdquisicion, descripcion, idTiposMaquina, idestadoMaquina, idProceso) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');",
                Datos.Maquina, Datos.UltimoMantenimiento, Datos.Modelo, Datos.Marca, Datos.FechaAdquisicion, Datos.Descripcion, Datos.idTipoMaquina, Datos.idEstadoMaquina, Datos.idProceso);
            return ActualizarDatos(Query);
        }

        public void ActualizarMaquina(Maquinas Datos)
        {

            string Query = string.Format("UPDATE maquina SET nombre = '{0}', ultimoMantenimiento = '{1}', modelo = '{2}', marca = '{3}', fechaAdquisicion = '{4}', descripcion = '{5}', idTiposMaquina = '{6}', idestadoMaquina = '{7}' WHERE idMaquina = '{8}';",
                Datos.Maquina, Datos.UltimoMantenimiento, Datos.Modelo, Datos.Marca, Datos.FechaAdquisicion, Datos.Descripcion, Datos.idTipoMaquina, Datos.idEstadoMaquina, Datos.idMaquina);
            ActualizarDatos(Query);
        }

        public void EliminarMaquina(int id)
        {

            string Query = string.Format("DELETE FROM maquina WHERE idMaquina = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Turnos
        public List<Turnos> ObtenerTurnos()
        {
            List<Turnos> Lista = new List<Turnos>();

            string Query = "SELECT idturno, tipo, horaInicio, horaFinal FROM turno;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Turnos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetDateTime(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Turnos> ObtenerTurnosSinDescanso()
        {
            List<Turnos> Lista = new List<Turnos>();

            string Query = "SELECT idturno, tipo, horaInicio, horaFinal FROM turno WHERE tipo != 'Descanso';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Turnos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetDateTime(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Turnos ObtenerTurnoById(int id)
        {
            Turnos Datos = new Turnos();

            string Query = "SELECT idturno, tipo, horaInicio, horaFinal FROM turno WHERE idturno = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Turnos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetDateTime(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }
        /*
        public Turnos ObtenerTurnoByHora()
        {
            Turnos Datos = new Turnos();
            DateTime HoraActual = DateTime.Now;
            HoraActual.da
            string Query = "SELECT idturno, tipo, horaInicio, horaFinal FROM turno WHERE idturno = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Turnos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetDateTime(3));

            Cerrar_Conexion();
            return Datos;
        }
        */
        public Turnos ObtenerTurnoByName(string Nombre)
        {
            Turnos Datos = new Turnos();

            string Query = "SELECT idturno, tipo, horaInicio, horaFinal FROM turno WHERE tipo = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Turnos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetDateTime(2), ReaderBD.GetDateTime(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarTurno(Turnos Datos)
        {
            DateTime Hoy = DateTime.Now;
            
            string Query = string.Format("INSERT INTO turno (tipo, horaInicio, horaFinal) VALUES ('{0}', '{1}', '{2}');", Datos.Turno, Hoy.ToString("yyyy-MM-dd") + " " + Datos.HoraInicio, Hoy.ToString("yyyy-MM-dd") + " " + Datos.HoraFinal);
            return ActualizarDatos(Query);
        }

        public void ActualizarTurno(Turnos Datos)
        {
            DateTime Hoy = DateTime.Now;

            string Query = string.Format("UPDATE turno SET tipo = '{0}', horaInicio = '{1}', horaFinal = '{2}' WHERE idturno = '{3}';", Datos.Turno, Hoy.ToString("yyyy-MM-dd") + " " + Datos.HoraInicio, Hoy.ToString("yyyy-MM-dd") + " " + Datos.HoraFinal, Datos.idTurno);
            ActualizarDatos(Query);
        }

        public void EliminarTurno(int id)
        {

            string Query = string.Format("DELETE FROM turno WHERE idturno = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Turnos Rotatativos
        public List<TurnosRotativos> ObtenerTurnosRotatativos()
        {
            List<TurnosRotativos> Lista = new List<TurnosRotativos>();

            string Query = "SELECT idturnoRotativo, dia, turnosrotativos.idTurno, turno.tipo, turnosrotativos.idGrupo, grupo.grupo FROM turnosrotativos " +
                "INNER JOIN turno ON turnosrotativos.idTurno = turno.idturno " +
                "INNER JOIN grupo ON turnosrotativos.idGrupo = grupo.idgrupo;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new TurnosRotativos(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4),ReaderBD.GetString(5)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public TurnosRotativos ObtenerTurnoRotatativoById(int id)
        {
            TurnosRotativos Datos = new TurnosRotativos();

            string Query = "SELECT idturnoRotativo, dia, turnosrotativos.idTurno, turno.tipo, turnosrotativos.idGrupo, grupo.grupo FROM turnosrotativos " +
                "INNER JOIN turno ON turnosrotativos.idTurno = turno.idturno " +
                "INNER JOIN grupo ON turnosrotativos.idGrupo = grupo.idgrupo " +
                "WHERE idturnoRotativo = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new TurnosRotativos(ReaderBD.GetInt32(0), ReaderBD.GetDateTime(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarTurnoRotatativo(TurnosRotativos Datos)
        {
            //DateTime Hoy = DateTime.Now;
            string Query = string.Format("INSERT INTO turnosrotativos (dia, idTurno, idGrupo) VALUES ('{0}', '{1}', '{2}');", Datos.Dia, Datos.idTurno, Datos.idGrupo);
            return ActualizarDatos(Query);
        }

        public void ActualizarTurnoRotatativo(TurnosRotativos Datos)
        {
            DateTime Hoy = DateTime.Now;

            string Query = string.Format("UPDATE turnosrotativos SET dia = '{0}', idTurno = '{1}', idGrupo = '{2}' WHERE idturnoRotativo = '{3}';", Datos.Dia, Datos.idTurno, Datos.idGrupo, Datos.idTurnoRotativo);
            ActualizarDatos(Query);
        }

        public void EliminarTurnoRotatativo(int id)
        {
            string Query = string.Format("DELETE FROM turnosrotativos WHERE idturnoRotativo = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Insumos
        public List<Insumos> ObtenerInsumos()
        {
            List<Insumos> Lista = new List<Insumos>();

            string Query = "SELECT idInsumo, Insumo, Cantidad, Descripcion FROM Insumos;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Insumos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Insumos ObtenerInsumoById(int id)
        {
            Insumos Datos = new Insumos();

            string Query = "SELECT idInsumo, Insumo, Cantidad, Descripcion FROM Insumos WHERE idInsumo = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Insumos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Insumos ObtenerInsumoByName(string Nombre)
        {
            Insumos Datos = new Insumos();

            string Query = "SELECT idInsumo, Insumo, Cantidad, Descripcion FROM Insumos WHERE Insumo = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Insumos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2), ReaderBD.GetString(3));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public int AgregarInsumos(Insumos Datos)
        {

            string Query = string.Format("INSERT INTO Insumos (Insumo, Cantidad, Descripcion) VALUES ('{0}', '{1}', '{2}');", Datos.Insumo, Datos.Cantidad, Datos.Descripcion);
            return ActualizarDatos(Query);
        }

        public void ActualizarInsumos(Insumos Datos)
        {

            string Query = string.Format("UPDATE Insumos SET Insumo = '{0}', Cantidad = '{1}', Descripcion = '{2}' WHERE idInsumo = '{3}';", Datos.Insumo, Datos.Cantidad, Datos.Descripcion, Datos.idInsumo);
            ActualizarDatos(Query);
        }

        public void EliminarInsumos(int id)
        {

            string Query = string.Format("DELETE FROM Insumos WHERE idInsumo = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Pedidos
        public List<Pedidos> ObtenerPedidos()
        {
            List<Pedidos> Lista = new List<Pedidos>();

            string Query = "SELECT idPedidos, Folio, Pedidos.idCliente, Empresa.Nombre, Pedidos.idProcesoFinal, proceso.nombreProceso, FechaEntrega, PorcentajeProcesos, PorcentajeEntrega, FechaEntregado, Descripcion, Pedidos.idEmpleado, empleado.Nombre, FechaPedido FROM Pedidos " +
                "INNER JOIN proceso ON Pedidos.idProcesoFinal = proceso.idproceso " +
                "INNER JOIN empleado ON Pedidos.idEmpleado = empleado.idEmpleado " +
                "INNER JOIN cliente ON Pedidos.idCliente = cliente.idCliente " +
                "INNER JOIN empresa ON cliente.idEmpresa = empresa.idEmpresa;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Pedidos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6), ReaderBD.GetDouble(7),
                        ReaderBD.GetDouble(8), ReaderBD.GetDateTime(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetDateTime(13)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Pedidos ObtenerPedidoById(int id)
        {
            Pedidos Datos = new Pedidos();

            string Query = "SELECT idPedidos, Folio, Pedidos.idCliente, Empresa.Nombre, Pedidos.idProcesoFinal, proceso.idproceso, FechaEntrega, PorcentajeProcesos, PorcentajeEntrega, FechaEntregado, Descripcion, Pedidos.idEmpleado, empleado.Nombre, FechaPedido FROM Pedidos " +
                "INNER JOIN proceso ON Pedidos.idProcesoFinal = proceso.idproceso " +
                "INNER JOIN empleado ON Pedidos.idEmpleado = empleado.idEmpleado " +
                "INNER JOIN cliente ON Pedidos.idCliente = cliente.idCliente " +
                "INNER JOIN empresa ON cliente.idEmpresa = empresa.idEmpresa " +
                "WHERE idPedidos = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Pedidos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6), ReaderBD.GetDouble(7),
                        ReaderBD.GetDouble(8), ReaderBD.GetDateTime(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetDateTime(13));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Pedidos ObtenerPedidoByIdRecepcion(int id)
        {
            Pedidos Datos = new Pedidos();

            string Query = "SELECT idPedidos, Folio, Pedidos.idCliente, Empresa.Nombre, Pedidos.idProcesoFinal, proceso.idproceso, FechaEntrega, PorcentajeProcesos, PorcentajeEntrega, FechaEntregado, Descripcion, Pedidos.idEmpleado, empleado.Nombre, FechaPedido FROM Pedidos " +
                "INNER JOIN proceso ON Pedidos.idProcesoFinal = proceso.idproceso " +
                "INNER JOIN empleado ON Pedidos.idEmpleado = empleado.idEmpleado " +
                "INNER JOIN cliente ON Pedidos.idCliente = cliente.idCliente " +
                "INNER JOIN empresa ON cliente.idEmpresa = empresa.idEmpresa " +
                "WHERE Pedidos.IdRecepcion = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Pedidos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6), ReaderBD.GetDouble(7),
                        ReaderBD.GetDouble(8), ReaderBD.GetDateTime(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetDateTime(13));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public List<Pedidos> ObtenerPedidosByCliente(int idCliente)
        {
            List<Pedidos> Lista = new List<Pedidos>();

            string Query = "SELECT idPedidos, Folio, Pedidos.idCliente, Empresa.Nombre, Pedidos.idProcesoFinal, proceso.idproceso, FechaEntrega, PorcentajeProcesos, PorcentajeEntrega, FechaEntregado, Descripcion, Pedidos.idEmpleado, empleado.Nombre, FechaPedido FROM Pedidos " +
                "INNER JOIN proceso ON Pedidos.idProcesoFinal = proceso.idproceso " +
                "INNER JOIN empleado ON Pedidos.idEmpleado = empleado.idEmpleado " +
                "INNER JOIN cliente ON Pedidos.idCliente = cliente.idCliente " +
                "INNER JOIN empresa ON cliente.idEmpresa = empresa.idEmpresa " +
                "WHERE Pedidos.idCliente = '" + idCliente + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Pedidos(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetInt32(2), ReaderBD.GetString(3), ReaderBD.GetInt32(4), ReaderBD.GetString(5), ReaderBD.GetDateTime(6), ReaderBD.GetDouble(7),
                        ReaderBD.GetDouble(8), ReaderBD.GetDateTime(9), ReaderBD.GetString(10), ReaderBD.GetInt32(11), ReaderBD.GetString(12), ReaderBD.GetDateTime(13)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Pedidos> ObtenerPedidosDisponibles()
        {
            List<Pedidos> Lista = new List<Pedidos>();

            string Query = "SELECT idPedidos, Folio, Pedidos.idCliente, Empresa.Nombre FROM Pedidos " +
                "INNER JOIN cliente ON Pedidos.idCliente = cliente.idCliente " +
                "INNER JOIN empresa ON cliente.idEmpresa = empresa.idEmpresa " +
                "WHERE Disponible = '1';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Pedidos() {idPedidos = ReaderBD.GetInt32(0), Folio=ReaderBD.GetString(1), idCliente = ReaderBD.GetInt32(2), Cliente = ReaderBD.GetString(3) });

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public int AgregarPedido(Pedidos Datos)
        {

            string Query = string.Format("INSERT INTO Pedidos (Folio, idCliente, idProcesoFinal, FechaEntrega, PorcentajeProcesos, PorcentajeEntrega, FechaEntregado, Descripcion, idEmpleado, FechaPedido) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
                Datos.Folio, Datos.idCliente, Datos.idProcesoFinal, Datos.FechaEntrega, 0, 0, "2020-01-01", Datos.Descripcion, Datos.idEmpleado, Datos.FechaPedido);
            return ActualizarDatos(Query);
        }

        public void ActualizarPedido(Pedidos Datos)
        {
            string Query = string.Format("UPDATE Pedidos SET Folio = '{0}', idCliente = '{1}', idProcesoFinal = '{2}', FechaEntrega = '{3}', PorcentajeProcesos = '{4}', PorcentajeEntrega = '{5}', FechaEntregado = '{6}', Descripcion = '{7}' WHERE idPedidos = '{8}';",
                Datos.Folio, Datos.idCliente, Datos.idProcesoFinal, Datos.FechaEntrega, Datos.PorcentajeProcesos, Datos.PorcentajeEntrega, Datos.FechaEntregado, Datos.Descripcion, Datos.idPedidos);
            ActualizarDatos(Query);
        }

        public void AsignarRecepcionaPedido(int idRecepcion, int idPedido)
        {

            string Query = string.Format("UPDATE Pedidos SET idRecepcion = '{0}', Disponible = '{1}' WHERE idPedidos = '{2}';",
                idRecepcion, 0, idPedido);
            ActualizarDatos(Query);
        }

        public void ActualizarProgresoPedido(double Porcentaje, int idPedido)
        {
            string Query = string.Format("UPDATE Pedidos SET PorcentajeProcesos = '{0}' WHERE idPedidos = '{1}';",
                Porcentaje, idPedido);
            ActualizarDatos(Query);
        }

        public void EliminarPedido(int id)
        {

            string Query = string.Format("DELETE FROM Pedidos WHERE idPedidos = '{0}';", id);
            ActualizarDatos(Query);
        }
        #endregion

        #region Reportes
        public List<Paquete> ObtenerProduccion1Dia(string fecha)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT paquete.idProcesoProduccion, sum(peso), paquete.idMaquina, maquina.nombre, proceso.nombreproceso, tipomaterial.tipomaterial FROM paquete " +
                "INNER JOIN proceso ON paquete.idProcesoProduccion = proceso.idproceso " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN material ON paquete.idMaterial = material.idMaterial " +
                "INNER JOIN tipomaterial ON material.idtipomaterial = tipomaterial.idtipomaterial " +
                "WHERE fechaPesado LIKE '" + fecha + "%' " +
                "GROUP BY paquete.idMaquina, material.idtipomaterial, paquete.idProcesoProduccion;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete() { idProcesoProduccion = ReaderBD.GetInt32(0), Peso = ReaderBD.GetDouble(1), idMaquina = ReaderBD.GetInt32(2), Maquina = ReaderBD.GetString(3), NombreProceso = ReaderBD.GetString(4), NombreMaterial = ReaderBD.GetString(5) });

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerProduccion1DiaByProceso(int idProcesoProduccion, string fecha)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT paquete.idProcesoProduccion, sum(peso), paquete.idMaquina, maquina.nombre, proceso.nombreproceso, tipomaterial.tipomaterial FROM paquete " +
                "INNER JOIN proceso ON paquete.idProcesoProduccion = proceso.idproceso " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN material ON paquete.idMaterial = material.idMaterial " +
                "INNER JOIN tipomaterial ON material.idtipomaterial = tipomaterial.idtipomaterial " +
                "WHERE fechaPesado LIKE '" + fecha + "%' " + 
                "AND paquete.idProcesoProduccion = '" + idProcesoProduccion + "' " +
                "GROUP BY paquete.idMaquina, material.idtipomaterial;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete() { idProcesoProduccion = ReaderBD.GetInt32(0), Peso = ReaderBD.GetDouble(1), idMaquina = ReaderBD.GetInt32(2), Maquina = ReaderBD.GetString(3), NombreProceso = ReaderBD.GetString(4), NombreMaterial = ReaderBD.GetString(5) });

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerProduccionVariosDias(string fechaInicio, string fechaFin)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT paquete.idProcesoProduccion, sum(peso), paquete.idMaquina, maquina.nombre, proceso.nombreproceso, tipomaterial.tipomaterial FROM paquete " +
                "INNER JOIN proceso ON paquete.idProcesoProduccion = proceso.idproceso " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN material ON paquete.idMaterial = material.idMaterial " +
                "INNER JOIN tipomaterial ON material.idtipomaterial = tipomaterial.idtipomaterial " +
                "WHERE fechaPesado > '" + fechaInicio + "' " +
                "AND fechaPesado < '" + fechaFin + "' " +
                "GROUP BY paquete.idMaquina, material.idtipomaterial, paquete.idProcesoProduccion;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete() { idProcesoProduccion = ReaderBD.GetInt32(0), Peso = ReaderBD.GetDouble(1), idMaquina = ReaderBD.GetInt32(2), Maquina = ReaderBD.GetString(3), NombreProceso = ReaderBD.GetString(4), NombreMaterial = ReaderBD.GetString(5) });

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public List<Paquete> ObtenerProduccionVariosDiasByProceso(int idProcesoProduccion, string fechaInicio, string fechaFin)
        {
            List<Paquete> Lista = new List<Paquete>();

            string Query = "SELECT paquete.idProcesoProduccion, sum(peso), paquete.idMaquina, maquina.nombre, proceso.nombreproceso, tipomaterial.tipomaterial FROM paquete " +
                "INNER JOIN proceso ON paquete.idProcesoProduccion = proceso.idproceso " +
                "INNER JOIN maquina ON paquete.idMaquina = maquina.idMaquina " +
                "INNER JOIN material ON paquete.idMaterial = material.idMaterial " +
                "INNER JOIN tipomaterial ON material.idtipomaterial = tipomaterial.idtipomaterial " +
                "WHERE fechaPesado > '" + fechaInicio + "' " +
                "AND fechaPesado < '" + fechaFin + "' " +
                "AND paquete.idProcesoProduccion = '" + idProcesoProduccion + "' " +
                "GROUP BY paquete.idMaquina, material.idtipomaterial;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Paquete() { idProcesoProduccion = ReaderBD.GetInt32(0), Peso = ReaderBD.GetDouble(1), idMaquina = ReaderBD.GetInt32(2), Maquina = ReaderBD.GetString(3), NombreProceso = ReaderBD.GetString(4), NombreMaterial = ReaderBD.GetString(5) });

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public byte[] CrearReporteProduccionDiaPDF(int idProceso, DateTime sinicio)
        {
            byte[] pdf = new byte[0];

            string inicio = sinicio.ToString("dd/MM/yyyy");
            List<Paquete> Reportes = new List<Paquete>();
            if (idProceso > 0)
                Reportes = ObtenerProduccion1DiaByProceso(idProceso, sinicio.ToString("yyyy-MM-dd"));
            else
                Reportes = ObtenerProduccion1Dia(sinicio.ToString("yyyy-MM-dd"));

            int j = 0;
            if (Reportes != null)
            {
                byte[] plantilla = Properties.Resources.reporte;
                double total = 0;
                int n = Reportes.Count;
                MemoryStream dest = new MemoryStream();
                PdfReader reader = new PdfReader(plantilla);

                PdfStamper stamper = new PdfStamper(reader, dest);

                AcroFields form = stamper.AcroFields;
                stamper.FormFlattening = true;

                iTextSharp.text.Rectangle rect = stamper.AcroFields.GetFieldPositions("Logotipo")[0].position;
                int page = form.GetFieldPositions("Logotipo")[0].page;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.logotipodragnet, System.Drawing.Imaging.ImageFormat.Png);
                img.ScaleAbsolute(rect.Width, rect.Height);
                img.SetAbsolutePosition(rect.Left, rect.Bottom);
                stamper.GetOverContent(page).AddImage(img);


                form.SetField("fecha", inicio);

                string formapago = "";

                for (int i = 0; i < 25 && j < n; i++, j++)
                {
                    Paquete recibo = Reportes[j];
                    form.SetField("n" + i, (j + 1).ToString());
                    form.SetField("recibo" + i, recibo.NombreProceso);
                    formapago = recibo.Maquina;
                    form.SetField("formapago" + i, formapago);
                    form.SetField("tipomaterial" + i, recibo.NombreMaterial);
                    form.SetField("subtotal" + i, recibo.Peso.ToString());
                    total += Convert.ToInt32(recibo.Peso);
                }


                if (j == n)
                {
                    form.SetField("total", "Total   " + total.ToString());
                }
                stamper.Close();
                pdf = dest.ToArray();
                reader.Close();

            }
            return pdf;
        }

        public byte[] CrearReporteProduccionDiaRangoFechasPDF(int idProceso, DateTime sinicio, DateTime sfin)
        {
            byte[] pdf = new byte[0];

            string inicio = sinicio.ToString("dd/MM/yyyy");
            string fin = sfin.ToString("dd/MM/yyyy");
            List<Paquete> Reportes = new List<Paquete>();
            if (idProceso > 0)
                Reportes = ObtenerProduccionVariosDiasByProceso(idProceso, sinicio.ToString("yyyy-MM-dd 00:00:00"), sfin.ToString("yyyy-MM-dd 11:59:59"));
            else
                Reportes = ObtenerProduccionVariosDias(sinicio.ToString("yyyy-MM-dd 00:00:00"), sfin.ToString("yyyy-MM-dd 11:59:59"));

            int j = 0;
            if (Reportes != null)
            {
                byte[] plantilla = Properties.Resources.reporte;
                double total = 0;
                int n = Reportes.Count;
                MemoryStream dest = new MemoryStream();
                PdfReader reader = new PdfReader(plantilla);

                PdfStamper stamper = new PdfStamper(reader, dest);

                AcroFields form = stamper.AcroFields;
                stamper.FormFlattening = true;

                iTextSharp.text.Rectangle rect = stamper.AcroFields.GetFieldPositions("Logotipo")[0].position;
                int page = form.GetFieldPositions("Logotipo")[0].page;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.logotipodragnet, System.Drawing.Imaging.ImageFormat.Png);
                img.ScaleAbsolute(rect.Width, rect.Height);
                img.SetAbsolutePosition(rect.Left, rect.Bottom);
                stamper.GetOverContent(page).AddImage(img);

                string RangoFechas = inicio + " - " + fin;
                form.SetField("fecha", RangoFechas);

                string formapago = "";

                for (int i = 0; i < 25 && j < n; i++, j++)
                {
                    Paquete recibo = Reportes[j];
                    form.SetField("n" + i, (j + 1).ToString());
                    form.SetField("recibo" + i, recibo.NombreProceso);
                    formapago = recibo.Maquina;
                    form.SetField("formapago" + i, formapago);
                    form.SetField("tipomaterial" + i, recibo.NombreMaterial);
                    form.SetField("subtotal" + i, recibo.Peso.ToString());
                    total += Convert.ToInt32(recibo.Peso);
                }


                if (j == n)
                {
                    form.SetField("total", "Total   " + total.ToString());
                }
                stamper.Close();
                pdf = dest.ToArray();
                reader.Close();

            }
            return pdf;
        }

        public byte[] CrearReporteRecepcionPDF(List<Paquete> Historial, int idRecepcion)
        {
            byte[] pdf = new byte[0];

            string inicio = DateTime.Now.ToString("dd/MM/yyyy");

            int j = 0;
            if (Historial != null)
            {
                //string imagen;
                double total = 0;
                int n = Historial.Count;
                int paginas = n / 25 + 1;
                MemoryStream dest = new MemoryStream();

                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                PdfCopy cp = new PdfCopy(doc, dest);
                doc.Open();

                for (int l = 0; l < paginas; l++)
                {
                    byte[] plantilla = Properties.Resources.reporteHistorialRecepcion;
                    PdfReader reader = new PdfReader(plantilla);
                    MemoryStream paginaR = new MemoryStream();
                    PdfStamper stamper = new PdfStamper(reader, paginaR);
                    AcroFields form = stamper.AcroFields;
                    stamper.FormFlattening = true;

                    //imagen = "Logotipos/logotipodragnet.png";

                    //if (File.Exists(imagen))
                    //{
                    iTextSharp.text.Rectangle rect = stamper.AcroFields.GetFieldPositions("Logotipo")[0].position;
                    int page = form.GetFieldPositions("Logotipo")[0].page;
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.logotipodragnet, System.Drawing.Imaging.ImageFormat.Png);
                    img.ScaleAbsolute(rect.Width, rect.Height);
                    img.SetAbsolutePosition(rect.Left, rect.Bottom);
                    stamper.GetOverContent(page).AddImage(img);
                    //}

                    string Fecha = inicio;
                    Recepcion Recep = ObtenerRecepcionRecicladoraById(idRecepcion);
                    if (n > 0)
                        Fecha = inicio + " - Cliente: " + Recep.Cliente;

                    form.SetField("fecha", Fecha);

                    for (int i = 0; i < 25 && j < n; i++, j++)
                    {
                        Paquete recibo = Historial[j];
                        form.SetField("n" + i, (j + 1).ToString());

                        if (recibo.CodigoPaquete == null)
                        {
                            form.SetField("recibo" + i, recibo.NombreProcesoProduccion);
                            form.SetField("formapago" + i, "");
                            form.SetField("tipomaterial" + i, "");
                            form.SetField("entrada" + i, recibo.Peso.ToString());
                            form.SetField("maquilado" + i, "");
                            form.SetField("subtotal" + i, "");
                        }
                        else
                        {

                            form.SetField("recibo" + i, DateTime.Parse(recibo.FechaPesado).ToString("dd-MM-yyyy"));
                            form.SetField("formapago" + i, recibo.CodigoPaquete);
                            form.SetField("tipomaterial" + i, recibo.NombreMaterial + " " + recibo.Color);
                            if (recibo.idProcesoProduccion == 1)
                            {
                                form.SetField("entrada" + i, recibo.Peso.ToString());
                                form.SetField("maquilado" + i, "");
                            }
                            else
                            {
                                form.SetField("entrada" + i, "");
                                form.SetField("maquilado" + i, recibo.Peso.ToString());
                            }
                            form.SetField("subtotal" + i, recibo.PesoSuperiorRestante.ToString());
                            total += Convert.ToInt32(recibo.Peso);
                        }
                    }

                    /*
                    if (j == n)
                    {
                        form.SetField("total", "Total   " + total.ToString());
                    }
                    */
                    stamper.Close();
                    //    pdf = dest.GetBuffer();

                    reader.Close();
                    reader.Dispose();
                    stamper.Dispose();
                    using (reader = new PdfReader(paginaR.GetBuffer()))
                    {
                        cp.AddPage(cp.GetImportedPage(reader, 1));
                    }
                }
                doc.Close();
                doc.Dispose();
                pdf = dest.ToArray();
            }
            return pdf;
        }
        #endregion

        #region Pieza

        public int AgregarPieza(Pieza Datos) {

            string Query = string.Format("INSERT INTO Pieza (Nombre, Descripcion) VALUES ('{0}', '{1}');",Datos.Nombre, Datos.Descripcion);
            return ActualizarDatos(Query);
        }

        public List<Pieza> ObtenerPieza()
        {
            List<Pieza> Lista = new List<Pieza>();

            string Query = "SELECT idPieza, Nombre, Descripcion FROM Pieza;";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Lista.Add(new Pieza(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2)));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Lista;
        }

        public Pieza ObtenerPiezaById(int id)
        {
            Pieza Datos = new Pieza();

            string Query = "SELECT idPieza, Nombre, Descripcion FROM Pieza WHERE idPieza = '" + id + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Pieza(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public Pieza ObtenerPiezaByName(string Nombre)
        {
            Pieza Datos = new Pieza();

            string Query = "SELECT idPieza, Nombre, Descripcion FROM Pieza WHERE Nombre = '" + Nombre + "';";

            MySqlDataReader ReaderBD = ObtenerDatosBD(Query);
            if (ReaderBD != null)
                while (ReaderBD.Read())
                    Datos = new Pieza(ReaderBD.GetInt32(0), ReaderBD.GetString(1), ReaderBD.GetString(2));

            ReaderBD.Dispose();
            ReaderBD.Close();
            Cerrar_Conexion();
            return Datos;
        }

        public void ActualizarPieza(Pieza Datos)
        {

            string Query = string.Format("UPDATE Pieza SET Nombre = '{0}', Descripcion = '{1}' WHERE idPieza = '{2}';", Datos.Nombre, Datos.Descripcion, Datos.idPieza);
            ActualizarDatos(Query);
        }

        public void EliminarPieza(int id)
        {

            string Query = string.Format("DELETE FROM Pieza WHERE idPieza = '{0}';", id);
            ActualizarDatos(Query);
        }


        #endregion


    }

    public class DASH
    {
        protected int m_CantidadProductos;
        protected int m_CantidadProveedores;
        protected int m_CantidadClientes;
        protected int m_CantidadVentas;
        protected int m_CantidadCompras;
        protected int m_CantidadCotizaciones;

        public int CantidadProductos { get { return m_CantidadProductos; } set { m_CantidadProductos = value; } }
        public int CantidadProveedores { get { return m_CantidadProveedores; } set { m_CantidadProveedores = value; } }
        public int CantidadClientes { get { return m_CantidadClientes; } set { m_CantidadClientes = value; } }
        public int CantidadVentas { get { return m_CantidadVentas; } set { m_CantidadVentas = value; } }
        public int CantidadCompras { get { return m_CantidadCompras; } set { m_CantidadCompras = value; } }
        public int CantidadCotizaciones { get { return m_CantidadCotizaciones; } set { m_CantidadCotizaciones = value; } }
    }

}
