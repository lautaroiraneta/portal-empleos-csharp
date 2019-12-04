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

			if (acuerdo.Calendario != null)
			{
				SqlCommand comCalendario = new SqlCommand("insert into calendario_acuerdo (acuerdo_individual, horario_ing_lunes, horario_ing_martes, horario_ing_miercoles, horario_ing_jueves, horario_ing_viernes, horario_ing_sabado, horario_ing_domingo, horario_sal_lunes, horario_sal_martes, horario_sal_miercoles, horario_sal_jueves, horario_sal_viernes, horario_sal_sabado, horario_sal_domingo, alta) " +
					"values (@acuerdo_individual, @horario_ing_lunes, @horario_ing_martes, @horario_ing_miercoles, @horario_ing_jueves, @horario_ing_viernes, @horario_ing_sabado, @horario_ing_domingo, @horario_sal_lunes, @horario_sal_martes, @horario_sal_miercoles, @horario_sal_jueves, @horario_sal_viernes, @horario_sal_sabado, @horario_sal_domingo, @alta)", connection);

				comCalendario.Parameters.AddWithValue("@acuerdo_individual", id);
				comCalendario.Parameters.AddWithValue("@horario_ing_lunes", acuerdo.Calendario.HorarioIngLunes);
				comCalendario.Parameters.AddWithValue("@horario_ing_martes", acuerdo.Calendario.HorarioIngMartes);
				comCalendario.Parameters.AddWithValue("@horario_ing_miercoles", acuerdo.Calendario.HorarioIngMiercoles);
				comCalendario.Parameters.AddWithValue("@horario_ing_jueves", acuerdo.Calendario.HorarioIngJueves);
				comCalendario.Parameters.AddWithValue("@horario_ing_viernes", acuerdo.Calendario.HorarioIngViernes);
				comCalendario.Parameters.AddWithValue("@horario_ing_sabado", acuerdo.Calendario.HorarioIngSabado);
				comCalendario.Parameters.AddWithValue("@horario_ing_domingo", acuerdo.Calendario.HorarioIngDomingo);
				comCalendario.Parameters.AddWithValue("@horario_sal_lunes", acuerdo.Calendario.HorarioSalLunes);
				comCalendario.Parameters.AddWithValue("@horario_sal_martes", acuerdo.Calendario.HorarioSalMartes);
				comCalendario.Parameters.AddWithValue("@horario_sal_miercoles", acuerdo.Calendario.HorarioSalMiercoles);
				comCalendario.Parameters.AddWithValue("@horario_sal_jueves", acuerdo.Calendario.HorarioSalJueves);
				comCalendario.Parameters.AddWithValue("@horario_sal_viernes", acuerdo.Calendario.HorarioSalViernes);
				comCalendario.Parameters.AddWithValue("@horario_sal_sabado", acuerdo.Calendario.HorarioSalSabado);
				comCalendario.Parameters.AddWithValue("@horario_sal_domingo", acuerdo.Calendario.HorarioSalDomingo);
				comCalendario.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comCalendario.ExecuteReader();
			}

			connection.Close();
		}
	}
}