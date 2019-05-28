using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    [Authorize]
    public class PreciosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Precios
        public List<Precios> Get()
        {
            return ClassBD.ObtenerPrecios();
        }

        // GET: api/Precios/5
        public Precios Get(int id)
        {
            return ClassBD.ObtenerPrecioById(id);
        }

        [Route("api/Precios/Busqueda/{consulta}")]
        public List<Precios> Get(string consulta)
        {
            consulta = consulta.Replace("'", "-");
            return ClassBD.ObtenerBusquedaPrecios(consulta);
        }

        [Route("api/Precios/Producto/Maximo/{id}")]
        public Precios GetPrecioMaxById(string id)
        {
            return ClassBD.ObtenerPrecioMaxById(Convert.ToInt32(id));
        }

        [Route("api/Precios/Etiqueta/{etiqueta}")]
        public List<Precios> GetPreciosByEtiqueta(string etiqueta)
        {
            return ClassBD.ObtenerPreciosByEtiqueta(etiqueta);
        }

        [Route("api/Precios/Sucursal/{idSucursal}/Producto/{id}")]
        public List<Precios> GetPreciosBySuc(string idSucursal, string id)
        {
            return ClassBD.ObtenerPreciosByProductoySucursal(Convert.ToInt32(idSucursal), Convert.ToInt32(id));
        }

        [Route("api/Precios/Sucursal/{idSucursal}/Cantidad/{cantidad}/Producto/{id}")]
        public Precios GetPreciosByCantidad(string idSucursal, string id, string cantidad)
        {
            return ClassBD.ObtenerPreciosByCantidad(Convert.ToInt32(idSucursal), Convert.ToInt32(id), Convert.ToInt32(cantidad));
        }

        // POST: api/Precios
        public ID Post([FromBody]Precios Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            List<Sucursal> Sucursales = ClassBD.ObtenerSucursal();
            foreach (Sucursal Suc in Sucursales)
            {
                Datos.idSucursal = Suc.idSucursal;
                ret = ClassBD.AgregarPrecios(Datos);
            }
            return new ID(ret);
        }

        // PUT: api/Precios
        public void Put([FromBody]Precios Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPrecios(Datos);
        }

        // DELETE: api/Precios/5
        public void Delete(int id)
        {
            ClassBD.EliminarPrecios(id);
        }
    }
}
