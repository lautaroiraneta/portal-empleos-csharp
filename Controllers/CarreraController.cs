using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PortalEmpleos.Models;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CarreraController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<CarreraController> _logger;

		public CarreraController(IConfiguration configuration, ILogger<CarreraController> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}
		
		[HttpPost]
		public void Post([FromBody] Carrera carrera)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into carreras (nombre, codigo, alta) values (@nombre, @codigo, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", carrera.Nombre);
			com.Parameters.AddWithValue("@codigo", carrera.Codigo);
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
	}
}