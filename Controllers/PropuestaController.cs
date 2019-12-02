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

			SqlCommand comPropuestasRequisitos = new SqlCommand("" +
				"insert into propuestas_requisitos (propuesta_caract, edad_min, excluyente_Edad_min, edad_max, excluyente_edad_max, disponibilidad_reubicacion, habilidades_personales, porcentaje_mat_apr, excluyente_porc, cantidad_mat_apr, excluyente_mat_apr, promedio, excluyente_promedio, anio_cursada, excluyente_anio_cursada, alta) output INSERTED.ID " +
				" values (@propuesta_caract, @edad_min, @excluyente_Edad_min, @edad_max, @excluyente_edad_max, @disponibilidad_reubicacion, @habilidades_personales, @porcentaje_mat_apr, @excluyente_porc, @cantidad_mat_apr, @excluyente_mat_apr, @promedio, @excluyente_promedio, @anio_cursada, @excluyente_anio_cursada, @alta)", connection);

			comPropuestasRequisitos.Parameters.AddWithValue("@propuesta_caract", propuestaCaractId);
			comPropuestasRequisitos.Parameters.AddWithValue("@edad_min", propuesta.EdadMin);
			comPropuestasRequisitos.Parameters.AddWithValue("@excluyente_Edad_min", propuesta.ExcluyenteEdadMin);
			comPropuestasRequisitos.Parameters.AddWithValue("@edad_max", propuesta.EdadMax);
			comPropuestasRequisitos.Parameters.AddWithValue("@excluyente_edad_max", propuesta.ExcluyenteEdadMax);
			comPropuestasRequisitos.Parameters.AddWithValue("@disponibilidad_reubicacion", propuesta.DisponibilidadReubicacion);
			comPropuestasRequisitos.Parameters.AddWithValue("@habilidades_personales", string.IsNullOrEmpty(propuesta.HabilidadesPersonales) ? DBNull.Value.ToString() : propuesta.HabilidadesPersonales);
			comPropuestasRequisitos.Parameters.AddWithValue("@porcentaje_mat_apr", propuesta.PorcentajeMatApr);
			comPropuestasRequisitos.Parameters.AddWithValue("@excluyente_porc", propuesta.ExcluyentePorc);
			comPropuestasRequisitos.Parameters.AddWithValue("@cantidad_mat_apr", propuesta.CantidadMatApr);
			comPropuestasRequisitos.Parameters.AddWithValue("@excluyente_mat_apr", propuesta.ExcluyenteMatApr);
			comPropuestasRequisitos.Parameters.AddWithValue("@promedio", propuesta.Promedio);
			comPropuestasRequisitos.Parameters.AddWithValue("@excluyente_promedio", propuesta.ExcluyentePromedio);
			comPropuestasRequisitos.Parameters.AddWithValue("@anio_cursada", propuesta.AnioCursada);
			comPropuestasRequisitos.Parameters.AddWithValue("@excluyente_anio_cursada", propuesta.ExcluyenteAnioCursada);
			comPropuestasRequisitos.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var propuestasRequisitosId = (int)comPropuestasRequisitos.ExecuteScalar();
			connection.Close();
			connection.Open();

			if (propuesta.Puestos?.Length > 0)
			{
				for (int i = 0; i < propuesta.Puestos.Length; ++i)
				{
					var puesto = propuesta.Puestos[i];
					if (puesto.Puesto != null)
					{
						SqlCommand comPuestoPropuestaReq = new SqlCommand("" +
						"insert into puestos_x_propuesta_req (puesto, propuesta_req, anios_exp, alta)" +
						" values (@puesto, @propuesta_req, @anios_exp, @alta)", connection);

						comPuestoPropuestaReq.Parameters.AddWithValue("@puesto", puesto.Puesto[0].Id);
						comPuestoPropuestaReq.Parameters.AddWithValue("@propuesta_req", propuestasRequisitosId);
						comPuestoPropuestaReq.Parameters.AddWithValue("@anios_exp", puesto.AniosExperiencia);
						comPuestoPropuestaReq.Parameters.AddWithValue("@alta", sqlFormattedDate);

						comPuestoPropuestaReq.ExecuteReader();

						connection.Close();
						connection.Open();
					}
				}
			}

			if (propuesta.Conocimientos?.Length > 0)
			{
				for (int i = 0; i < propuesta.Conocimientos.Length; ++i)
				{
					var conocimiento = propuesta.Conocimientos[i];
					if (conocimiento.Conocimiento != null)
					{
						SqlCommand comConocimientoPropuestaReq = new SqlCommand("" +
							"insert into conocimientos_x_propuesta_req (conocimiento, propuesta_req, tipo, anios_exp, alta) " +
							"values (@conocimiento, @propuesta_req, @tipo, @anios_exp, @alta)", connection);

						comConocimientoPropuestaReq.Parameters.AddWithValue("@conocimiento", conocimiento.Conocimiento[0].Id);
						comConocimientoPropuestaReq.Parameters.AddWithValue("@propuesta_req", propuestasRequisitosId);
						comConocimientoPropuestaReq.Parameters.AddWithValue("@tipo", "f");
						comConocimientoPropuestaReq.Parameters.AddWithValue("@anios_exp", conocimiento.AniosExperiencia);
						comConocimientoPropuestaReq.Parameters.AddWithValue("@alta", sqlFormattedDate);

						comConocimientoPropuestaReq.ExecuteReader();

						connection.Close();
						connection.Open();
					}
				}
			}

			if (propuesta.ConocimientosExtra?.Length > 0)
			{
				for (int i = 0; i < propuesta.ConocimientosExtra.Length; ++i)
				{
					var conocimiento = propuesta.ConocimientosExtra[i];

					SqlCommand comConocimientoExtra = new SqlCommand("" +
							"insert into conocimientos_x_propuesta_req (conocimiento, propuesta_req, tipo, alta) " +
							"values (@conocimiento, @propuesta_req, @tipo, @alta)", connection);

					comConocimientoExtra.Parameters.AddWithValue("@conocimiento", conocimiento.Id);
					comConocimientoExtra.Parameters.AddWithValue("@propuesta_req", propuestasRequisitosId);
					comConocimientoExtra.Parameters.AddWithValue("@tipo", "e");
					comConocimientoExtra.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comConocimientoExtra.ExecuteReader();

					connection.Close();
					connection.Open();
				}
			}

			connection.Close();
		}

	}
}
