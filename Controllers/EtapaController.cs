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
	public class EtapaController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public EtapaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		[Route("get-data")]
		public Etapa GetData([FromQuery] string etapaDefinicionId)
		{
			var etapa = new Etapa();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select u.id,'Administrador de Universidad: ' + u.nombre + ' ' + u.apellido as nombre " +
				"from usuarios u " +
				"inner join etapas_definicion_convenio ec on u.id = ec.admin_universidad " +
				"where ec.baja IS NULL and u.baja IS NULL and ec.id = @etapadefinicion", connection);

			com.Parameters.AddWithValue("@etapadefinicion", etapaDefinicionId);

			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var adminstradorUniversidad = new IdValor();
				adminstradorUniversidad.Id = dr["id"].ToString();
				adminstradorUniversidad.Valor = dr["nombre"].ToString();
				etapa.AdministradorUniversidad = adminstradorUniversidad;
			}

			connection.Close();
			connection.Open();

			SqlCommand comAdminEmp = new SqlCommand("select u.id as id, 'Administrador de Empresa: ' + u.nombre + ' ' + u.apellido  as nombre " +
				"from usuarios u " +
				"inner join etapas_definicion_convenio ec on u.id = ec.admin_empresa " + 
				"where ec.baja IS NULL and u.baja IS NULL and ec.id = @etapadefinicion", connection);

			comAdminEmp.Parameters.AddWithValue("@etapadefinicion", etapaDefinicionId);

			SqlDataReader drAdminEmp = comAdminEmp.ExecuteReader();

			while (drAdminEmp.Read())
			{
				var administradorEmpresa = new IdValor();
				administradorEmpresa.Id = drAdminEmp["id"].ToString();
				administradorEmpresa.Valor = drAdminEmp["nombre"].ToString();
				etapa.AdministradorEmpresa = administradorEmpresa;
			}

			connection.Close();
			connection.Open();

			SqlCommand comEstado = new SqlCommand("select est.id as id, 'Estado: ' + est.nombre as nombre " +
				"from estados_convenios est " +
				"inner join etapas_definicion_convenio ec on est.id = ec.estado " +
				"where ec.baja is null and est.baja is null and ec.id = @etapadefinicion", connection);

			comEstado.Parameters.AddWithValue("@etapadefinicion", etapaDefinicionId);

			SqlDataReader drEstado = comEstado.ExecuteReader();

			while (drEstado.Read())
			{
				var estado = new IdValor();
				estado.Id = drEstado["id"].ToString();
				estado.Valor = drEstado["nombre"].ToString();
				etapa.Estado = estado;
			}

			connection.Close();
			connection.Open();

			SqlCommand comTarea = new SqlCommand("SELECT " +
				"t.id as idtarea, " +
				"t.alta as alta, " +
				"t.nombre as nombre, " +
				"'Responsable: ' + u.nombre + ' ' + u.apellido as responsable, " +
				"'Estado: ' + et.nombre as estado, " +
				"'Fecha de fin de tarea: ' + convert(varchar, t.fecha_fin, 103) as fecha_fin, " +
				"'Última modificación: Hace ' + convert(varchar, datediff(dd, t.fecha_modif, getdate())) + ' días' as dias_modif, " +
				"isnull((datediff(dd, t.fecha_modif, getdate())),0) as dias_modif_int, " +
				"case when exists " +
				"(select 1 from relacion_tareas rt " +
				"inner join tareas t2 on rt.tarea_pred = t2.id " +
				"where rt.tarea_suce = t.id and t2.estado in (1, 2, 3)) then 'Existen predecesoras sin terminar' else '' end as predec " +
				"FROM tareas t " +
				"inner join etapas_definicion_convenio ec on t.etapa_definicion_convenio = ec.id " +
				"left join usuarios u on u.id = t.responsable " +
				"inner join estados_tareas et on et.id = t.estado " +
				"where t.baja is null and u.baja is null and et.baja is null and ec.baja is null " +
				"and ec.id = @etapadefinicion", connection);

			comTarea.Parameters.AddWithValue("@etapadefinicion", etapaDefinicionId);

			SqlDataReader drTarea = comTarea.ExecuteReader();

			if (drTarea.HasRows)
			{
				var tareas = new List<Tarea>();
				while (drTarea.Read())
				{
					var tarea = new Tarea();
					tarea.Id = drTarea["idtarea"].ToString();
					tarea.Nombre = drTarea["nombre"].ToString();
					tarea.Responsable = drTarea["responsable"].ToString();
					tarea.Estado = drTarea["estado"].ToString();
					tarea.FechaFin = drTarea["fecha_fin"].ToString();
					tarea.DiaModif = drTarea["dias_modif"].ToString();
					tarea.Predecesoras = drTarea["predec"].ToString();
					tarea.Alta = drTarea["alta"].ToString();
					tarea.DiasModifInt = Convert.ToInt32(drTarea["dias_modif_int"]);

					tareas.Add(tarea);
				}
				etapa.Tareas = tareas.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand com2 = new SqlCommand("select e.id as empresa_id, e.nombre as empresa_nombre, c.id as convenio_id, c.nombre as convenio_nombre, edc.nombre as nombre_etapa, " +
				"c.archivo_convenio_string as archivo " +
				"from etapas_definicion_convenio edc inner join empresas e on e.id=edc.empresa " +
				"left join convenios c on c.etapa_definicion=edc.id " +
				"where c.baja is null and e.baja is null and edc.baja is null and edc.id = @id", connection);
			com2.Parameters.AddWithValue("@id", etapaDefinicionId);

			SqlDataReader dr2 = com2.ExecuteReader();
			if (dr2.HasRows)
			{
				while(dr2.Read())
				{
					etapa.Empresa = new IdValor { Id = dr2["empresa_id"].ToString(), Valor = dr2["empresa_nombre"].ToString() };
					etapa.Convenio = new IdValor { Id = dr2["convenio_id"].ToString(), Valor = dr2["convenio_nombre"].ToString() };
					etapa.Archivo = dr2["archivo"].ToString();
					etapa.NombreEtapa = dr2["nombre_etapa"].ToString();
				}
			}

			connection.Close();

			return etapa;
		}

		[HttpGet]
		[Route("get-data-seleccion-alumno")]
		public Etapa GetDataSeleccionAlumno([FromQuery] string etapaSeleccionId)
		{
			var etapa = new Etapa();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select u.id,'Administrador de Universidad: ' + u.nombre + ' ' + u.apellido as nombre " +
				"from usuarios u " +
				"inner join etapas_seleccion_alumnos ec on u.id = ec.admin_universidad " +
				"where ec.baja IS NULL and u.baja IS NULL and ec.id = @etapaId", connection);

			com.Parameters.AddWithValue("@etapaId", etapaSeleccionId);

			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var adminstradorUniversidad = new IdValor();
				adminstradorUniversidad.Id = dr["id"].ToString();
				adminstradorUniversidad.Valor = dr["nombre"].ToString();
				etapa.AdministradorUniversidad = adminstradorUniversidad;
			}

			connection.Close();
			connection.Open();

			SqlCommand comAdminEmp = new SqlCommand("select u.id as id, 'Administrador de Empresa: ' + u.nombre + ' ' + u.apellido  as nombre " +
				"from usuarios u " +
				"inner join etapas_seleccion_alumnos ec on u.id = ec.admin_empresa " +
				"where ec.baja IS NULL and u.baja IS NULL and ec.id = @etapaId", connection);

			comAdminEmp.Parameters.AddWithValue("@etapaId", etapaSeleccionId);

			SqlDataReader drAdminEmp = comAdminEmp.ExecuteReader();

			while (drAdminEmp.Read())
			{
				var administradorEmpresa = new IdValor();
				administradorEmpresa.Id = drAdminEmp["id"].ToString();
				administradorEmpresa.Valor = drAdminEmp["nombre"].ToString();
				etapa.AdministradorEmpresa = administradorEmpresa;
			}

			connection.Close();
			connection.Open();

			SqlCommand comEstado = new SqlCommand("select est.id as id, 'Estado: ' + est.nombre as nombre " +
				"from estados_seleccion est " +
				"inner join etapas_seleccion_alumnos ec on est.id = ec.estado " +
				"where ec.baja is null and est.baja is null and ec.id = @etapaId", connection);

			comEstado.Parameters.AddWithValue("@etapaId", etapaSeleccionId);

			SqlDataReader drEstado = comEstado.ExecuteReader();

			while (drEstado.Read())
			{
				var estado = new IdValor();
				estado.Id = drEstado["id"].ToString();
				estado.Valor = drEstado["nombre"].ToString();
				etapa.Estado = estado;
			}

			connection.Close();
			connection.Open();

			SqlCommand comTarea = new SqlCommand("SELECT " +
				"t.id as idtarea, " +
				"t.alta as alta, " +
				"t.nombre as nombre, " +
				"'Responsable: ' + u.nombre + ' ' + u.apellido as responsable, " +
				"'Estado: ' + et.nombre as estado, " +
				"'Fecha de fin de tarea: ' + convert(varchar, t.fecha_fin, 103) as fecha_fin, " +
				"'Última modificación: Hace ' + convert(varchar, datediff(dd, t.fecha_modif, getdate())) + ' días' as dias_modif, " +
				"isnull((datediff(dd, t.fecha_modif, getdate())),0) as dias_modif_int, " +
				"case when exists " +
				"(select 1 from relacion_tareas rt " +
				"inner join tareas t2 on rt.tarea_pred = t2.id " +
				"where rt.tarea_suce = t.id and t2.estado in (1, 2, 3)) then 'Existen predecesoras sin terminar' else '' end as predec " +
				"FROM tareas t " +
				"inner join etapas_seleccion_alumnos ec on t.etapa_seleccion_alumno = ec.id " +
				"left join usuarios u on u.id = t.responsable " +
				"inner join estados_tareas et on et.id = t.estado " +
				"where t.baja is null and u.baja is null and et.baja is null and ec.baja is null " +
				"and ec.id = @etapaId", connection);

			comTarea.Parameters.AddWithValue("@etapaId", etapaSeleccionId);

			SqlDataReader drTarea = comTarea.ExecuteReader();

			if (drTarea.HasRows)
			{
				var tareas = new List<Tarea>();
				while (drTarea.Read())
				{
					var tarea = new Tarea();
					tarea.Id = drTarea["idtarea"].ToString();
					tarea.Nombre = drTarea["nombre"].ToString();
					tarea.Responsable = drTarea["responsable"].ToString();
					tarea.Estado = drTarea["estado"].ToString();
					tarea.FechaFin = drTarea["fecha_fin"].ToString();
					tarea.DiaModif = drTarea["dias_modif"].ToString();
					tarea.Predecesoras = drTarea["predec"].ToString();
					tarea.Alta = drTarea["alta"].ToString();
					tarea.DiasModifInt = Convert.ToInt32(drTarea["dias_modif_int"]);

					tareas.Add(tarea);
				}
				etapa.Tareas = tareas.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand com2 = new SqlCommand("select e.id as empresa_id, " +
				"e.nombre as empresa_nombre,  " +
				"a.nombre + ' ' + a.apellido as alumno, " +
				"isnull(perf.id,0) as perfil, " +
				"isnull(pos.archivo,0) as archivo, " +
				"pro.id as id_propuesta, " +
				"pro.titulo as titulo_propuesta, " +
				"c.id as convenio_id, " +
				"c.nombre as convenio_nombre, " +
				"isnull(edc.id,0) as etapa_definicion_convenio, " +
				"sa.razon_rechazo as razon_rechazo, " +
				"isnull((select ia.id from etapas_ingreso_alumnos ia where ia.seleccion_alumno=sa.id),0) as etapa_ingreso " +
				"from etapas_seleccion_alumnos sa " +
				"left join etapas_definicion_convenio edc on sa.etapas_definicion_convenio = edc.id " +
				"left join convenios c on c.etapa_definicion=edc.id " +
				"inner join empresas e on e.id=sa.empresa " +
				"inner join usuarios u on sa.alumno=u.id " +
				"inner join alumnos a on a.id=u.alumno " +
				"inner join propuestas_caract_propias pro on pro.id=sa.propuesta " +
				"inner join postulaciones pos on pro.id=pos.propuesta and pos.alumno=a.id " +
				"left join perfil perf on perf.id=pos.perfil " +
				"where e.baja is null and sa.baja is null and u.baja is null and a.baja is null and pro.baja is null and pos.baja is null " +
				"and sa.id=@idetapa", connection);
			com2.Parameters.AddWithValue("@idetapa", etapaSeleccionId);

			SqlDataReader dr2 = com2.ExecuteReader();
			if (dr2.HasRows)
			{
				while (dr2.Read())
				{
					etapa.Empresa = new IdValor { Id = dr2["empresa_id"].ToString(), Valor = dr2["empresa_nombre"].ToString() };
					etapa.Convenio = new IdValor { Id = dr2["convenio_id"].ToString(), Valor = dr2["convenio_nombre"].ToString() };
					etapa.Archivo = dr2["archivo"].ToString();
					etapa.Alumno = dr2["alumno"].ToString();
					etapa.TituloPropuesta = dr2["titulo_propuesta"].ToString();
					etapa.EtapaDefinicionConvenio = dr2["etapa_definicion_convenio"].ToString();
					etapa.EtapaIngreso = dr2["etapa_ingreso"].ToString();
					etapa.RazonRechazo = dr2["razon_rechazo"].ToString();
				}
			}

			connection.Close();

			return etapa;
		}

		[HttpGet]
		[Route("iniciar-etapa")]
		public void IniciarEtapa([FromQuery] string etapaDefinicionId)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update etapas_definicion_convenio set estado = (select id from estados_convenios where nombre = 'En progreso') where id = @id", connection);
			com.Parameters.AddWithValue("@id", etapaDefinicionId);

			com.ExecuteReader();

			connection.Close();
		}

		[HttpGet]
		[Route("iniciar-seleccion")]
		public void IniciarSeleccion([FromQuery] string etapaId)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update etapas_seleccion_alumnos set estado = (select id from estados_seleccion where nombre = 'En progreso') where id = @id", connection);
			com.Parameters.AddWithValue("@id", etapaId);

			com.ExecuteReader();

			connection.Close();
		}

		[HttpGet]
		[Route("desestimar-etapa")]
		public void DesestimarEtapa([FromQuery] string etapaDefinicionId)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update etapas_definicion_convenio set estado = (select id from estados_convenios where nombre = 'Desestimado') where id = @id", connection);
			com.Parameters.AddWithValue("@id", etapaDefinicionId);

			com.ExecuteReader();

			connection.Close();
		}

		[HttpPost]
		[Route("desestimar-seleccion")]
		public void DesestimarSeleccion([FromBody] IdValor razonId)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update etapas_seleccion_alumnos set estado = (select id from estados_seleccion where nombre = 'Alumno Rechazado') where id = @id", connection);
			com.Parameters.AddWithValue("@id", razonId.Id);

			com.ExecuteReader();

			connection.Close();
			connection.Open();

			SqlCommand com2 = new SqlCommand("update etapas_seleccion_alumnos set razon_rechazo = @razon where id = @id", connection);
			com2.Parameters.AddWithValue("@id", razonId.Id);
			com2.Parameters.AddWithValue("@razon", razonId.Valor);

			com2.ExecuteReader();

			connection.Close();
		}

		[HttpGet]
		[Route("finalizar-etapa")]
		public void FinalizarEtapa([FromQuery] string etapaDefinicionId)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update etapas_definicion_convenio set estado = (select id from estados_convenios where nombre = 'Finalizada') where id = @id", connection);
			com.Parameters.AddWithValue("@id", etapaDefinicionId);

			com.ExecuteReader();

			connection.Close();
		}

		[HttpGet]
		[Route("finalizar-seleccion")]
		public void FinalizarSeleccion([FromQuery] string etapaId)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update etapas_seleccion_alumnos set estado = (select id from estados_seleccion where nombre = 'Finalizada') where id = @id", connection);
			com.Parameters.AddWithValue("@id", etapaId);

			com.ExecuteReader();

			connection.Close();
			connection.Open();
			SqlCommand comte = new SqlCommand("select etapas_definicion_convenio from etapas_seleccion_alumnos where id = @etapaId", connection);
			comte.Parameters.AddWithValue("@etapaId", etapaId);

			bool esPasantia = false;
			SqlDataReader drr = comte.ExecuteReader();
			if (drr.HasRows)
			{
				while (drr.Read())
				{
					if (!Convert.ToInt32(drr["etapas_definicion_convenio"].ToString()).Equals(0))
					{
						esPasantia = true;
					}
				}
			}

			connection.Close();
			if (esPasantia)
			{
				connection.Open();

				SqlCommand comAdminEmpresa = new SqlCommand("select top 1 u.id from usuarios u" +
					" inner join miembros_equipos me on me.usuario = u.id" +
					" inner join equipos e on me.equipo = e.id" +
					 " where me.baja is null" +
					 " and u.baja is null" +
					 " and e.empresa = (select empresa from etapas_seleccion_alumnos where id = @etapaId) " +
					 " and e.baja is null" +
					 " and e.nombre = 'Gestores de ingreso de alumno'" +
					 " and e.tipo_equipo = 'e'" +
					 " order by(select count(1) from etapas_ingreso_alumnos where admin_empresa = u.id) asc", connection);

				comAdminEmpresa.Parameters.AddWithValue("@etapaId", etapaId);

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
					 " and e.nombre = 'Gestores de ingreso de alumno'" +
					 " and e.tipo_equipo = 'u'" +
					 " order by(select count(1) from etapas_ingreso_alumnos where admin_universidad = u.id) asc", connection);

				SqlDataReader dr2 = comAdminUni.ExecuteReader();
				var adminUniId = string.Empty;
				while (dr2.Read())
				{
					adminUniId = dr2["id"].ToString();
				}
				connection.Close();
				connection.Open();
				SqlCommand com2 = new SqlCommand("insert into etapas_ingreso_alumnos " +
					"(seleccion_alumno, admin_empresa, admin_universidad, alumno, estado, alta, empresa) " +
					"values " +
					"(@etapaId, @admin_empresa, @admin_universidad, (select alumno from etapas_seleccion_alumnos where id = @etapaId), 1, @alta, (select empresa from etapas_seleccion_alumnos where id = @etapaId))", connection);
				com2.Parameters.AddWithValue("@etapaId", etapaId);
				com2.Parameters.AddWithValue("@admin_empresa", adminEmpresaId);
				com2.Parameters.AddWithValue("@admin_universidad", adminUniId);
				com2.Parameters.AddWithValue("@alta", sqlFormattedDate);

				com2.ExecuteReader();

				connection.Close();
			}
		}

		[HttpGet]
		[Route("reiniciar-seleccion")]
		public void ReiniciarSeleccion([FromQuery] string etapaId)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update etapas_seleccion_alumnos set estado = (select id from estados_seleccion where nombre = 'Lista para iniciar') where id = @id", connection);
			com.Parameters.AddWithValue("@id", etapaId);

			com.ExecuteReader();

			connection.Close();
		}
	}
}
