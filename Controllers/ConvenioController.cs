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
	public class ConvenioController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<ConvenioController> _logger;

		public ConvenioController(IConfiguration configuration, ILogger<ConvenioController> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPost]
		public void Post([FromBody] Convenio convenio)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into convenios (nombre, facultad, duracion_min, duracion_max, plazo_renov, cant_max_horas, inicio_vigencia, fin_vigencia, renov_automatica, cant_max_pasantes, alta, plazo_extension, etapa_definicion) " +
				"values (@nombre, @facultad, @duracion_min, @duracion_max, @plazo_renov, @cant_max_horas, @inicio_vigencia, @fin_vigencia, @renov_automatica, @cant_max_pasantes, @alta, @plazo_extension, @etapa_definicion)", connection);

			com.Parameters.AddWithValue("@nombre", convenio.Nombre);
			com.Parameters.AddWithValue("@facultad", convenio.Facultad[0].Id);
			com.Parameters.AddWithValue("@duracion_min", convenio.DuracionMinima);
			com.Parameters.AddWithValue("@duracion_max", convenio.DuracionMaxima);
			com.Parameters.AddWithValue("@plazo_renov", convenio.PlazoRenovacion);
			com.Parameters.AddWithValue("@cant_max_horas", convenio.CantidadMaxHoras);
			com.Parameters.AddWithValue("@inicio_vigencia", convenio.InicioVigenciaDT);
			com.Parameters.AddWithValue("@fin_vigencia", convenio.FinVigenciaDT);
			com.Parameters.AddWithValue("@renov_automatica", convenio.RenovacionAutomatica);
			com.Parameters.AddWithValue("@cant_max_pasantes", convenio.CantMaxPasantes);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);
			com.Parameters.AddWithValue("@plazo_extension", convenio.PlazoExtension);
			com.Parameters.AddWithValue("@etapa_definicion", convenio.EtapaId);
			
			com.ExecuteReader();

			connection.Close();
		}

		[HttpGet]
		public List<ConvenioView> GetList()
		{
			var convenios = new List<ConvenioView>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("SELECT " +
				"dc.id as id, " +
				"e.id as empresa_id, " +
				"e.nombre as empresa_nombre, " +
				"dc.nombre as nombre, " +
				"convert(varchar, dc.alta, 103) as fec_ini, " +
				"ec.nombre as estado " +
				"from " +
				"etapas_definicion_convenio dc " +
				"inner join estados_convenios ec on ec.id = dc.estado " +
				"inner join empresas e on e.id = dc.empresa " +
				"where dc.baja is null and ec.baja is null and e.baja is null", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var convenio = new ConvenioView();
				convenio.Id = dr["id"].ToString();
				convenio.Empresa = new IdValor { Id = dr["empresa_id"].ToString(), Valor = dr["empresa_nombre"].ToString() };
				convenio.Estado = dr["estado"].ToString();
				convenio.FecIni = dr["fec_ini"].ToString();
				convenio.NombreEtapa = dr["nombre"].ToString();

				convenios.Add(convenio);
			}

			connection.Close();

			return convenios;
		}
	}
}