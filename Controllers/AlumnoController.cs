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
		}
	}
}