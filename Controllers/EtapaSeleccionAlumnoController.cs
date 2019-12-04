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
	}
}