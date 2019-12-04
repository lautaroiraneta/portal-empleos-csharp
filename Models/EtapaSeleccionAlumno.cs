namespace PortalEmpleos.Models
{
	public class EtapaSeleccionAlumno
	{
		public string EtapaId { get; set; }
		public string Alumno { get; set; }
		public string Carrera { get; set; }
		public IdValor Empresa { get; set; }
		public IdValor Propuesta { get; set; }
		public string FechaPostulacion { get; set; }
		public string EstadoEtapa { get; set; }

	}
}
