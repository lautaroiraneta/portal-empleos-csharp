using System;

namespace PortalEmpleos.Models
{
	public class Puesto
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string Estado { get; set; }
		public DateTime Alta { get; set; }
		public DateTime Baja { get; set; }
	}
}
