using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;
using System.Collections.Generic;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class EtapaSeleccionAlumnoController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public EtapaSeleccionAlumnoController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public List<EtapaSeleccionAlumno> GetList()
		{
			var etapas = new List<EtapaSeleccionAlumno>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select " +
				"esa.id as etapa_id, ua.nombre + ' ' + ua.apellido + ' (' + a.libretaUniversitaria + ')' as alumno, " +
				"c.nombre as carrera, e.id as empresa_id, e.nombre as empresa, pro.id as propuesta_id, pro.titulo as propuesta_titulo, " +
				"convert(varchar, esa.alta, 103) as fecha_postulacion, es.nombre as estado_etapa from etapas_seleccion_alumnos esa " +
				"left join estados_seleccion es on esa.estado = es.id " +
				"left join usuarios ua on ua.id = esa.alumno " +
				"left join alumnos a on ua.alumno = a.id " +
				"left join perfil p on p.alumno = ua.alumno " +
				"left join carreras c on c.id = p.carrera " +
				"left join empresas e on e.id = esa.EMPRESA " +
				"left join propuestas_caract_propias pro on pro.id = esa.propuesta " +
				"where esa.baja is null and es.baja is null and ua.baja is null and p.baja is null and c.baja is null and e.baja is null and pro.baja is null", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var etapa = new EtapaSeleccionAlumno();
				etapa.Id = dr["etapa_id"].ToString();
				etapa.Alumno = dr["alumno"].ToString();
				etapa.Carrera = dr["carrera"].ToString();
				etapa.Empresa = new IdValor { Id = dr["empresa_id"].ToString(), Valor = dr["empresa"].ToString() };
				etapa.EstadoEtapa = dr["estado_etapa"].ToString();
				etapa.EtapaId = dr["etapa_id"].ToString();
				etapa.FechaPostulacion = dr["fecha_postulacion"].ToString();
				etapa.Propuesta = new IdValor { Id = dr["propuesta_id"].ToString(), Valor = dr["propuesta_titulo"].ToString() };
				etapas.Add(etapa);
			}

			connection.Close();

			return etapas;
		}

		[HttpPost]
		[Route("postulacion")]
		public void SetPostulacion([FromBody] Postulacion postulacion)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into postulaciones (alumno, propuesta, perfil, alta) " +
				"values (@alumno, @propuesta, (select id from perfil where alumno = @alumno), @alta)", connection);
			com.Parameters.AddWithValue("@alumno", postulacion.AlumnoId);
			com.Parameters.AddWithValue("@propuesta", postulacion.PropuestaId);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			com.ExecuteReader();

			CrearEtapaSeleccionAlumno(postulacion.AlumnoId, postulacion.PropuestaId, postulacion.EmpresaId);

			connection.Close();
		}

		private void CrearEtapaSeleccionAlumno(string alumnoId, string propuestaId, string empresaId)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand comAdminEmpresa = new SqlCommand("select top 1 u.id from usuarios u" +
				" inner join miembros_equipos me on me.usuario = u.id" +
				" inner join equipos e on me.equipo = e.id" +
				 " where me.baja is null" +
				 " and u.baja is null" +
				 " and e.empresa = @empresa" +
				 " and e.baja is null" +
				 " and e.nombre = 'Seleccionador de alumnos'" +
				 " and e.tipo_equipo = 'e'" +
				 " order by(select count(1) from etapas_seleccion_alumnos where admin_empresa = u.id) asc", connection);

			comAdminEmpresa.Parameters.AddWithValue("@empresa", empresaId);

			SqlDataReader dr = comAdminEmpresa.ExecuteReader();
			var adminEmpresaId = string.Empty;
			while (dr.Read())
			{
				adminEmpresaId = dr["id"].ToString();
			}
			connection.Close();
			connection.Open();

			SqlCommand comAdminUni = new SqlCommand("select top 1 u.id from usuarios u" +
				" inner join miembros_equipos me on me.usuario = u.id" +
				" inner join equipos e on me.equipo = e.id" +
				 " where me.baja is null" +
				 " and u.baja is null" +
				 " and e.baja is null" +
				 " and e.nombre = 'Seleccionador de alumnos'" +
				 " and e.tipo_equipo = 'u'" +
				 " order by(select count(1) from etapas_seleccion_alumnos where admin_universidad = u.id) asc", connection);

			SqlDataReader dr2 = comAdminUni.ExecuteReader();
			var adminUniId = string.Empty;
			while (dr2.Read())
			{
				adminUniId = dr2["id"].ToString();
			}

			connection.Close();
			connection.Open();

			SqlCommand sqlTipoEmpleo = new SqlCommand("select tipo_empleo as tipo_empleo from propuestas_caract_propias where id=@id", connection);
			sqlTipoEmpleo.Parameters.AddWithValue("@id", propuestaId);

			SqlDataReader drte = sqlTipoEmpleo.ExecuteReader();
			var esPasantia = false;
			if (drte.HasRows)
			{
				while (drte.Read())
				{
					if (drte["tipo_empleo"].ToString() == "Pasantía")
					{
						esPasantia = true;
					}
				}				
			}
			connection.Close();
			string etapaDefinicionConvenio = string.Empty;
			if (esPasantia)
			{
				connection.Open();

				SqlCommand comEtapaDefCon = new SqlCommand("select c.id as id " +
					"from convenios c " +
					"inner join etapas_definicion_convenio edc on c.etapa_definicion = edc.id " +
					"where edc.baja is null and edc.estado = 4 " +
					//"and((exists(select 1 from carreras ca where ca.id = (select carrera from perfil where alumno = @alumno))) or " +
					//"c.facultad is null) " 
					"and edc.empresa = @empresa and c.baja is null", connection);

				//comEtapaDefCon.Parameters.AddWithValue("@alumno", propuestaId);
				comEtapaDefCon.Parameters.AddWithValue("@empresa", empresaId);

				SqlDataReader drEtapaDefCon = comEtapaDefCon.ExecuteReader();
				if (drEtapaDefCon.HasRows)
				{
					while(drEtapaDefCon.Read())
					{
						etapaDefinicionConvenio = drEtapaDefCon["id"].ToString();
					}
				}

				connection.Close();
			}
			connection.Open();

			SqlCommand com = new SqlCommand("" +
				"insert into etapas_seleccion_alumnos (propuesta, admin_empresa, admin_universidad, alumno, estado, alta, etapas_definicion_convenio, EMPRESA) output INSERTED.ID" +
				" values (@propuesta, @admin_empresa, @admin_universidad, (select id as id from usuarios where alumno = @alumno), @estado, @alta, @etapas_definicion_convenio, @EMPRESA)", connection);

			com.Parameters.AddWithValue("@propuesta", propuestaId);
			com.Parameters.AddWithValue("@admin_empresa", adminEmpresaId);
			com.Parameters.AddWithValue("@admin_universidad", adminUniId);
			com.Parameters.AddWithValue("@alumno", alumnoId);
			com.Parameters.AddWithValue("@estado", "1");
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);
			com.Parameters.AddWithValue("@etapas_definicion_convenio", esPasantia ? etapaDefinicionConvenio : DBNull.Value.ToString());
			com.Parameters.AddWithValue("@EMPRESA", empresaId);

			var etapaId = (int)com.ExecuteScalar();

			connection.Close();
			connection.Open();

			SqlCommand com2 = new SqlCommand("insert into tareas " +
				"(nombre, descripcion, estado, creador, responsable, etapa_definicion_convenio, fecha_fin, alta, baja, etapa_seleccion_alumno, etapa_ingreso_alumno, etapa_pasantia_en_curso, etapa_cierre, tarea_defecto) " +
				"select nombre, descripcion, 1, " +
				"CASE WHEN tipo_responsable='e' then(select admin_empresa from etapas_seleccion_alumnos where id = @id) " +
				"WHEN tipo_responsable='a' then (select id as id from usuarios where alumno = @alumno) " +
				"else (select admin_universidad from etapas_seleccion_alumnos where id = @id) " +
				"end," +
			   "CASE WHEN tipo_responsable = 'e' then(select admin_empresa from etapas_seleccion_alumnos where id = @id) " +
				"WHEN tipo_responsable='a' then (select id as id from usuarios where alumno = @alumno) " +
			   "else (select admin_universidad from etapas_seleccion_alumnos where id = @id) " +
			   "end," +
			   "null," +
			   "getdate() + 30," +
			   "getdate()," +
			   "null, " +
			   "@id, " +
			   "null, " +
			   "null, " +
			   "null, " +
			   "id " +
			   "from tareas_x_defecto " +
			   "where etapa = 2 and baja is null", connection);

			com2.Parameters.AddWithValue("@id", etapaId);
			com2.Parameters.AddWithValue("@alumno", alumnoId);

			com2.ExecuteReader();
			connection.Close();
			connection.Open();

			SqlCommand com3 = new SqlCommand("insert into relacion_tareas (tarea_pred, tarea_suce, alta, baja)" +
				" select t1.id, t2.id, getdate(), null from relacion_tareas_x_defecto rtd" +
				" inner join tareas_x_defecto td1 on rtd.tarea_pred = td1.id" +
				" inner join tareas_x_defecto td2 on rtd.tarea_suce = td2.id" +
				" inner join tareas t1 on t1.tarea_defecto = td1.id" +
				" inner join tareas t2 on t2.tarea_defecto = td2.id" +
				" where t1.etapa_seleccion_alumno = @id and t2.etapa_seleccion_alumno = @id" +
				" and td2.baja is null and td1.baja is null and t1.baja is null and t2.baja is null and rtd.baja is null", connection);

			com3.Parameters.AddWithValue("@id", etapaId);
			com3.ExecuteReader();

			connection.Close();
		}
	}
}