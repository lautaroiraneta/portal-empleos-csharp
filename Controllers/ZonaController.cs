using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ZonaController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public ZonaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<IdValor> GetList()
		{
			var zonas = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from zonas where provincia = 2", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var zona = new IdValor();
				zona.Id = dr["id"].ToString();
				zona.Valor = dr["nombre"].ToString();
				zonas.Add(zona);
			}

			connection.Close();

			return zonas;
		}
	}
}
