using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEmpleos.Models
{
	public class Convenio
	{
		public string Nombre { get; set; }
		public IdValor[] Facultad { get; set; }
		public bool GenerarUniversidad { get; set; }
		public int DuracionMinima { get; set; }
		public int DuracionMaxima { get; set; }
		public int PlazoRenovacion { get; set; }
		public int CantidadMaxHoras { get; set; }
		public DateTime InicioVigenciaDT { get; set; }
		public DateTime FinVigenciaDT { get; set; }
		public bool RenovacionAutomatica { get; set; }
		public int PlazoExtension { get; set; }
		public int CantMaxPasantes { get; set; }
	}
}
