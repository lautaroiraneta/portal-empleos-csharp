using System;

namespace PortalEmpleos.Models
{
	public class PropuestaView
	{
		public string Id { get; internal set; }
		public string Titulo { get; internal set; }
		public IdValor Empresa { get; internal set; }
		public DateTime FechaPosteo { get; internal set; }
		public int DiasDif { get; internal set; }
		public string Provincia { get; internal set; }
		public string Zona { get; internal set; }
		public string Ciudad { get; internal set; }
		public string Localidad { get; internal set; }
		public string TipoEmpleo { get; internal set; }
		public string TurnoEmpleo { get; internal set; }
		public IdValor[] Carreras { get; internal set; }
	}
}
