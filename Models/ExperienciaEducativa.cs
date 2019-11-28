using PortalEmpleos.Models;
using System;

namespace PortalEmpleos
{
	public class ExperienciaEducativa
	{
		public string Institucion { get; set; }
		public string Titulo { get; set; }
		public IdValor[] TipoEstudio { get; set; }
		public IdValor[] Estado { get; set; }
		public DateTime FechaDesdeDT { get; set; }
		public DateTime FechaHastaDT { get; set; }
		public bool ActualmenteEstudiando { get; set; }
		public string Comentarios { get; set; }
	}
}