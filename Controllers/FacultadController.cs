using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class FacultadController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public FacultadController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<IdValor> GetList()
		{
			var facultades = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from facultades", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var facultad = new IdValor();
				facultad.Id = dr["id"].ToString();
				facultad.Valor = dr["nombre"].ToString();
				facultades.Add(facultad);
			}

			connection.Close();

			return facultades;
		}
	}
}
