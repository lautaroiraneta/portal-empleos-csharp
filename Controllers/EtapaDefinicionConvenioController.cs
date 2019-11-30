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
	public class EtapaDefinicionConvenioController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public EtapaDefinicionConvenioController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpPost]
		public void Post([FromBody] EtapaDefinicionConvenio etapa)
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
				 " and e.nombre = 'Administradores de Etapas de Definición de Convenio'" +
				 " and e.tipo_equipo = 'e'" +
				 " order by(select count(1) from etapas_definicion_convenio where admin_empresa = u.id) asc", connection);

			comAdminEmpresa.Parameters.AddWithValue("@empresa", etapa.Empresa[0].Id);

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
				 " and e.nombre = 'Administradores de Etapas de Definición de Convenio'" +
				 " and e.tipo_equipo = 'u'" +
				 " order by(select count(1) from etapas_definicion_convenio where admin_universidad = u.id) asc", connection);

			SqlDataReader dr2 = comAdminUni.ExecuteReader();
			var adminUniId = string.Empty;
			while (dr2.Read())
			{
				adminUniId = dr2["id"].ToString();
			}

			connection.Close();
			connection.Open();

			SqlCommand com = new SqlCommand("" +
				"insert into etapas_definicion_convenio (nombre, empresa, admin_empresa, admin_universidad, estado, alta) output INSERTED.ID" +
				" values (@nombre, @empresa, @admin_empresa, @admin_universidad, @estado, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(etapa.Nombre) ? DBNull.Value.ToString() : etapa.Nombre);
			com.Parameters.AddWithValue("@empresa", string.IsNullOrEmpty(etapa.Empresa[0].Id) ? DBNull.Value.ToString() : etapa.Empresa[0].Id);
			com.Parameters.AddWithValue("@admin_empresa", adminEmpresaId);
			com.Parameters.AddWithValue("@admin_universidad", adminUniId);
			com.Parameters.AddWithValue("@estado", "1");
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var etapaId = (int)com.ExecuteScalar();

			connection.Close();
			connection.Open();

			SqlCommand com2 = new SqlCommand("insert into tareas " +
				"(nombre, descripcion, estado, creador, responsable, etapa_definicion_convenio, fecha_fin, alta, baja, etapa_seleccion_alumno, etapa_ingreso_alumno, etapa_pasantia_en_curso, etapa_cierre, tarea_defecto) " +
				"select nombre, descripcion, 1, " +
				"CASE WHEN tipo_responsable='e' then(select admin_empresa from etapas_definicion_convenio where id = @id)" +
				"else (select admin_universidad from etapas_definicion_convenio where id = @id)" +
				"end," +
			   "CASE WHEN tipo_responsable = 'e' then(select admin_empresa from etapas_definicion_convenio where id = @id)" +
			   "else (select admin_universidad from etapas_definicion_convenio where id = @id)" +
			   "end," +
			   "@id," +
			   "getdate() + 30," +
			   "getdate()," +
			   "null, " +
			   "null, " +
			   "null, " +
			   "null, " +
			   "null, " +
			   "id " +
			   "from tareas_x_defecto " +
			   "where etapa = 1 and baja is null", connection);

			com2.Parameters.AddWithValue("@id", etapaId);

			com2.ExecuteReader();
			connection.Close();
			connection.Open();

			SqlCommand com3 = new SqlCommand("insert into relacion_tareas (tarea_pred, tarea_suce, alta, baja)" +
				" select t1.id, t2.id, getdate(), null from relacion_tareas_x_defecto rtd" +
				" inner join tareas_x_defecto td1 on rtd.tarea_pred = td1.id" +
				" inner join tareas_x_defecto td2 on rtd.tarea_suce = td2.id" +
				" inner join tareas t1 on t1.tarea_defecto = td1.id" +
				" inner join tareas t2 on t2.tarea_defecto = td2.id" +
				" where t1.etapa_definicion_convenio = @id and t2.etapa_definicion_convenio = @id" +
				" and td2.baja is null and td1.baja is null and t1.baja is null and t2.baja is null and rtd.baja is null", connection);

			com3.Parameters.AddWithValue("@id", etapaId);
			com3.ExecuteReader();

			connection.Close();
		}
	}
}