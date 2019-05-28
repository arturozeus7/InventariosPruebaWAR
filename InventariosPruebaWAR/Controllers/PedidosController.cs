using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class PedidosController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Pedidos
        public List<Pedidos> Get()
        {
            return ClassBD.ObtenerPedidos();
        }

        // GET: api/Pedidos/5
        public Pedidos Get(int id)
        {
            return ClassBD.ObtenerPedidoById(id);
        }

        [Route("api/Pedidos/Cliente/{id}")]
        public List<Pedidos> GetPedidosByCliente(int id)
        {
            return ClassBD.ObtenerPedidosByCliente(id);
        }

        [Route("api/Pedidos/Disponibles")]
        public List<Pedidos> GetPedidosDisponibles()
        {
            return ClassBD.ObtenerPedidosDisponibles();
        }

        // POST: api/Pedidos
        public ID Post([FromBody]Pedidos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarPedido(Datos));
        }

        // PUT: api/Pedidos
        public void Put(Pedidos Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPedido(Datos);
        }

        // DELETE: api/Pedidos/5
        public void Delete(int id)
        {
            ClassBD.EliminarPedido(id);
        }
    }
}
