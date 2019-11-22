namespace PortalEmpleos
{
	public class Alumno
	{
		// /alumno
		public string nombres;
		public string apellidos;
		public string libretaUniversitaria;
		public string[] emails;
		public string tipoDocumento;
		public string numeroDocumento;
		public string nombreUsuario;

		// /crear-perfil
		public string[] telefonos;
		public string paisResidencia;
		public string provinciaResidencia;
		public string zona;
		public string ciudad;
		public string localidad;
		public string fechaDeNacimiento;
		public string estadoCivil;
		public string paisNacionalidad;

		public RedesSociales redesSociales;
		public string objetivoLaboral;
		public string interesesPersonales;

		public ExperienciaLaboral[] experenciasLaborales;
		public ExperienciaEducativa[] experienciasEducativas;
		public Idioma[] idiomas;

		public string[] otrosComentarios;

	}
}
