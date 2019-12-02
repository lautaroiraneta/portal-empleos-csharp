using PortalEmpleos.Models;
using System;

namespace PortalEmpleos
{
	public class ExperienciaLaboral
	{
		public string Id { get; set; }
		public string Empresa { get; set; }
		public Puesto[] PuestoLaboral { get; set; }
		public DateTime FechaDesdeDT { get; set; }
		public DateTime FechaHastaDT { get; set; }
		public bool ActualmenteTrabajando { get; set; }
		public Conocimiento[] Conocimientos { get; set; }
		public string Descripcion { get; set; }
	}
}