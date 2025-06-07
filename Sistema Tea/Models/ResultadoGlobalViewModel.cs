namespace Sistema_Tea.Models
{
    public class ResultadoGlobalViewModel
    {
        public int SesionID { get; set; }
        public string PacienteNombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaInicio { get; set; }
        public string PsicologoNombre { get; set; }
        public string TipoAlgoritmoUsado { get; set; }
        public int ComunicacionSocial_PuntuacionTotal { get; set; }
        public int InteraccionSocialReciproca_PuntuacionTotal { get; set; }
        public int ComportamientosRepetitivos_PuntuacionTotal { get; set; }
        public string ClasificacionADIR { get; set; }
        public string NotasResultadoGlobal { get; set; }
        public string EdadTexto { get; set; }

        public List<ItemRespondidoViewModel> Historial { get; set; }
    }

    public class ItemRespondidoViewModel
    {
        public string CodigoPregunta { get; set; }
        public string TextoPregunta { get; set; }
        public string CodigoRespuestaAlgoritmo { get; set; }
        public string NotasObservacionItem { get; set; }
    }
}
