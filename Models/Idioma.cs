using PortalEmpleos.Models;

namespace PortalEmpleos
{
	public class Idioma
	{
		public string Id { get; set; }
		public IdValor[] NombreIdioma { get; set; }
		public IdValor[] NivelOral { get; set; }
		public IdValor[] NivelEscrito { get; set; }
		public string Comentarios { get; set; }
	}
}