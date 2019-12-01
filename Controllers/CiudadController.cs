using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CiudadController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public CiudadController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<IdValor> GetList([FromQuery] string prov, [FromQuery] string zona)
		{
			var ciudades = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from ciudades where provincia = @prov and zona = @zona", connection);
			com.Parameters.AddWithValue("@prov", prov);
			com.Parameters.AddWithValue("@zona", zona);
			
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var ciudad = new IdValor();
				ciudad.Id = dr["id"].ToString();
				ciudad.Valor = dr["nombre"].ToString();
				ciudades.Add(ciudad);
			}

			connection.Close();

			return ciudades;
		}
	}
}
