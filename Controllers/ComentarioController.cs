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
	public class ComentarioController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public ComentarioController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public void Post([FromBody] Comentario comentario)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into comentarios_x_tarea (usuario, comentario, tarea, alta) values (@usuario, @comentario, @tarea, @alta)", connection);

			com.Parameters.AddWithValue("@usuario", comentario.Usuario);
			com.Parameters.AddWithValue("@comentario", comentario.Contenido);
			com.Parameters.AddWithValue("@tarea", comentario.Tarea);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);
			com.ExecuteReader();

			connection.Close();
		}
	}
}