using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PortalEmpleos.Models;

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
		public Alumno Post([FromBody] Alumno alumno)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into alumnos (nombre, apellido, libretaUniversitaria, email, tipoDocumento, documento, nombreUsuario, alta) output INSERTED.ID " +
											"values (@nombre, @apellido, @libretaUniversitaria, @email, @tipoDocumento, @documento, @nombreUsuario, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", alumno.Nombres);
			com.Parameters.AddWithValue("@apellido", alumno.Apellidos);
			com.Parameters.AddWithValue("@libretaUniversitaria", alumno.LibretaUniversitaria);
			com.Parameters.AddWithValue("@email", alumno.Email);
			com.Parameters.AddWithValue("@tipoDocumento", alumno.TipoDocumento);
			com.Parameters.AddWithValue("@documento", alumno.NumeroDocumento);
			com.Parameters.AddWithValue("@nombreUsuario", alumno.NombreUsuario);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var id = (int)com.ExecuteScalar();

			connection.Close();

			var alumnox = new Alumno();
			alumnox.Id = id.ToString();
			alumnox.Nombres = alumno.Nombres;
			alumnox.Apellidos = alumno.Apellidos;

			return alumnox;
		}

		[Route("/alumno/get-id-by-usuario")]
		[HttpPost]
		public Alumno GetId([FromBody] IdValor usuario)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre, apellido from alumnos where nombreUsuario = @nombreUsuario", connection);
			com.Parameters.AddWithValue("@nombreUsuario", usuario.Valor);
			SqlDataReader dr = com.ExecuteReader();

			Alumno alumno = new Alumno();
			while (dr.Read())
			{
				alumno.Id = dr["id"].ToString();
				alumno.Nombres = dr["nombre"].ToString();
				alumno.Apellidos = dr["apellido"].ToString();
			}
			connection.Close();

			return alumno;
		}
	}
}