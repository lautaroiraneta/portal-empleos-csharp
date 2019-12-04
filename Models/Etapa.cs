namespace PortalEmpleos.Models
{
	public class Etapa
	{
		public IdValor AdministradorUniversidad { get; set; }
		public IdValor AdministradorEmpresa { get; set; }
		public IdValor Estado { get; set; }
		public Tarea[] Tareas { get; set; }
	}
}
