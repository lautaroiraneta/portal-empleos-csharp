using System;

namespace PortalEmpleos.Models
{
	public class Propuesta
	{
		public string Titulo { get; set; }
		public Puesto[] PuestosCarac { get; set; }
		public IdValor[] Carreras { get; set; }
		public string EmpresaId { get; set; }
		public bool CarrerasAfines { get; set; }
		public IdValor[] Pais { get; set; }
		public IdValor[] Provincia { get; set; }
		public IdValor[] Zona { get; set; }
		public IdValor[] Ciudad { get; set; }
		public IdValor[] Localidad { get; set; }
		public float SueldoBruto { get; set; }
		public IdValor[] TipoEmpleo { get; set; }
		public IdValor[] Turno { get; set; }
		public string Beneficios { get; set; }
		public DateTime FechaFinalizacionDT { get; set; }
		public string Descripcion { get; set; }
		public int EdadMin { get; set; }
		public bool ExcluyenteEdadMin { get; set; }
		public int EdadMax { get; set; }
		public bool ExcluyenteEdadMax { get; set; }
		public bool DisponibilidadReubicacion { get; set; }
		public string HabilidadesPersonales { get; set; }
		public float PorcentajeMatApr { get; set; }
		public bool ExcluyentePorc { get; set; }
		public int CantidadMatApr { get; set; }
		public bool ExcluyenteMatApr { get; set; }
		public float Promedio { get; set; }
		public bool ExcluyentePromedio { get; set; }
		public float AnioCursada { get; set; }
		public bool ExcluyenteAnioCursada { get; set; }
		public DateTime Alta { get; set; }
		public DateTime Baja { get; set; }

	}
}
