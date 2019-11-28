using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class IdiomaController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public IdiomaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<IdValor> GetList()
		{
			var idiomas = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre_idioma from idiomas", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var idioma = new IdValor();
				idioma.Id = dr["id"].ToString();
				idioma.Valor = dr["nombre_idioma"].ToString();
				idiomas.Add(idioma);
			}

			connection.Close();

			return idiomas;
		}
	}
}
