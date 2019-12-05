
namespace PortalEmpleos.Models
{
	public class TareaView
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public string Estado { get; set; }
		public string Responsable { get; set; }
		public string FechaFin { get; set; }
		public Comentario[] Comentarios { get; set; }
		public TareaViewArchivo[] Archivos { get; set; }
		public IdValor[] TareasQueHabilita { get; set; }
		public IdValor[] TareasPredecesoras { get; set; }
	}
}
