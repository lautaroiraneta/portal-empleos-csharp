namespace PortalEmpleos.Models
{
	public class Etapa
	{
		public IdValor AdministradorUniversidad { get; set; }
		public IdValor AdministradorEmpresa { get; set; }
		public IdValor Estado { get; set; }
		public Tarea[] Tareas { get; set; }

		public IdValor Empresa { get; set; }
		public IdValor Convenio { get; set; }
		public string Archivo { get; set; }
		public string NombreEtapa { get; set; }
		public IdValor Alumno { get; internal set; }
		public string TituloPropuesta { get; internal set; }
		public string NombreConvenio { get; internal set; }
		public string EtapaDefinicionConvenio { get; internal set; }
		public string EtapaIngreso { get; internal set; }
		public string RazonRechazo { get; internal set; }
	}
}
