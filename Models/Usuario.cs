using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEmpleos.Models
{
	public class Usuario
	{
		public string Id { get; set; }
		public string NombreUsuario { get; set; }
		public string Password { get; set; }
		public string TipoUsuario { get; set; }
		public string AlumnoId { get; set; }
		public string EmpresaId { get; set; }
		public string Alta { get; set; }
		public bool Aprobado { get; set; }
		public string Nombre { get; set; }
		public string Apellido { get; set; }
	}
}
