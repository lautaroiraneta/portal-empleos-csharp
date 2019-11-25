using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEmpleos.Models
{
	public class Empresa
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string Cuit { get; set; }
		public string SitioWeb { get; set; }
		public string Domicilio { get; set; }
		public IdValor[] Emails { get; set; }
		public IdValor[] Telefonos { get; set; }
		public string ContactoNombre { get; set; }
		public string ContactoApellido { get; set; }
		public string ContactoTelefono { get; set; }
		public string ContactoEmail { get; set; }
		public string ContactoCargo { get; set; }
		public string ContactoNombreUsuario { get; set; }
		public DateTime Alta { get; set; }
		public DateTime Baja { get; set; }
	}
}
