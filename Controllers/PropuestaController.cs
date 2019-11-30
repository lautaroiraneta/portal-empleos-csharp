using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PropuestaController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public PropuestaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public void Post([FromBody] Propuesta propuesta)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("" +
				"insert into propuestas_caract_propias (carreras_afines, alta) output INSERTED.ID" +
				" values (@carreras_afines, @alta)", connection);

			com.Parameters.AddWithValue("@carreras_afines", propuesta.CarrerasAfines);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var propuestaCaractId = (int)com.ExecuteScalar();
			connection.Close();
			connection.Open();
			if (propuesta.PuestosCarac != null)
			{
				for (int i = 0; i < propuesta.PuestosCarac.Length; ++i)
				{
					var puestoCarac = propuesta.PuestosCarac[i];

					SqlCommand comPuestosCarac = new SqlCommand("" +
						"insert into puestos_x_propuesta_caract (puesto, propuesta_caract, alta)" +
						" values (@puesto, @propuesta_caract, @alta)", connection);

					comPuestosCarac.Parameters.AddWithValue("@puesto", puestoCarac.Id);
					comPuestosCarac.Parameters.AddWithValue("@propuesta_caract", propuestaCaractId);
					comPuestosCarac.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comPuestosCarac.ExecuteReader();
					connection.Close();
					connection.Open();
				}
			}

			if (propuesta.Carreras != null)
			{
				for (int i = 0; i < propuesta.Carreras.Length; ++i)
				{
					var carrera = propuesta.Carreras[i];

					SqlCommand comCarrerasPropuesta = new SqlCommand("" +
						"insert into carreras_x_propuesta (carrera, propuesta_caract, alta)" +
						" values (@carrera, @propuesta_caract, @alta)", connection);

					comCarrerasPropuesta.Parameters.AddWithValue("@carrera", carrera.Id);
					comCarrerasPropuesta.Parameters.AddWithValue("@propuesta_caract", propuestaCaractId);
					comCarrerasPropuesta.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comCarrerasPropuesta.ExecuteReader();
					connection.Close();
					connection.Open();
				}
			}

			connection.Close();
		}

	}
}
