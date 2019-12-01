using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class LocalidadController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public LocalidadController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<IdValor> GetList([FromQuery] string ciudad)
		{
			var localidades = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from localidades where ciudad = @ciudad", connection);
			com.Parameters.AddWithValue("@ciudad", ciudad);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var localidad = new IdValor();
				localidad.Id = dr["id"].ToString();
				localidad.Valor = dr["nombre"].ToString();
				localidades.Add(localidad);
			}

			connection.Close();

			return localidades;
		}
	}
}
