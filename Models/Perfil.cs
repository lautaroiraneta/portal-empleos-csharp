using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalEmpleos.Models
{
	public class Perfil
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public string Apellido { get; set; }
		public IdValor[] PaisResidencia { get; set; }
		public IdValor[] Emails { get; set; }
		public IdValor[] Telefonos { get; set; }
		public IdValor[] ProvinciaResidencia { get; set; }
		public DateTime FechaNacimientoDT { get; set; }
		public IdValor[] EstadoCivil { get; set; }
		public IdValor[] PaisNacionalidad { get; set; }
		public string TipoDocumento { get; set; }
		public string Documento { get; set; }
		public RedesSociales RedesSociales { get; set; }
		public string ObjetivoLaboral { get; set; }
		public string InteresesPersonales { get; set; }
		public ExperienciaLaboral[] ExperienciaLaboral { get; set; }
	}
}
