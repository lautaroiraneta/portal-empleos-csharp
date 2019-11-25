using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[EnableCors("AllowOrigin")]
	public class AlumnoController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<AlumnoController> _logger;

		public AlumnoController(IConfiguration configuration, ILogger<AlumnoController> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPost]
		public void Post([FromBody] Alumno alumno)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into alumnos (nombre, apellido, libretaUniversitaria, email, tipoDocumento, documento, nombreUsuario, alta) " +
											"values (@nombre, @apellido, @libretaUniversitaria, @email, @tipoDocumento, @documento, @nombreUsuario, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", alumno.Nombres);
			com.Parameters.AddWithValue("@apellido", alumno.Apellidos);
			com.Parameters.AddWithValue("@libretaUniversitaria", alumno.LibretaUniversitaria);
			com.Parameters.AddWithValue("@email", alumno.Email);
			com.Parameters.AddWithValue("@tipoDocumento", alumno.TipoDocumento);
			com.Parameters.AddWithValue("@documento", alumno.NumeroDocumento);
			com.Parameters.AddWithValue("@nombreUsuario", alumno.NombreUsuario);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);
			//SqlCommand com = new SqlCommand("" +
			//	"insert into carreras " +
			//	"(nombre, codigo) " +
			//	"values" +
			//	"('" + carrera.Nombre + "','" + carrera.Codigo + "')", connection);

			//SqlCommand com = new SqlCommand("select top 1 * from alumnos", connection);
			com.ExecuteReader();

			connection.Close();
		}

		[HttpGet]
		[Route("/alumno/list")]
		public Alumno Get()
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();
			SqlCommand com = new SqlCommand("insert into alumnos values ('lucas','vvv','123ee456','ww1234@..','dnei','ewewf','liranewweeta',getdate(),null)", connection);

			//SqlCommand com = new SqlCommand("select top 1 * from alumnos", connection);
			SqlDataReader dr = com.ExecuteReader();
			var alumno = new Alumno();

			while (dr.Read())
			{
				alumno.Nombres = dr["nombre"].ToString();


				//cmbProductCategory.Items.Add(name);
			}
			return alumno;
			//var count = (Alumno)com.ExecuteScalar();

			//connection.Close();

			//var alumnos = new List<Alumno>();

			//alumnos.Add(new Alumno()
			//{
			//	nombres = "Lautaro",
			//	apellidos = "Irañeta",
			//	libretaUniversitaria = "1018944",
			//	emails = new string[]
			//	{
			//		"lautaroiraneta@gmail.com",
			//		"liraneta@uade.edu.ar"
			//	},
			//	ciudad = "Wilde",
			//	estadoCivil = "En Pareja",
			//	experenciasLaborales = new ExperienciaLaboral[]
			//	{
			//		new ExperienciaLaboral() {
			//			actualmenteTrabajando = false,
			//			comentarios = "me gusto mucho",
			//			conocimientosAdquiridos = new string[] { "SQL" },
			//			empresa = "Google",
			//			fechaDesde = "07/2015",
			//			fechaHasta = "07/2017",
			//			puesto = "Software Engineer"
			//		}
			//	},
			//	experienciasEducativas = new ExperienciaEducativa[]
			//	{
			//		new ExperienciaEducativa()
			//		{
			//			alPresente = true,
			//			comentarios = "Comentarios11",
			//			estado = "En Curso",
			//			fechaFin = "Presente",
			//			fechaInicio = "03/2010",
			//			institucion = "UADE",
			//			tipo = "Universitario"
			//		},
			//		new ExperienciaEducativa()
			//		{
			//			alPresente = false,
			//			comentarios = "Comentarios 22",
			//			estado = "Completo",
			//			fechaFin = "12/2009",
			//			fechaInicio = "03/2006",
			//			institucion = "EET N°7 'José Hernández'",
			//			tipo = "Secundario"
			//		}
			//	},
			//	fechaDeNacimiento = "01/07/1991",
			//	idiomas = new Idioma[]
			//	{
			//		new Idioma()
			//		{
			//			comentarios = "Comentario idioma 1",
			//			idioma = "Inglés",
			//			nivelEscrito = "Básico",
			//			nivelOral = "Intermedio"
			//		}
			//	},
			//	interesesPersonales = "Mets, Raiders y Spurs",
			//	localidad = "Wilde",
			//	nombreUsuario = "lautaro.iraneta",
			//	numeroDocumento = "35.941.589",
			//	objetivoLaboral = "Hacer poco y ganar mucho.",
			//	otrosComentarios = new string[] { "No más comentarios" },
			//	paisNacionalidad = "Argentina",
			//	paisResidencia = "Argentina",
			//	provinciaResidencia = "Buenos Aires",
			//	redesSociales = new RedesSociales()
			//	{
			//		mostarFeedLinkedIn = false,
			//		mostrarFeedFacebook = true,
			//		mostrarFeedInstagram = true,
			//		mostrarFeedTwitter = true,
			//		usuarioFacebook = "lautaroiraneta",
			//		usuarioInstagram = "lautaaa",
			//		usuarioLinkedIn = "Lautaro Irañeta",
			//		usuarioTwitter = "__lauta"
			//	},
			//	telefonos = new string[] { "11-4207-1825" },
			//	tipoDocumento = "DNI",
			//	zona = "Sur"
			//});

			//return alumnos;
		}

		[HttpGet]
		[Route("/alumno/asd")]
		public Idioma asdd()
		{
			return new Idioma()
			{
				comentarios = "Comentario idioma 1",
				idioma = "Inglés",
				nivelEscrito = "Básico",
				nivelOral = "Intermedio"
			};
		}
		//[HttpGet]
		//public string Asd()
		//{
		//	return "hola";
		//}
	}
}