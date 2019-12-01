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
				"insert into propuestas_caract_propias (titulo, carreras_afines, pais, provincia, zona, ciudad, localidad, sueldo_bruto, tipo_empleo, turno_empleo, beneficios, fecha_finalizacion, descripcion, alta) output INSERTED.ID" +
				" values (@titulo, @carreras_afines, @pais, @provincia, @zona, @ciudad, @localidad, @sueldo_bruto, @tipo_empleo, @turno_empleo, @beneficios, @fecha_finalizacion, @descripcion, @alta)", connection);

			com.Parameters.AddWithValue("@titulo", string.IsNullOrEmpty(propuesta.Titulo) ? DBNull.Value.ToString() : propuesta.Titulo);
			com.Parameters.AddWithValue("@carreras_afines", propuesta.CarrerasAfines);
			com.Parameters.AddWithValue("@pais", propuesta.Pais?.Length > 0 ? propuesta.Pais[0].Id : "1");
			com.Parameters.AddWithValue("@provincia", propuesta.Provincia?.Length > 0 ? propuesta.Provincia[0].Id : "1");
			com.Parameters.AddWithValue("@zona", propuesta.Zona?.Length > 0 ? propuesta.Zona[0].Id : "1");
			com.Parameters.AddWithValue("@ciudad", propuesta.Ciudad?.Length > 0 ? propuesta.Ciudad[0].Id : "1");
			com.Parameters.AddWithValue("@localidad", propuesta.Localidad?.Length > 0 ? propuesta.Localidad[0].Id : "1");
			com.Parameters.AddWithValue("@sueldo_bruto", propuesta.SueldoBruto);
			com.Parameters.AddWithValue("@tipo_empleo", propuesta.TipoEmpleo?.Length > 0 ? propuesta.TipoEmpleo[0].Valor : DBNull.Value.ToString());
			com.Parameters.AddWithValue("@turno_empleo", propuesta.Turno?.Length > 0 ? propuesta.Turno[0].Valor : DBNull.Value.ToString());
			com.Parameters.AddWithValue("@beneficios", string.IsNullOrEmpty(propuesta.Beneficios) ? DBNull.Value.ToString() : propuesta.Beneficios);
			com.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(propuesta.Descripcion) ? DBNull.Value.ToString() : propuesta.Descripcion);
			com.Parameters.AddWithValue("@fecha_finalizacion", propuesta.FechaFinalizacionDT);
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
