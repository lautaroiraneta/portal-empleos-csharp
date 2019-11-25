namespace PortalEmpleos
{
	public class Alumno
	{
		internal string nombres;

		// /alumno
		public string Nombres { get; set; }
		public string Apellidos { get; set; }
		public string LibretaUniversitaria { get; set; }
		public string Email { get; set; }
		public string TipoDocumento { get; set; }
		public string NumeroDocumento { get; set; }
		public string NombreUsuario { get; set; }

		// /crear-perfil
		public string[] telefonos { get; set; }
		public string paisResidencia { get; set; }
		public string provinciaResidencia { get; set; }
		public string zona { get; set; }
		public string ciudad { get; set; }
		public string localidad { get; set; } 
		public string fechaDeNacimiento { get; set; }
		public string estadoCivil { get; set; }
		public string paisNacionalidad { get; set; }

		public RedesSociales redesSociales { get; set; }
		public string objetivoLaboral { get; set; }
		public string interesesPersonales { get; set; }

		public ExperienciaLaboral[] experenciasLaborales { get; set; }
		public ExperienciaEducativa[] experienciasEducativas { get; set; }
		public Idioma[] idiomas { get; set; }

		public string[] otrosComentarios { get; set; }

	}
}
