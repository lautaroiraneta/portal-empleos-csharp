using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TutorController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public TutorController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public List<IdValor> GetList()
		{
			var tutores = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select u.id as id, u.nombre + ' ' + u.apellido as nombre from usuarios u " +
				"inner join miembros_equipos me on me.usuario = u.id " +
				"inner join equipos e on e.id = me.equipo " +
				"inner join empresas em on em.id = e.empresa " +
			    "where me.baja is null and e.baja is null and u.baja is null " +
				"and e.Nombre = 'Tutores' and em.baja is null and em.id = 23", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var tutor = new IdValor();
				tutor.Id = dr["id"].ToString();
				tutor.Valor = dr["nombre"].ToString();
				tutores.Add(tutor);
			}

			connection.Close();

			return tutores;
		}
	}
}
