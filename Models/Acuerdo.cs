namespace PortalEmpleos.Models
{
	public class Acuerdo
	{
		public string IngresoAlumno { get; set; }
		public string Alumno { get; set; }
		public string Empresa { get; set; }
		public string Nombre { get; set; }
		public IdValor[] DocenteGuia { get; set; }
		public IdValor[] Tutor { get; set; }
		public int Duracion { get; set; }
		public IdValor[] Tareas { get; set; }
		public Calendario Calendario { get; set; }
	}
}
