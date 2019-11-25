namespace PortalEmpleos
{
	public class ExperienciaLaboral
	{
		public string empresa { get; set; }
		public string puesto { get; set; }
		public string fechaDesde { get; set; }
		public string fechaHasta { get; set; }
		public bool actualmenteTrabajando { get; set; }
		public string[] conocimientosAdquiridos { get; set; }
		public string comentarios { get; set; }
	}
}