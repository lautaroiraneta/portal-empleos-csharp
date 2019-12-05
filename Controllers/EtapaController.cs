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
				"datediff(dd, t.fecha_modif, getdate()) as dias_modif_int, " +
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

			return etapa;
		}
	}
}
