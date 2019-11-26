using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PaisController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public PaisController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<IdValor> GetList()
		{
			var paises = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from paises", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var pais = new IdValor();
				pais.Id = dr["id"].ToString();
				pais.Valor = dr["nombre"].ToString();
				paises.Add(pais);
			}

			connection.Close();

			return paises;
		}
	}
}
