using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Http;

namespace InventariosPruebaWAR.Controllers
{
    public class PaqueteController : ApiController
    {
        private ClaseBD ClassBD = new ClaseBD();

        // GET: api/Paquete
        public List<Paquete> Get()
        {
            return ClassBD.ObtenerPaquetes();
        }

        // GET: api/Paquete/5
        public Paquete Get(int id)
        {
            return ClassBD.ObtenerPaqueteById(id);
        }

        [Route("api/Paquete/Completo/{id}")]
        public Paquete GetPaqueteCompletoById(int id)
        {
            return ClassBD.ObtenerPaqueteCompletoById(id);
        }

        [Route("api/Paquetes/Recepcion/{id}")]
        public List<Paquete> GetPaquetesByRecepcion(int id)
        {
            return ClassBD.ObtenerPaquetesByRecepcion(id);
        }

        [Route("api/Paquetes/Disponibles")]
        public List<Paquete> GetPaquetesDisponibles()
        {
            return ClassBD.ObtenerPaquetesDisponibles();
        }

        [Route("api/Paquetes/Triturar")]
        public List<Paquete> GetPaquetesparaTriturar()
        {
            return ClassBD.ObtenerPaquetesParaTriturar();
        }

        [Route("api/Paquetes/Procesar/{idProceso}")]
        public List<Paquete> GetPaquetesparaParaProcesar(int idProceso)
        {

            List<Paquete> Paquetes = ClassBD.ObtenerPaquetesParaProcesar(idProceso);
            ClassBD.Cerrar_Conexion();
            return Paquetes;
        }

        [Route("api/Paquetes/Recepcion/Historial/{id}/Proceso/{idProceso}")]
        public List<Paquete> GetHistorialPaquetesByRecepcion(int id, int idProceso)
        {
            List<Paquete> HistorialCompleto = new List<Paquete>();
            List<Paquete> HistorialProduccionPorProceso = new List<Paquete>();
            List<Paquete> HistorialBasuraPorProceso = new List<Paquete>();
            List<Proceso> ListaProcesos = ClassBD.ObtenerProcesos();

            if(idProceso != 0)
                ListaProcesos = ListaProcesos.FindAll(x => x.idProceso.Equals(idProceso));

            foreach (Proceso ProcesoRecicladora in ListaProcesos)
            {
                if(ProcesoRecicladora.idProceso < 6)
                {
                    HistorialProduccionPorProceso = ClassBD.ObtenerPaquetesProduccionByRecepcionProceso(id, ProcesoRecicladora.idProceso);

                    HistorialBasuraPorProceso = ClassBD.ObtenerPaquetesBasuraByRecepcionProceso(id, ProcesoRecicladora.idProceso);

                    if (HistorialProduccionPorProceso.Count != 0)
                    {
                        double entrada = ClassBD.ObtenerPaquetesEntradaProduccionByRecepcionProceso(id, ProcesoRecicladora.idProceso).Sum(x => x.Peso);
                        //double entrada = HistorialProduccionPorProceso.Sum(x => Convert.ToDouble(x.Peso)) + HistorialBasuraPorProceso.Sum(x => Convert.ToDouble(x.Peso));
                        double entradaRestante = entrada;//HistorialProduccionPorProceso.Sum(x => Convert.ToDouble(x.Peso)) + HistorialBasuraPorProceso.Sum(x => Convert.ToDouble(x.Peso));
                        if (ProcesoRecicladora.idProceso == 1)
                        {
                            entrada = HistorialProduccionPorProceso.Sum(x => x.Peso) + HistorialBasuraPorProceso.Sum(x => x.Peso);
                            entradaRestante = entrada;
                        }
                        if (ProcesoRecicladora.idProceso != 1)
                        {
                            foreach (Paquete PaqueteDeHistorialProd in HistorialProduccionPorProceso)
                            {
                                entradaRestante -= PaqueteDeHistorialProd.Peso;
                                PaqueteDeHistorialProd.PesoSuperiorRestante = entradaRestante;
                            }

                            foreach (Paquete PaqueteDeHistorialBas in HistorialBasuraPorProceso)
                            {
                                entradaRestante -= PaqueteDeHistorialBas.Peso;
                                PaqueteDeHistorialBas.PesoSuperiorRestante = entradaRestante;
                            }
                        }

                        HistorialCompleto.AddRange(HistorialProduccionPorProceso);
                        HistorialCompleto.AddRange(HistorialBasuraPorProceso);

                        int posicion = HistorialCompleto.Count - (HistorialBasuraPorProceso.Count + HistorialProduccionPorProceso.Count);
                        HistorialCompleto.Insert(posicion, new Paquete() { NombreProcesoProduccion = ProcesoRecicladora.NombreProceso, Peso = entrada });
                    }

                    

                }

            }
            return HistorialCompleto;
        }

