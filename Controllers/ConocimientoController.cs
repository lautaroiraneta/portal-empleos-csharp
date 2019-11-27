using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ConocimientoController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public ConocimientoController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public void Post([FromBody] Conocimiento conocimiento)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("" +
				"insert into conocimientos (nombre, estado, alta) output INSERTED.ID" +
				" values (@nombre, @estado, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(conocimiento.Nombre) ? DBNull.Value.ToString() : conocimiento.Nombre);
			com.Parameters.AddWithValue("@estado", "Pendiente");
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var id = (int)com.ExecuteScalar();
			connection.Close();
		}

		[HttpGet]
		public List<Conocimiento> GetList()
		{
			var conocimientos = new List<Conocimiento>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from conocimientos", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var conocimiento = new Conocimiento();
				conocimiento.Id = dr["id"].ToString();
				conocimiento.Nombre = dr["nombre"].ToString();
				conocimientos.Add(conocimiento);
			}

			connection.Close();

			return conocimientos;
		}
	}
}
