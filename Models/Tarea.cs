namespace PortalEmpleos.Models
{
	public class Tarea
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string Responsable { get; set; }
		public string Estado { get; set; }
		public string FechaFin { get; set; }
		public string DiaModif { get; set; }
		public string Predecesoras { get; set; }
		public string Alta { get; set; }
		public int DiasModifInt { get; set; }
	}
}