        [Route("api/Paquetes/Produccion/Proceso/{idProceso}/Fecha/{fecha}")]
        public List<Paquete> GetPaquetesProduccionbyProcesoyFecha(int idProceso, string fecha)
        {
            List<Turnos> ListaTurnos = ClassBD.ObtenerTurnosSinDescanso();
            List<Maquinas> MaquinasdelProceso = ClassBD.ObtenerMaquinasByProceso(idProceso);
            List<Paquete> Produccion = ClassBD.ObtenerProduccionbyProcesoGeneral(idProceso, DateTime.Parse(fecha).ToString("yyyy-MM-dd"));
            Paquete PaqueteAuxTurnos, PaqueteAuxMaquinas;

            if (Produccion.Count == 0)
                foreach (Turnos Turno in ListaTurnos)
                {
                    PaqueteAuxTurnos = new Paquete(idProceso, DateTime.Parse(fecha), 0, 0, "", Turno.idTurno, Turno.Turno);
                    PaqueteAuxTurnos.SubPaquetes = new List<Paquete>();
                    foreach (Maquinas Maquina in MaquinasdelProceso)
                    {
                        PaqueteAuxMaquinas = new Paquete(idProceso, DateTime.Parse(fecha), 0, Maquina.idMaquina, Maquina.Maquina, Turno.idTurno, Turno.Turno);
                        PaqueteAuxTurnos.SubPaquetes.Add(PaqueteAuxMaquinas);
                    }
                    Produccion.Add(PaqueteAuxTurnos);
                }
            else
            {
                foreach (Paquete ObtenerporMaquina in Produccion)
                    ObtenerporMaquina.SubPaquetes = ClassBD.ObtenerProduccionbyProcesoGeneralByTurno(idProceso, DateTime.Parse(fecha).ToString("yyyy-MM-dd"), ObtenerporMaquina.idTurno);

                foreach (Turnos Turno in ListaTurnos)
                {
                    PaqueteAuxTurnos = Produccion.Find(x => x.idTurno.Equals(Turno.idTurno));
                    if (PaqueteAuxTurnos != null)
                    {
                        foreach (Maquinas Maquina in MaquinasdelProceso)
                        {
                            PaqueteAuxMaquinas = PaqueteAuxTurnos.SubPaquetes.Find(x => x.idMaquina.Equals(Maquina.idMaquina));
                            if (PaqueteAuxMaquinas == null)
                            {
                                PaqueteAuxMaquinas = new Paquete(idProceso, DateTime.Parse(fecha), 0, Maquina.idMaquina, Maquina.Maquina, Turno.idTurno, Turno.Turno);
                                PaqueteAuxTurnos.SubPaquetes.Add(PaqueteAuxMaquinas);
                            }
                        }
                    }
                    else
                    {
                        PaqueteAuxTurnos = new Paquete(idProceso, DateTime.Parse(fecha), 0, 0, "", Turno.idTurno, Turno.Turno);
                        PaqueteAuxTurnos.SubPaquetes = new List<Paquete>();
                        foreach (Maquinas Maquina in MaquinasdelProceso)
                        {
                            PaqueteAuxMaquinas = new Paquete(idProceso, DateTime.Parse(fecha), 0, Maquina.idMaquina, Maquina.Maquina, Turno.idTurno, Turno.Turno);
                            PaqueteAuxTurnos.SubPaquetes.Add(PaqueteAuxMaquinas);
                        }
                        Produccion.Add(PaqueteAuxTurnos);
                    }
                }
            }

            foreach (Paquete Paq in Produccion)
                Paq.SubPaquetes = Paq.SubPaquetes.OrderBy(x => x.idMaquina).ToList();

            return Produccion;
        }
        
