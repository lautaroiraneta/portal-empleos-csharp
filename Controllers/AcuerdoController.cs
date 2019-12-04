using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PortalEmpleos.Models;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AcuerdoController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public AcuerdoController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public void Post([FromBody] Acuerdo acuerdo)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into acuerdos_individuales (ingreso_alumnos, alumno, empresa, nombre, docente_guia, tutor, duracion, alta) output INSERTED.ID " +
				"values (@ingreso_alumnos, @alumno, @empresa, @nombre, @docente_guia, @tutor, @duracion, @alta)", connection);

			com.Parameters.AddWithValue("@ingreso_alumnos", DBNull.Value);
			com.Parameters.AddWithValue("@alumno", DBNull.Value);
			com.Parameters.AddWithValue("@empresa", DBNull.Value);
			com.Parameters.AddWithValue("@nombre", acuerdo.Nombre);
			com.Parameters.AddWithValue("@docente_guia", acuerdo.DocenteGuia[0].Id);
			com.Parameters.AddWithValue("@tutor", acuerdo.Tutor[0].Id);
			com.Parameters.AddWithValue("@duracion", acuerdo.Duracion);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var id = (int)com.ExecuteScalar();

			connection.Close();
			connection.Open();

			if (acuerdo.Tareas != null && acuerdo.Tareas.Length > 0)
			{
				for (int i = 0; i < acuerdo.Tareas.Length; ++i)
				{
					var tarea = acuerdo.Tareas[i];
					SqlCommand comTarea = new SqlCommand("insert into tareas_x_acuerdo (acuerdo_individual, tarea, alta) values (@acuerdo_individual, @tarea, @alta)", connection);
					comTarea.Parameters.AddWithValue("@acuerdo_individual", id);
					comTarea.Parameters.AddWithValue("@tarea", tarea.Valor);
					comTarea.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comTarea.ExecuteReader();

					connection.Close();
					connection.Open();
				}
			}

			connection.Close();
		}
	}
}