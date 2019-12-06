using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
				"insert into propuestas_caract_propias (empresa, titulo, carreras_afines, pais, provincia, zona, ciudad, localidad, sueldo_bruto, tipo_empleo, turno_empleo, beneficios, fecha_finalizacion, descripcion, alta) output INSERTED.ID" +
				" values (@empresa, @titulo, @carreras_afines, @pais, @provincia, @zona, @ciudad, @localidad, @sueldo_bruto, @tipo_empleo, @turno_empleo, @beneficios, @fecha_finalizacion, @descripcion, @alta)", connection);

			com.Parameters.AddWithValue("@empresa", propuesta.Empresa);
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
						"insert into puestos_x_propuesta_req (puesto, propuesta_req, anios_exp, excluyente, alta)" +
						" values (@puesto, @propuesta_req, @anios_exp, @excluyente, @alta)", connection);

						comPuestoPropuestaReq.Parameters.AddWithValue("@puesto", puesto.Puesto[0].Id);
						comPuestoPropuestaReq.Parameters.AddWithValue("@propuesta_req", propuestasRequisitosId);
						comPuestoPropuestaReq.Parameters.AddWithValue("@anios_exp", puesto.AniosExperiencia);
						comPuestoPropuestaReq.Parameters.AddWithValue("@excluyente", puesto.Excluyente);
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
							"insert into conocimientos_x_propuesta_req (conocimiento, propuesta_req, tipo, anios_exp, excluyente, alta) " +
							"values (@conocimiento, @propuesta_req, @tipo, @anios_exp, @excluyente, @alta)", connection);

						comConocimientoPropuestaReq.Parameters.AddWithValue("@conocimiento", conocimiento.Conocimiento[0].Id);
						comConocimientoPropuestaReq.Parameters.AddWithValue("@propuesta_req", propuestasRequisitosId);
						comConocimientoPropuestaReq.Parameters.AddWithValue("@tipo", "f");
						comConocimientoPropuestaReq.Parameters.AddWithValue("@anios_exp", conocimiento.AniosExperiencia);
						comConocimientoPropuestaReq.Parameters.AddWithValue("@excluyente", conocimiento.Excluyente);
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
							"insert into conocimientos_x_propuesta_req (conocimiento, propuesta_req, tipo, excluyente, alta) " +
							"values (@conocimiento, @propuesta_req, @tipo, @excluyente, @alta)", connection);

					comConocimientoExtra.Parameters.AddWithValue("@conocimiento", conocimiento.Id);
					comConocimientoExtra.Parameters.AddWithValue("@propuesta_req", propuestasRequisitosId);
					comConocimientoExtra.Parameters.AddWithValue("@tipo", "e");
					comConocimientoExtra.Parameters.AddWithValue("@excluyente", conocimiento.Excluyente);
					comConocimientoExtra.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comConocimientoExtra.ExecuteReader();

					connection.Close();
					connection.Open();
				}
			}

			connection.Close();
		}

		[HttpGet]
		public List<PropuestaView> GetList()
		{
			var propuestas = new List<PropuestaView>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select pro.id as id, pro.titulo as titulo, " +
				"e.nombre as nombre_empresa, " +
				"e.id as id_empresa, " +
				"pro.alta as fecha_posteo, " +
				"datediff(dd,pro.alta,getdate()) as dias_dif, " +
				"p.nombre as nombre_prov, " +
				"z.nombre as nombre_zona, " +
				"c.nombre as nombre_ciudad, " +
				"l.nombre as nombre_localidad, " +
				"tipo_empleo as tipo_empleo, " +
				"turno_empleo as turno_empleo " +
				"from propuestas_caract_propias pro " +
				"inner join empresas e on e.id=pro.empresa " +
				"left join provincias p on p.id=pro.provincia " +
				"left join zonas z on z.id=pro.zona " +
				"left join ciudades c on c.id=pro.ciudad " +
				"left join localidades l on l.id=pro.localidad " +
				"where pro.baja is null", connection);

			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var propuesta = new PropuestaView();
				propuesta.Id = dr["id"].ToString();
				propuesta.Titulo = dr["titulo"].ToString();
				propuesta.Empresa = new IdValor { Id = dr["id_empresa"].ToString(), Valor = dr["nombre_empresa"].ToString() };
				propuesta.FechaPosteo = Convert.ToDateTime(dr["fecha_posteo"].ToString());
				propuesta.DiasDif = Convert.ToInt32(dr["dias_dif"].ToString());
				propuesta.Provincia = dr["nombre_prov"].ToString();
				propuesta.Zona = dr["nombre_zona"].ToString();
				propuesta.Ciudad = dr["nombre_ciudad"].ToString();
				propuesta.Localidad = dr["nombre_localidad"].ToString();
				propuesta.TipoEmpleo = dr["tipo_empleo"].ToString();
				propuesta.TurnoEmpleo = dr["turno_empleo"].ToString();

				propuestas.Add(propuesta);
			}

			connection.Close();
			

			if (propuestas.Count() > 0)
			{
				for (int i = 0; i < propuestas.Count(); ++i)
				{
					var propuesta = propuestas[i];
					connection.Open();
					SqlCommand com2 = new SqlCommand("select " +
					 "pro.id as id_propuesta, " +
					 "c.id as id_carrera, " +
					 "c.nombre as nombre_carrera " +
					 "from carreras c " +
					 "inner join carreras_x_propuesta cxp on cxp.carrera=c.id " +
					 "inner join propuestas_caract_propias pro on pro.id=cxp.propuesta_caract " +
					 "where cxp.baja is null and c.baja is null and pro.baja is null and pro.id = @idPropuesta", connection);

					com2.Parameters.AddWithValue("@idPropuesta", propuesta.Id);

					SqlDataReader dr2 = com2.ExecuteReader();

					if (dr2.HasRows)
					{
						var carreras = new List<IdValor>();

						while (dr2.Read())
						{
							carreras.Add(new IdValor { Id = dr2["id_carrera"].ToString(), Valor = dr2["nombre_carrera"].ToString() });
						}

						propuesta.Carreras = carreras.ToArray();
					}
					connection.Close();
				}
			}			

			connection.Close();

			return propuestas;
		}

		[HttpGet]
		[Route("get-by-id")]
		public PropuestaVista GetById([FromQuery] string propuestaId)
		{
			var propuesta = new PropuestaVista();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select pro.id as id_propuesta, " +
				"e.id as empresa_id, e.nombre as empresa_nombre, " +
				"pro.titulo as titulo, " +
				"p.nombre as nombre_prov, " +
				"z.nombre as nombre_zona, " +
				"c.nombre as nombre_ciudad, " +
				"l.nombre as nombre_localidad, " +
				"pro.tipo_empleo as tipo_empleo, " +
				"case when pro.tipo_empleo='Full-Time' then '' else pro.turno_empleo end as turno_empleo, " +
				"'Fecha de alta: ' + convert(varchar,pro.alta,103) as fecha_alta, " +
				"pro.sueldo_bruto as sueldo_bruto, " +
				"pro.beneficios as beneficios, " +
				"pro.descripcion as desc_propuesta, " +
				"convert(varchar(3),prr.edad_min) + case when excluyente_Edad_min =1 then ' (Excluyente) ' else '' end as edad_min, " +
				"convert(varchar(3),prr.edad_max) + case when excluyente_Edad_max =1 then ' (Excluyente) ' else '' end as edad_max, " +
				"case when prr.disponibilidad_reubicacion=1 then 'Disponibilidad a Reubicarse' else null end " +
				" + case when prr.disponibilidad_reubicacion_Excluyente =1 then ' (Excluyente) ' else '' end as reubicarse, " +
				"prr.habilidades_personales as habilidades_personales, " +
				"convert(varchar(20),prr.porcentaje_mat_apr) + case when excluyente_porc =1 then ' (Excluyente) ' else '' end as porcentaje_mat_apr, " +
				"convert(varchar(20),prr.cantidad_mat_apr) + case when excluyente_mat_apr =1 then ' (Excluyente) ' else '' end as cant_mat_apr, " +
				"convert(varchar(20),prr.promedio) + case when excluyente_promedio =1 then ' (Excluyente) ' else '' end as promedio, " +
				"convert(varchar(20),prr.anio_cursada) + case when excluyente_anio_cursada =1 then ' (Excluyente) ' else '' end as anio_cursada " +
				"from propuestas_caract_propias pro " +
				"left join propuestas_requisitos prr on pro.id=prr.propuesta_caract " +
				"inner join empresas e on pro.empresa=e.id " +
				"left join provincias p on p.id=pro.provincia " +
				"left join zonas z on z.id=pro.zona " +
				"left join ciudades c on c.id=pro.ciudad " +
				"left join localidades l on l.id=pro.localidad " +
				"where prr.baja is null and e.baja is null and pro.id = @id", connection);

			com.Parameters.AddWithValue("@id", propuestaId);

			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				propuesta.Id = dr["id_propuesta"].ToString();
				propuesta.Empresa = new IdValor { Id = dr["empresa_id"].ToString(), Valor = dr["empresa_nombre"].ToString() };
				propuesta.Titulo = dr["titulo"].ToString();
				propuesta.Provincia = dr["nombre_prov"].ToString();
				propuesta.Zona = dr["nombre_zona"].ToString();
				propuesta.Ciudad = dr["nombre_ciudad"].ToString();
				propuesta.Localidad = dr["nombre_localidad"].ToString();
				propuesta.TipoEmpleo = dr["tipo_empleo"].ToString();
				propuesta.TurnoEmpleo = dr["turno_empleo"].ToString();
				propuesta.FechaAlta = dr["fecha_alta"].ToString();
				propuesta.SueldoBruto = dr["sueldo_bruto"].ToString();
				propuesta.Beneficios = dr["beneficios"].ToString();
				propuesta.Descripcion = dr["desc_propuesta"].ToString();
				propuesta.EdadMin = dr["edad_min"].ToString();
				propuesta.EdadMax = dr["edad_max"].ToString();
				propuesta.Reubicarse = dr["reubicarse"].ToString();
				propuesta.HabilidadesPersonales = dr["habilidades_personales"].ToString();
				propuesta.PorcentajeMatApr = dr["porcentaje_mat_apr"].ToString();
				propuesta.CantidadMatApr = dr["cant_mat_apr"].ToString();
				propuesta.Promedio = dr["promedio"].ToString();
				propuesta.AnioCursada = dr["anio_cursada"].ToString();
			}

			connection.Close();
			connection.Open();

			SqlCommand com2 = new SqlCommand("select  " +
				"c.nombre + case when pro.carreras_afines = 1 then ' y afines' else '' end as carrera " +
				"from " +
				"carreras c  " +
				"inner join carreras_x_propuesta cxp on cxp.carrera=c.id " +
				"inner join propuestas_caract_propias pro on pro.id=cxp.propuesta_caract " +
				"where pro.id=@id and cxp.baja is null and c.baja is null", connection);

			com2.Parameters.AddWithValue("@id", propuesta.Id);
			SqlDataReader dr2 = com2.ExecuteReader();

			if (dr2.HasRows)
			{
				var carreras = new List<string>();
				while (dr2.Read())
				{
					carreras.Add(dr2["carrera"].ToString());
				}
				propuesta.Carreras = carreras.ToArray();
			}
			
			connection.Close();
			connection.Open();

			SqlCommand com3 = new SqlCommand("select  " +
				"c.nombre " +
				"+ case when cpr.tipo='e' then '' else ': ' + convert(varchar(5),cpr.anios_exp) + ' años de experiencia' end " +
				"+ case when cpr.excluyente=1 then ' (Excluyente)' else ''  end as conocimiento " +
				"from  " +
				"conocimientos_x_propuesta_req cpr  " +
				"inner join conocimientos c on c.id=cpr.conocimiento " +
				"inner join propuestas_requisitos pr on pr.id=cpr.propuesta_req " +
				"where pr.propuesta_caract=@id and cpr.baja is null and c.baja is null and pr.baja is null", connection);

			com3.Parameters.AddWithValue("@id", propuesta.Id);
			SqlDataReader dr3 = com3.ExecuteReader();

			if (dr3.HasRows)
			{
				var conocimientos = new List<string>();
				while (dr3.Read())
				{
					conocimientos.Add(dr3["conocimiento"].ToString());
				}
				propuesta.Conocimientos = conocimientos.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand com4 = new SqlCommand("select  " +
				"c.nombre  " +
				"+ case when anios_exp is not null then ': ' + convert(varchar(5),cpr.anios_exp) + ' años de experiencia' else '' end " +
				"+ case when cpr.excluyente=1 then ' (Excluyente)' else ''  end as puesto " +
				"from  " +
				"puestos_x_propuesta_req cpr  " +
				"inner join puestos c on c.id=cpr.puesto " +
				"inner join propuestas_requisitos pr on pr.id=cpr.propuesta_req " +
				"where pr.propuesta_caract=@id and cpr.baja is null and c.baja is null and pr.baja is null ", connection);

			com4.Parameters.AddWithValue("@id", propuesta.Id);
			SqlDataReader dr4 = com4.ExecuteReader();

			if (dr4.HasRows)
			{
				var puestosReq = new List<string>();
				while (dr4.Read())
				{
					puestosReq.Add(dr4["puesto"].ToString());
				}
				propuesta.PuestosReq = puestosReq.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand com5 = new SqlCommand("select  " +
				"c.nombre as puesto " +
				"from " +
				"puestos_x_propuesta_caract cpr " +
				"inner join puestos c on c.id=cpr.puesto " +
				"inner join propuestas_caract_propias pr on pr.id=cpr.propuesta_caract " +
				"where pr.id=@id and cpr.baja is null and c.baja is null and pr.baja is null", connection);

			com5.Parameters.AddWithValue("@id", propuesta.Id);
			SqlDataReader dr5 = com5.ExecuteReader();

			if (dr5.HasRows)
			{
				var puestosCarac = new List<string>();
				while (dr5.Read())
				{
					puestosCarac.Add(dr5["puesto"].ToString());
				}
				propuesta.PuestosCarac = puestosCarac.ToArray();
			}

			connection.Close();

			return propuesta;
		}
	}
}
