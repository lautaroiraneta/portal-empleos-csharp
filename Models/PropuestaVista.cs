namespace PortalEmpleos.Models
{
	public class PropuestaVista
	{
		public string Id { get; internal set; }
		public string Provincia { get; internal set; }
		public string Zona { get; internal set; }
		public string Ciudad { get; internal set; }
		public string Localidad { get; internal set; }
		public string TipoEmpleo { get; internal set; }
		public string TurnoEmpleo { get; internal set; }
		public string FechaAlta { get; internal set; }
		public string SueldoBruto { get; internal set; }
		public string Beneficios { get; internal set; }
		public string Descripcion { get; internal set; }
		public string EdadMin { get; internal set; }
		public string EdadMax { get; internal set; }
		public string Reubicarse { get; internal set; }
		public string HabilidadesPersonales { get; internal set; }
		public string PorcentajeMatApr { get; internal set; }
		public string CantidadMatApr { get; internal set; }
		public string Promedio { get; internal set; }
		public string AnioCursada { get; internal set; }
		public IdValor Empresa { get; internal set; }
		public string Titulo { get; internal set; }
		public string[] Carreras { get; internal set; }
		public string[] Conocimientos { get; internal set; }
		public string[] PuestosReq { get; internal set; }
		public string[] PuestosCarac { get; internal set; }
	}
}
