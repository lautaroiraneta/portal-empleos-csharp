using PortalEmpleos.Models;

namespace PortalEmpleos
{
	public class Idioma
	{
		public IdValor[] NombreIdioma { get; set; }
		public IdValor[] NivelOral { get; set; }
		public IdValor[] NivelEscrito { get; set; }
		public string Comentarios { get; set; }
	}
}