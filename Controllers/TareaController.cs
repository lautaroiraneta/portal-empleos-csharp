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
	public class TareaController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public TareaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		[Route("iniciar-tarea")]
		public void IniciarTarea([FromQuery] string tareaId)
		{
			CambiarEstado(tareaId, 3);
		}

		[HttpGet]
		[Route("finalizar-tarea")]
		public void FinalizarTarea([FromQuery] string tareaId)
		{
			CambiarEstado(tareaId, 5);

			string connectionstring2 = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection2 = new SqlConnection(connectionstring2);
			connection2.Open();

			SqlCommand com2 = new SqlCommand("update tareas set estado=2 " +
				"where id in (select rt.tarea_suce from relacion_tareas rt where rt.tarea_pred = @id) " +
				"and estado = 1", connection2);

			com2.Parameters.AddWithValue("@id", tareaId);
			com2.ExecuteReader();

			connection2.Close();
		}

		[HttpGet]
		[Route("mostrar-conf")]
		public IdValor MostrarConf([FromQuery] string tareaId)
		{
			var str = new IdValor();

			string connectionstring2 = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection2 = new SqlConnection(connectionstring2);
			connection2.Open();

			SqlCommand com2 = new SqlCommand("select case when " +
				"exists(select 1 from tareas t2 where t2.id in " +
				"(select rt.tarea_pred from relacion_tareas rt where rt.tarea_suce = t.id) " +
				"and t2.estado in (1, 2, 3)) then 'mostrar_conf' else '' end as mostrar_conf " +
				"from tareas t where id = @id", connection2);

			com2.Parameters.AddWithValue("@id", tareaId);
			SqlDataReader dr2 = com2.ExecuteReader();

			if (dr2.HasRows)
			{
				while (dr2.Read())
				{
					str.Valor = dr2["mostrar_conf"].ToString();
				}
			}

			connection2.Close();

			return str;	
		}

		[HttpGet]
		[Route("cancelar-tarea")]
		public void CancelarTarea([FromQuery] string tareaId)
		{
			CambiarEstado(tareaId, 4);

			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring2 = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection2 = new SqlConnection(connectionstring2);
			connection2.Open();

			SqlCommand com2 = new SqlCommand("update tareas set estado = 2, fecha_modif = @fecha_modif " +
				"where id in (select rt.tarea_suce from relacion_tareas rt where rt.tarea_pred = @id) " +
				"and estado = 1", connection2);

			com2.Parameters.AddWithValue("@id", tareaId);
			com2.Parameters.AddWithValue("@fecha_modif", sqlFormattedDate);
			com2.ExecuteReader();

			connection2.Close();
		}

		[HttpPost]
		[Route("cambiar-responsable")]
		public void CambiarResponsable([FromBody] IdValor responsable, [FromQuery] string tipo)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring2 = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection2 = new SqlConnection(connectionstring2);
			connection2.Open();

			SqlCommand com2 = new SqlCommand("update tareas set responsable = @responsable_id, fecha_modif = @fecha_modif " +
				"where id = @id", connection2);

			com2.Parameters.AddWithValue("@responsable_id", responsable.Valor);
			com2.Parameters.AddWithValue("@id", responsable.Id);
			com2.Parameters.AddWithValue("@fecha_modif", sqlFormattedDate);
			com2.ExecuteReader();

			connection2.Close();
			connection2.Open();

			var aux = 0;
			if (tipo == "definicion")
			{
				aux = 1;
			}
			else 
			{
				aux = 2;
			}

			SqlCommand com3 = new SqlCommand("insert into participantes_etapa select @aux, @id, @responsable_id, @alta, null", connection2);
			com3.Parameters.AddWithValue("@aux", aux);
			com3.Parameters.AddWithValue("@responsable_id", responsable.Valor);
			com3.Parameters.AddWithValue("@id", responsable.Id);
			com3.Parameters.AddWithValue("@fecha_modif", sqlFormattedDate);
			com3.Parameters.AddWithValue("@alta", sqlFormattedDate);
			com3.ExecuteReader();

			connection2.Close();
		}



		private void CambiarEstado(string tareaId, int estado)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update tareas set estado = @estado, fecha_modif = @fecha_modif where id = @id", connection);
			com.Parameters.AddWithValue("@estado", estado);
			com.Parameters.AddWithValue("@fecha_modif", sqlFormattedDate);
			com.Parameters.AddWithValue("@id", tareaId);
			com.ExecuteReader();

			connection.Close();
		}
	}
}