        [Route("api/Paquetes/Produccion/Proceso/{idProceso}/Turno/{idTurno}/Maquina/{idMaquina}/Fecha/{fecha}")]
        public List<Paquete> GetPaquetesProduccionbyMaquina(int idProceso, int idTurno, int idMaquina, string fecha)
        {
            List<Paquete> ProduccionMaquina = ClassBD.ObtenerProduccionbyMaquinaFechaTurnoProceso(idProceso, idTurno, idMaquina, DateTime.Parse(fecha).ToString("yyyy-MM-dd"));

            foreach (Paquete Produccion in ProduccionMaquina)
            {
                Produccion.Cliente = ClassBD.ObtenerRecepcionRecicladoraById(Produccion.idRecepcion).Cliente;
            }
            return ProduccionMaquina;
        }

        [Route("api/Paquetes/Produccion/Turno/{id}")]
        public List<Paquete> GetPaquetesProduccionbyTurno(int id)
        {
            return ClassBD.ObtenerProduccionbyTurno(id);
        }


        [Route("api/Paquetes/Almacen/{id}")]
        public List<Paquete> GetPaquetesByAlmacen(int id)
        {
            return ClassBD.ObtenerPaquetesByAlmacen(id);
        }

        [Route("api/Paquete/aTriturar/{id}")]
        public ID GetPaqueteToTriturar(int id)
        {
            if (id < 1)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            Paquete Paq = ClassBD.ObtenerPaqueteById(id);
            ret = ClassBD.ActualizarEstatusPaquete(Paq, 2);
            Paq.idProceso = 3;
            ClassBD.ActualizarProcesoPaquete(Paq);
            return new ID(ret);
        }

        [Route("api/Paquete/{idPaquete}/Iniciar/Proceso/{idProceso}")]
        public ID GetPaqueteIniciarProceso(int idPaquete, int idProceso)
        {
            if (idPaquete < 1 || idProceso < 1)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            Paquete Paq = ClassBD.ObtenerPaqueteById(idPaquete);
            ret = ClassBD.ActualizarEstatusPaquete(Paq, 2);
            Paq.idProceso = idProceso;
            ClassBD.ActualizarProcesoPaquete(Paq);
            return new ID(ret);
        }
        
        [Route("api/ReporteProduccion/Proceso/{idProceso}/Fecha/{Fecha}")]
        public HttpResponseMessage GetCrearReporteDiarioPDF(int idProceso, string Fecha)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);

            //DateTime sinicio = DateTime.Now;
            DateTime sinicio = DateTime.Parse(Fecha);
            byte[] pdf = ClassBD.CrearReporteProduccionDiaPDF(idProceso, sinicio);

            if (pdf.Length != 0)
            {
                int length = (int)pdf.Length;
                byte[] buffer = new byte[length];

                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(pdf));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = length;

