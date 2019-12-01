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
	public class PuestoController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public PuestoController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public void Post([FromBody] Puesto puesto)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("" +
				"insert into puestos (nombre, estado, alta) output INSERTED.ID" +
				" values (@nombre, @estado, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(puesto.Nombre) ? DBNull.Value.ToString() : puesto.Nombre);
			com.Parameters.AddWithValue("@estado", "Pendiente"); // Max 10 caract
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var empresaId = (int)com.ExecuteScalar();
			connection.Close();
		}

		[HttpGet]
		public List<Puesto> GetList()
		{
			var puestos = new List<Puesto>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();
			
			SqlCommand com = new SqlCommand("select id, nombre from puestos where estado in ('Activo','Pendiente')", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var puesto = new Puesto();
				puesto.Id = dr["id"].ToString();
				puesto.Nombre = dr["nombre"].ToString();
				puestos.Add(puesto);
			}

			connection.Close();

			return puestos;
		}
	}
}
