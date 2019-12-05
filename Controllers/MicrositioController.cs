using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MicrositioController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public MicrositioController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public void Post([FromBody] Micrositio micrositio)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("insert into micrositios_empresa (empresa, descripcion_empresa, sitio_web, alta) output INSERTED.ID values (@empresa, @descripcion_empresa, @sitio_web, @alta)", connection);

			com.Parameters.AddWithValue("@empresa", micrositio.EmpresaId);
			com.Parameters.AddWithValue("@descripcion_empresa", string.IsNullOrEmpty(micrositio.Descripcion) ? DBNull.Value.ToString() : micrositio.Descripcion);
			com.Parameters.AddWithValue("@sitio_web", string.IsNullOrEmpty(micrositio.SitioWeb) ? DBNull.Value.ToString() : micrositio.SitioWeb);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var id = (int)com.ExecuteScalar();

			if (micrositio.RedesSociales != null)
			{
				myDateTime = DateTime.Now;
				sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

				//Facebook
				SqlCommand comFacebook = new SqlCommand("insert into redes_sociales_empresa (micrositio, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@micrositio, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comFacebook.Parameters.AddWithValue("@micrositio", id);
				comFacebook.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(micrositio.RedesSociales.usuarioFacebook) ? DBNull.Value.ToString() : micrositio.RedesSociales.usuarioFacebook);
				comFacebook.Parameters.AddWithValue("@tipo_red", "FB");
				comFacebook.Parameters.AddWithValue("@mostrar_feed", micrositio.RedesSociales.mostrarFeedFacebook);
				comFacebook.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comFacebook.ExecuteReader();
				connection.Close();
				connection.Open();

				//Twitter
				SqlCommand comTwitter = new SqlCommand("insert into redes_sociales_empresa (micrositio, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@micrositio, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comTwitter.Parameters.AddWithValue("@micrositio", id);
				comTwitter.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(micrositio.RedesSociales.usuarioTwitter) ? DBNull.Value.ToString() : micrositio.RedesSociales.usuarioTwitter);
				comTwitter.Parameters.AddWithValue("@tipo_red", "TW");
				comTwitter.Parameters.AddWithValue("@mostrar_feed", micrositio.RedesSociales.mostrarFeedTwitter);
				comTwitter.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comTwitter.ExecuteReader();
				connection.Close();
				connection.Open();

				//Instagram
				SqlCommand comInstagram = new SqlCommand("insert into redes_sociales_empresa (micrositio, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@micrositio, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comInstagram.Parameters.AddWithValue("@micrositio", id);
				comInstagram.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(micrositio.RedesSociales.usuarioInstagram) ? DBNull.Value.ToString() : micrositio.RedesSociales.usuarioInstagram);
				comInstagram.Parameters.AddWithValue("@tipo_red", "IG");
				comInstagram.Parameters.AddWithValue("@mostrar_feed", micrositio.RedesSociales.mostrarFeedInstagram);
				comInstagram.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comInstagram.ExecuteReader();
				connection.Close();
				connection.Open();

				//LinkedIn
				SqlCommand comLinkedIn = new SqlCommand("insert into redes_sociales_empresa (micrositio, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@micrositio, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comLinkedIn.Parameters.AddWithValue("@micrositio", id);
				comLinkedIn.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(micrositio.RedesSociales.usuarioLinkedIn) ? DBNull.Value.ToString() : micrositio.RedesSociales.usuarioLinkedIn);
				comLinkedIn.Parameters.AddWithValue("@tipo_red", "LI");
				comLinkedIn.Parameters.AddWithValue("@mostrar_feed", micrositio.RedesSociales.mostrarFeedLinkedIn);
				comLinkedIn.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comLinkedIn.ExecuteReader();
				connection.Close();
				connection.Open();
			}

			connection.Close();
		}

		[HttpGet]
		public Micrositio GetById([FromQuery] string empresaId)
		{
			var micrositio = new Micrositio();
			micrositio.EmpresaId = empresaId;

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, descripcion_empresa, sitio_web from micrositios_empresa where empresa = @empresa", connection);
			com.Parameters.AddWithValue("@empresa", empresaId);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				micrositio.Id = dr["id"].ToString();
				micrositio.Descripcion = dr["descripcion_empresa"].ToString();
				micrositio.SitioWeb = dr["sitio_web"].ToString();
			}

			connection.Close();
			connection.Open();

			SqlCommand comRedesSociales = new SqlCommand("select id, red_social, tipo_red, mostrar_feed from redes_sociales_empresa where micrositio = @micrositio", connection);
			comRedesSociales.Parameters.AddWithValue("@micrositio", micrositio.Id);
			SqlDataReader drRedesSociales = comRedesSociales.ExecuteReader();

			if (drRedesSociales.HasRows)
			{
				micrositio.RedesSociales = new RedesSociales();
				while (drRedesSociales.Read())
				{
					switch (drRedesSociales["tipo_red"].ToString())
					{
						case "FB":
							micrositio.RedesSociales.usuarioFacebook = drRedesSociales["red_social"].ToString();
							micrositio.RedesSociales.mostrarFeedFacebook = (bool)drRedesSociales["mostrar_feed"];
							break;
						case "TW":
							micrositio.RedesSociales.usuarioTwitter = drRedesSociales["red_social"].ToString();
							micrositio.RedesSociales.mostrarFeedTwitter = (bool)drRedesSociales["mostrar_feed"];
							break;
						case "IG":
							micrositio.RedesSociales.usuarioInstagram = drRedesSociales["red_social"].ToString();
							micrositio.RedesSociales.mostrarFeedInstagram = (bool)drRedesSociales["mostrar_feed"];
							break;
						case "LI":
							micrositio.RedesSociales.usuarioLinkedIn = drRedesSociales["red_social"].ToString();
							micrositio.RedesSociales.mostrarFeedLinkedIn = (bool)drRedesSociales["mostrar_feed"];
							break;
					}
				}
			}

			connection.Close();

			return micrositio;
		}
	}
}
