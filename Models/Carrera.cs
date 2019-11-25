using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEmpleos.Models
{
	public class Carrera
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string Facultad { get; set; }
		public string Codigo { get; set; }
		public DateTime Alta { get; set; }
		public DateTime Baja { get; set; }
	}
}
