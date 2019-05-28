using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class LoginController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();
        protected int Privilegios = 0;

        // POST: api/Login
        public Respuesta Post([FromBody]Empleado usr)
        {
            Empleado emp = null;
            bool ret = false;
            string token;

            if (usr == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            if (usr.Usuario == null || usr.Contra == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }


            long id = ClassBD.Login(usr.Usuario, usr.Contra, out Privilegios);

            if (id < 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else if (id == 0) //contraseña incorrecta
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            else
            {
                token = TokenGenerator.GenerateTokenJwt(usr.Usuario);
                ret = true;
            }

            if (ret)
                emp = ClassBD.ObtenerEmpleadoById((int)id);

            HttpContext.Current.Response.Headers.Add("Authorization", "Bearer " + token);

            return new Respuesta
            {
                Texto = ret ? emp.Nombre + " " + emp.ApellidoPaterno : "",
                emplid = ret ? emp.idEmpleado : 0,
                Resultado = ret,
                priv = emp.idPrivilegio
            };
        }

        /*
        public HttpResponseMessage Post([FromBody]Empleado usr)
        {
            Empleado emp = null;
            bool ret = false;
            string token;

            if (usr == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            if (usr.Usuario == null || usr.Contra == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }


            long id = ClassBD.Login(usr.Usuario, usr.Contra, out Privilegios);

            if (id < 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else if (id == 0) //contraseña incorrecta
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            else
            {
                token = TokenGenerator.GenerateTokenJwt(usr.Usuario);
                ret = true;
            }

            if (ret)
                emp = ClassBD.ObtenerEmpleadoById((int)id);

            Respuesta Res = new Respuesta
            {
                Texto = ret ? emp.Nombre + " " + emp.ApellidoPaterno : "",
                emplid = ret ? emp.idEmpleado : 0,
                Resultado = ret,
                priv = emp.idPrivilegio
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, Res);
            //var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer ", token);

            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //response.Headers.Add("Authorization", "Bearer " + token);

            HttpContext.Current.Response.Headers.Add("Authorization", "Bearer " + token);
            return response;
        }*/


        [Route("api/ValidaUsuario")]
        public Respuesta PostValidaUsuario(Empleado usr)
        {
            Empleado emp = null;
            bool ret = false;
            string token;

            if (usr == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
            if (usr.Usuario == null || usr.Contra == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }


            long id = ClassBD.Login(usr.Usuario, usr.Contra, out Privilegios);
            if (id < 0)
            {
                ret = false;
            }
            else if (id == 0) //contraseña incorrecta
            {
                ret = false;
            }
            else
            {
                token = TokenGenerator.GenerateTokenJwt(usr.Usuario);
                ret = true;
            }

            if (ret)
            {
                ClaseBD db = new ClaseBD();
                emp = db.ObtenerEmpleadoById((int)id);
            }
            HttpContext.Current.Response.Headers.Add("Authorization", "Bearer ");
            return new Respuesta { Resultado = ret };
        }

        //[Route("api/ObtenerDatosDash")]
        [Route("api/DatosDash")]
        public DASH GetObtenerDatosDash()
        {
            //string token = TokenGenerator.GenerateTokenJwt("Axel");
        
            //HttpContext.Current.Response.Headers.Add("Authorization", "Bearer " + token);
            return ClassBD.ObtenerDatosDash();
        }
    }

    public class Respuesta
    {
        public bool Resultado { get; set; }
        public string Texto { get; set; }
        public int emplid { get; set; }
        public int priv { get; set; }
    }
}
