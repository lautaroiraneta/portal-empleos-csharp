using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TareaViewController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly IHostingEnvironment _hostingEnvironment;

		public TareaViewController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
		{
			_configuration = configuration;
			_hostingEnvironment = hostingEnvironment;
		}


		[HttpGet]
		public TareaView Get([FromQuery] string tareaId)
		{
			var tarea = new TareaView();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select t.id as id_tarea, t.nombre as nombre, t.descripcion as descripcion, e.nombre as estado, " +
				"u.nombre + ' ' + u.apellido as responsable, convert(varchar, t.fecha_fin, 103) as fecha_fin from tareas t " +
				"inner join estados_tareas e on e.id = t.estado " +
				"inner join usuarios u on u.id = t.responsable " +
				"where t.id = @id and e.baja is null and u.baja is null", connection);
			com.Parameters.AddWithValue("@id", tareaId);

			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				tarea.Id = dr["id_tarea"].ToString();
				tarea.Nombre = dr["nombre"].ToString();
				tarea.Descripcion = dr["descripcion"].ToString();
				tarea.Estado = dr["estado"].ToString();
				tarea.Responsable = dr["responsable"].ToString();
				tarea.FechaFin = dr["fecha_fin"].ToString();
			}

			connection.Close();
			connection.Open();

			SqlCommand com2 = new SqlCommand("select c.id as id_comentario, c.comentario as comentario, u.nombre + ' ' + u.apellido as usuario, " +
				"convert(varchar, c.fecha, 103) as fecha " +
				"from comentarios_x_tarea c " +
				"inner join usuarios u on u.id = c.usuario " +
				"where tarea = @id and u.baja is null and c.baja is null " +
				"order by c.alta desc", connection);

			com2.Parameters.AddWithValue("@id", tareaId);

			SqlDataReader dr2 = com2.ExecuteReader();

			if (dr2.HasRows)
			{
				var comentarios = new List<Comentario>();
				while (dr2.Read())
				{
					var comentario = new Comentario();
					comentario.Contenido = dr2["comentario"].ToString();
					comentario.Fecha = dr2["fecha"].ToString();
					comentario.Id = dr2["id_comentario"].ToString();
					comentario.Usuario = dr2["usuario"].ToString();

					comentarios.Add(comentario);
				}

				tarea.Comentarios = comentarios.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand com3 = new SqlCommand("select a.id as id_archivo, a.archivo_ruta as archivo, u.nombre + ' ' + u.apellido as usuario, " +
				"convert(varchar, a.fecha, 103) as fecha " +
				"from archivos_x_tarea a " +
				"inner join usuarios u on u.id = a.usuario " +
				"where tarea = @id and u.baja is null and a.baja is null " +
				"order by a.alta desc", connection);

			com3.Parameters.AddWithValue("@id", tareaId);

			SqlDataReader dr3 = com3.ExecuteReader();

			if (dr3.HasRows)
			{
				var archivos = new List<TareaViewArchivo>();
				while (dr3.Read())
				{
					var archivo = new TareaViewArchivo();
					archivo.Archivo = dr3["archivo"].ToString();
					archivo.Id = dr3["id_archivo"].ToString();
					archivo.Usuario = dr3["usuario"].ToString();

					archivos.Add(archivo);
				}

				tarea.Archivos = archivos.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand com4 = new SqlCommand("select t2.id as tarea_que_hab_id, t2.nombre + ' (' + e.nombre + ')' as tarea_que_hab_nombre " +
				"from relacion_tareas rt " +
				"inner join tareas t on t.id = rt.tarea_pred " +
				"inner join tareas t2 on t2.id = rt.tarea_suce " +
				"inner join estados_tareas e on e.id = t2.estado " +
				"where t.id = @id and rt.baja is null " +
				"and t2.baja is null", connection);
			com4.Parameters.AddWithValue("@id", tareaId);

			SqlDataReader dr4 = com4.ExecuteReader();

			if (dr4.HasRows)
			{
				var tareas = new List<IdValor>();
				while (dr4.Read())
				{
					var tareaQH = new IdValor();
					tareaQH.Id = dr4["tarea_que_hab_id"].ToString();
					tareaQH.Valor = dr4["tarea_que_hab_nombre"].ToString();

					tareas.Add(tareaQH);
				}
				tarea.TareasQueHabilita = tareas.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand com5 = new SqlCommand("select t2.id as tarea_pred_id, t2.nombre + ' (' + e.nombre + ')' as tarea_pred_nombre " +
				"from relacion_tareas rt " +
				"inner join tareas t on t.id = rt.tarea_suce " +
				"inner join tareas t2 on t2.id = rt.tarea_pred " +
				"inner join estados_tareas e on e.id = t2.estado " +
				"where t.id = @id and rt.baja is null " +
				"and t2.baja is null", connection);
			com5.Parameters.AddWithValue("@id", tareaId);

			SqlDataReader dr5 = com5.ExecuteReader();

			if (dr5.HasRows)
			{
				var tareas = new List<IdValor>();
				while (dr5.Read())
				{
					var tareaP = new IdValor();
					tareaP.Id = dr5["tarea_pred_id"].ToString();
					tareaP.Valor = dr5["tarea_pred_nombre"].ToString();

					tareas.Add(tareaP);
				}
				tarea.TareasPredecesoras = tareas.ToArray();
			}

			connection.Close();

			return tarea;
		}

		[HttpPost, DisableRequestSizeLimit]
		[Route("upload")]
		public void UploadFile([FromQuery] string usuarioId, [FromQuery] string tareaId)
		{

			var file = Request.Form.Files[0];
			string folderName = "Upload";
			string webRootPath = _hostingEnvironment.ContentRootPath;
			string newPath = Path.Combine(webRootPath, folderName);
			if (!Directory.Exists(newPath))
			{
				Directory.CreateDirectory(newPath);
			}
			if (file.Length > 0)
			{
				string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
				string fullPath = Path.Combine(newPath, fileName);
				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					file.CopyTo(stream);
				}

				string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
				SqlConnection connection = new SqlConnection(connectionstring);
				string sqlFormattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				connection.Open();

				SqlCommand com = new SqlCommand("insert into archivos_x_tarea (usuario, tarea, alta, archivo_ruta) " +
					"values (@usuario, @tarea, @alta, @archivo_ruta)", connection);
				com.Parameters.AddWithValue("@usuario", usuarioId);
				com.Parameters.AddWithValue("@tarea", tareaId);
				com.Parameters.AddWithValue("@alta", sqlFormattedDate);
				com.Parameters.AddWithValue("@archivo_ruta", fileName);

				com.ExecuteReader();

				connection.Close();
			}
		}
	}
}
