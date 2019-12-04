using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DocenteGuiaController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public DocenteGuiaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public List<IdValor> GetList()
		{
			var docentesGuia = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select u.id as id, u.nombre + ' ' + u.apellido as nombre from usuarios u " +
				"inner join miembros_equipos me on me.usuario = u.id " +
				"inner join equipos e on e.id = me.equipo " +
				"where me.baja is null and e.baja is null and u.baja is null " +
				"and e.Nombre = 'Docentes guía' ", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var docenteGuia = new IdValor();
				docenteGuia.Id = dr["id"].ToString();
				docenteGuia.Valor = dr["nombre"].ToString();
				docentesGuia.Add(docenteGuia);
			}

			connection.Close();

			return docentesGuia;
		}
	}
}
