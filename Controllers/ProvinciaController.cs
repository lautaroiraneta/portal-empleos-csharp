using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ProvinciaController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public ProvinciaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<IdValor> GetList()
		{
			var provincias = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from provincias", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var provincia = new IdValor();
				provincia.Id = dr["id"].ToString();
				provincia.Valor = dr["nombre"].ToString();
				provincias.Add(provincia);
			}

			connection.Close();

			return provincias;
		}
	}
}
