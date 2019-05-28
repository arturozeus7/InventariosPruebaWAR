using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace InventariosPruebaWAR.Controllers
{
    public class DescargasController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Descargas
        [Route("api/facturas/{archivo}")]
        public Stream GetPdfFile(string archivo)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
            string filename = "Facturas/" + archivo;
            if (File.Exists(filename))
            {


                FileStream f = new FileStream(filename, FileMode.Open);
                int length = (int)f.Length;
                WebOperationContext.Current.OutgoingResponse.ContentLength = length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();
                return new MemoryStream(buffer);
            }
            else
                throw new WebFaultException(HttpStatusCode.NotFound);
        }

        [Route("api/Ventas/{archivo}")]
        public Stream GetPdfFileVentas(string archivo)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
            string filename = "Ventas/" + archivo;
            if (File.Exists(filename))
            {


                FileStream f = new FileStream(filename, FileMode.Open);
                int length = (int)f.Length;
                WebOperationContext.Current.OutgoingResponse.ContentLength = length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();
                return new MemoryStream(buffer);
            }
            else
                throw new WebFaultException(HttpStatusCode.NotFound);
        }

        [Route("api/Compras/{archivo}")]
        public HttpResponseMessage GetPdfFileCompras(string archivo)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            string filename = "Compras/" + archivo + ".pdf";
            if (File.Exists(filename))
            {
                FileStream f = new FileStream(filename, FileMode.Open);
                int length = (int)f.Length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();

                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(buffer));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = length;

                return response;
            }
            else
                throw new WebFaultException(HttpStatusCode.NotFound);
        }

        [Route("api/Cotizacion/{archivo}")]
        public HttpResponseMessage GetPdfFileCotizacion(string archivo)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            string filename = "Cotizacion/" + archivo + ".pdf";
            if (File.Exists(filename))
            {
                FileStream f = new FileStream(filename, FileMode.Open);
                int length = (int)f.Length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();
                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(buffer));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = length;

                return response;
            }
            else
                throw new WebFaultException(HttpStatusCode.NotFound);
        }

        public Stream GetCSVFileInventario(string archivo)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/csv";
            string filename = "Inventario/" + archivo;
            if (File.Exists(filename))
            {


                FileStream f = new FileStream(filename, FileMode.Open);
                int length = (int)f.Length;
                WebOperationContext.Current.OutgoingResponse.ContentLength = length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();
                return new MemoryStream(buffer);
            }
            else
                throw new WebFaultException(HttpStatusCode.NotFound);
        }

        [Route("api/CotizacionXpz/{archivo}")]
        public HttpResponseMessage GetPdfFileCotizacionXpz(string archivo)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            string filename = "Cotizacionpz/" + archivo + ".pdf";
            if (File.Exists(filename))
            {


                FileStream f = new FileStream(filename, FileMode.Open);
                int length = (int)f.Length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();
                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(buffer));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = length;

                return response;
            }
            else
                throw new WebFaultException(HttpStatusCode.NotFound);
        }

        [Route("api/inventario/descarga/{archivo}")]
        public HttpResponseMessage GetInventario(string archivo)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            //WebOperationContext.Current.OutgoingResponse.ContentType = "text/csv";
            string filename = "csv/inventario" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            try
            {
                List<Inventario> lista = ClassBD.ObtenerInventario();
                CostosHistoricos CostoProducto = new CostosHistoricos();
                foreach (Inventario Producto in lista)
                {
                    CostoProducto = ClassBD.ObtenerCostosHbyProduct(Producto.idProducto);
                    Producto.idProveedor = CostoProducto.idProveedor;
                    Producto.Proveedor = CostoProducto.Proveedor;
                }

                string[] lineas = new string[lista.Count + 1];
                int i = 0;
                lineas[0] = "Almacén,Proveedor,Código,Producto,Color,Presentación,Piezas,Piezas Totales,Descripción,Existencia,Existencia Mínima";
                i++;
                foreach (Inventario inv in lista)
                {

                    lineas[i] = inv.Sucursal + "," + inv.Proveedor + "," + inv.Codigo + "," + inv.Producto + "," + inv.Color + "," + inv.Presentacion + "," + inv.Piezas + "," + inv.PiezasTotales +
                        ",\"" + inv.Descripcion + "\"," + inv.Existencia + "," + inv.ExistenciaMinima;
                    i++;
                }
                File.WriteAllLines(filename, lineas, Encoding.UTF8);

                if (File.Exists(filename))
                {


                    FileStream f = new FileStream(filename, FileMode.Open);
                    int length = (int)f.Length;
                    WebOperationContext.Current.OutgoingResponse.ContentLength = length;
                    byte[] buffer = new byte[length];
                    int sum = 0;
                    int count;
                    while ((count = f.Read(buffer, sum, length - sum)) > 0)
                    {
                        sum += count;
                    }
                    f.Close();

                    var statuscode = HttpStatusCode.OK;
                    response = Request.CreateResponse(statuscode);
                    response.Content = new StreamContent(new MemoryStream(buffer));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                    response.Content.Headers.ContentLength = length;

                    return response;
                }
                else
                    throw new WebFaultException(HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }

        }
    }
}