                return response;
            }
            else
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [Route("api/ReporteProduccion/Proceso/{idProceso}/FechaInicio/{FechaInicio}/FechaFin/{FechaFin}")]
        public HttpResponseMessage GetCrearReporteDiarioRangoFechasPDF(int idProceso, string FechaInicio, string FechaFin)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);

            //DateTime sinicio = DateTime.Now;
            DateTime sinicio = DateTime.Parse(FechaInicio);
            DateTime sfin = DateTime.Parse(FechaFin);

            byte[] pdf = ClassBD.CrearReporteProduccionDiaRangoFechasPDF(idProceso, sinicio, sfin);

            if (pdf.Length != 0)
            {
                int length = (int)pdf.Length;
                byte[] buffer = new byte[length];

                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(pdf));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = length;

                return response;
            }
            else
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [Route("api/ReporteProduccion/Recepcion/{idOrden}/Proceso/{idProceso}")]
        public HttpResponseMessage GetCrearReporteRecepcionPDF(int idOrden, int idProceso)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);

            //DateTime sinicio = DateTime.Now;
            //DateTime sinicio = DateTime.Parse(Fecha);
            List<Paquete> Historial = GetHistorialPaquetesByRecepcion(idOrden, idProceso);
            byte[] pdf = ClassBD.CrearReporteRecepcionPDF(Historial, idOrden);

            if (pdf.Length != 0)
            {
                int length = (int)pdf.Length;
                byte[] buffer = new byte[length];

                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(pdf));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = length;

                return response;
            }
            else
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        // POST: api/Paquete
        public ID Post([FromBody]Paquete Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            return new ID(ClassBD.AgregarPaquete(Datos));
        }

        [Route("api/Recepcion/ListaPaquetes")]
        public ID PostListaPaquetesdeRecepcion([FromBody]List<Paquete> ListaDatos)
        {
            if (ListaDatos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Recepcion Datos = ClassBD.ObtenerRecepcionRecicladoraById(ListaDatos[0].idRecepcion);
            Datos.FechaRecepcion = ListaDatos[0].FechaPesado;

            Paquete PaqueteRecepcion;
            int ret = -1;
            int Codigo = 1;
            foreach (Paquete Pack in ListaDatos)
            {
                PaqueteRecepcion = new Paquete(0, Pack.Peso, Datos.Embarque, Datos.idAlmacen, "", Pack.idRecepcion, Pack.idBascula, "",
                        DateTime.Parse(Pack.FechaPesado), Datos.idOperador, "", Pack.idMaterial, "", Datos.Descripcion, 1, "", Datos.idTurno, "", 1, "", Datos.Folio + Codigo, 1, "", Pack.idPaquete, "", Pack.PesoSuperiorRestante, 0);
                Codigo++;
                ret = ClassBD.AgregarPaquete(PaqueteRecepcion);

            }
            return new ID(ret);
        }

        [Route("api/Paquete/CambiarAlmacen")]
        public ID PutCambiarAlmacenOProceso([FromBody]Paquete Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;

            if (Datos.idAlmacen == 0 && Datos.idProceso == 0)
                return new ID(ret);

            
            Paquete NuevoPaquete = ClassBD.ObtenerPaqueteById(Datos.idPaquete);

            if (Datos.idProceso != 0)
            {
                if (Datos.idProceso != 6)
                    ClassBD.ActualizarEstatusPaquete(NuevoPaquete, 2);
                else
                    ClassBD.ActualizarEstatusPaquete(NuevoPaquete, 3);

                NuevoPaquete.idProceso = Datos.idProceso;
                ClassBD.ActualizarProcesoPaquete(NuevoPaquete);

                ret = 1;
                return new ID(ret);
            }


            if (NuevoPaquete.idAlmacen != Datos.idAlmacen && Datos.idAlmacen != 0)
            {
                NuevoPaquete.FechaPesado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Almacen Almacen = ClassBD.ObtenerAlmacenById(Convert.ToInt32(Datos.idAlmacen));
                NuevoPaquete.Descripcion = NuevoPaquete.Descripcion + "\nTransferido de: " + NuevoPaquete.Almacen + "\n a: " + Almacen.NombreAlmacen;
                NuevoPaquete.idAlmacen = Almacen.idAlmacen;
                NuevoPaquete.CodigoPaquete += "TA";
                ret = ClassBD.AgregarPaquete(NuevoPaquete);
                ClassBD.ActualizarEstatusPaquete(NuevoPaquete, 3);
                return new ID(ret);
            }

            return new ID(ret);
        }

        [Route("api/Paquete/ListaPaquetes")]
        public ID PostListaPaquetesdePaqueteProcesado([FromBody]List<Paquete> ListaDatos)
        {
            if (ListaDatos == null && ListaDatos.Count > 0)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            int Codigo = 1;
            int Finalizado = 0;
            bool ActualizarPedido = false;

            Pedidos Pedido = ClassBD.ObtenerPedidoByIdRecepcion(ListaDatos[0].idRecepcion);
            Paquete PaqueteSuperior = ClassBD.ObtenerPaqueteById(ListaDatos[0].idPaqueteSuperior);
            Proceso Proceso = ClassBD.ObtenerProcesoById(ListaDatos[0].idProceso);
            PaqueteSuperior.CodigoPaquete += Proceso.Codigo;

            List<Paquete> SubPaquetesActuales = ClassBD.ObtenerPaquetesByPaqueteSuperior(PaqueteSuperior.idPaquete);
            if (SubPaquetesActuales != null && SubPaquetesActuales.Count > 0)
                foreach (Paquete PaqueteExistente in SubPaquetesActuales)
                    ClassBD.EliminarPaquete(PaqueteExistente.idPaquete);
            
            foreach (Paquete Pack in ListaDatos)
            {
                Pack.Embarque = PaqueteSuperior.Embarque;
                Pack.idEstatus = 1;
                Pack.CodigoPaquete = PaqueteSuperior.CodigoPaquete + Codigo;
                Codigo++;
                Pack.idProcesoProduccion = Pack.idProceso;
                Pack.Precio = Pack.Peso * Proceso.PrecioKG;
                if(Pack.idProcesoProduccion == Pedido.idProcesoFinal)
                {
                    Pack.idEstatus = 4;
                    ActualizarPedido = true;
                }
                ret = ClassBD.AgregarPaquete(Pack);
                Finalizado += Pack.Finalizado;
            }
            if(Finalizado > 0)
                ClassBD.ActualizarEstatusPaquete(PaqueteSuperior, 3);

            if (ActualizarPedido) {
                double Porcentaje = 0;
                Recepcion Recep = ClassBD.ObtenerRecepcionRecicladoraById(ListaDatos[0].idRecepcion);
                Paquete ProduccionProcesoFinal = ClassBD.ObtenerProduccionbyProcesoFinal(ListaDatos[0].idRecepcion, Pedido.idProcesoFinal);
                Paquete BasuraPedidoCompleto = ClassBD.ObtenerTodaBasuraByidRecepcion(ListaDatos[0].idRecepcion);

                Porcentaje = (100 * (ProduccionProcesoFinal.Peso + BasuraPedidoCompleto.Peso) ) / Recep.PesoInterno ;
                ClassBD.ActualizarProgresoPedido(Porcentaje, Pedido.idPedidos);
            }

            return new ID(ret);
        }

        [Route("api/Paquete/Editar/ListaPaquetes")]
        public ID PostEditarListaPaquetesdePaqueteProcesado([FromBody]List<Paquete> ListaDatos)
        {
            if (ListaDatos == null && ListaDatos.Count > 0)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            int ret = -1;
            int Codigo = 1;
            int Finalizado = 0;
            bool ActualizarPedido = false;

            Pedidos Pedido = ClassBD.ObtenerPedidoByIdRecepcion(ListaDatos[0].idRecepcion);
            Paquete PaqueteSuperior = ClassBD.ObtenerPaqueteById(ListaDatos[0].idPaqueteSuperior);
            Proceso Proceso = ClassBD.ObtenerProcesoById(ListaDatos[0].idProceso);
            PaqueteSuperior.CodigoPaquete += Proceso.Codigo;

            List<Paquete> SubPaquetesActuales = ClassBD.ObtenerPaquetesByPaqueteSuperior(PaqueteSuperior.idPaquete);
            if (SubPaquetesActuales != null && SubPaquetesActuales.Count > 0)
                foreach (Paquete PaqueteExistente in SubPaquetesActuales)
                    ClassBD.EliminarPaquete(PaqueteExistente.idPaquete);

            foreach (Paquete Pack in ListaDatos)
            {
                Pack.Embarque = PaqueteSuperior.Embarque;
                Pack.idEstatus = 1;
                Pack.CodigoPaquete = PaqueteSuperior.CodigoPaquete + Codigo;
                Codigo++;
                Pack.Precio = Pack.Peso * Proceso.PrecioKG;
                if (Pack.idProcesoProduccion == Pedido.idProcesoFinal)
                {
                    Pack.idEstatus = 4;
                    ActualizarPedido = true;
                }
                ret = ClassBD.AgregarPaquete(Pack);
                Finalizado += Pack.Finalizado;
            }
            if (Finalizado > 0)
                ClassBD.ActualizarEstatusPaquete(PaqueteSuperior, 3);

            if (ActualizarPedido)
            {
                double Porcentaje = 0;
                Recepcion Recep = ClassBD.ObtenerRecepcionRecicladoraById(ListaDatos[0].idRecepcion);
                Paquete ProduccionProcesoFinal = ClassBD.ObtenerProduccionbyProcesoFinal(ListaDatos[0].idRecepcion, Pedido.idProcesoFinal);
                Paquete BasuraPedidoCompleto = ClassBD.ObtenerTodaBasuraByidRecepcion(ListaDatos[0].idRecepcion);

                Porcentaje = (100 * (ProduccionProcesoFinal.Peso + BasuraPedidoCompleto.Peso)) / Recep.PesoInterno;
                ClassBD.ActualizarProgresoPedido(Porcentaje, Pedido.idPedidos);
            }

            return new ID(ret);
        }

        // PUT: api/Paquete
        public void Put(Paquete Datos)
        {
            if (Datos == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            ClassBD.ActualizarPaquete(Datos);
        }

        // DELETE: api/Paquete/5
        public void Delete(int id)
        {
            ClassBD.EliminarPaquete(id);
        }
    }
}
