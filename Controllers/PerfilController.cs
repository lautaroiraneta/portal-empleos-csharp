using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PerfilController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public PerfilController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public void Post([FromBody] Perfil perfil)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("" +
				"insert into perfil (nombre, apellido, pais_residencia, provincia, fecha_nac, estado_civil, pais_nacionalidad, tipo_documento, documento, objetivo_laboral, intereses_personales, alta) output INSERTED.ID" +
				" values (@nombre, @apellido, @pais_residencia, @provincia, @fecha_nac, @estado_civil, @pais_nacionalidad, @tipo_documento, @documento, @objetivo_laboral, @intereses_personales, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(perfil.Nombre) ? DBNull.Value.ToString() : perfil.Nombre);
			com.Parameters.AddWithValue("@apellido", string.IsNullOrEmpty(perfil.Apellido) ? DBNull.Value.ToString() : perfil.Apellido);
			com.Parameters.AddWithValue("@pais_residencia", perfil.PaisResidencia?.Length > 0 ? perfil.PaisResidencia[0].Id : "1");
			com.Parameters.AddWithValue("@provincia", perfil.ProvinciaResidencia?.Length > 0 ? perfil.ProvinciaResidencia[0].Id : "1");
			com.Parameters.AddWithValue("@fecha_nac", perfil.FechaNacimientoDT);
			com.Parameters.AddWithValue("@estado_civil", perfil.EstadoCivil?.Length > 0 ? perfil.EstadoCivil[0].Valor : DBNull.Value.ToString());
			com.Parameters.AddWithValue("@pais_nacionalidad", perfil.PaisNacionalidad?.Length > 0 ? perfil.PaisNacionalidad[0].Id : "1");
			com.Parameters.AddWithValue("@tipo_documento", string.IsNullOrEmpty(perfil.TipoDocumento) ? DBNull.Value.ToString() : perfil.TipoDocumento);
			com.Parameters.AddWithValue("@documento", string.IsNullOrEmpty(perfil.Documento) ? DBNull.Value.ToString() : perfil.Documento);
			com.Parameters.AddWithValue("@objetivo_laboral", string.IsNullOrEmpty(perfil.ObjetivoLaboral) ? DBNull.Value.ToString() : perfil.ObjetivoLaboral);
			com.Parameters.AddWithValue("@intereses_personales", string.IsNullOrEmpty(perfil.InteresesPersonales) ? DBNull.Value.ToString() : perfil.InteresesPersonales);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var perfilId = (int)com.ExecuteScalar();

			connection.Close();
			connection.Open();

			if (perfil.RedesSociales != null)
			{
				myDateTime = DateTime.Now;
				sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
				
				//Facebook
				SqlCommand comFacebook = new SqlCommand("insert into redes_sociales (perfil, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@perfil, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comFacebook.Parameters.AddWithValue("@perfil", perfilId);
				comFacebook.Parameters.AddWithValue("@red_social", perfil.RedesSociales.usuarioFacebook);
				comFacebook.Parameters.AddWithValue("@tipo_red", "FB");
				comFacebook.Parameters.AddWithValue("@mostrar_feed", perfil.RedesSociales.mostrarFeedFacebook);
				comFacebook.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comFacebook.ExecuteReader();
				connection.Close();
				connection.Open();

				//Twitter
				SqlCommand comTwitter = new SqlCommand("insert into redes_sociales (perfil, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@perfil, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comTwitter.Parameters.AddWithValue("@perfil", perfilId);
				comTwitter.Parameters.AddWithValue("@red_social", perfil.RedesSociales.usuarioTwitter);
				comTwitter.Parameters.AddWithValue("@tipo_red", "TW");
				comTwitter.Parameters.AddWithValue("@mostrar_feed", perfil.RedesSociales.mostrarFeedTwitter);
				comTwitter.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comTwitter.ExecuteReader();
				connection.Close();
				connection.Open();

				//Instagram
				SqlCommand comInstagram = new SqlCommand("insert into redes_sociales (perfil, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@perfil, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comInstagram.Parameters.AddWithValue("@perfil", perfilId);
				comInstagram.Parameters.AddWithValue("@red_social", perfil.RedesSociales.usuarioInstagram);
				comInstagram.Parameters.AddWithValue("@tipo_red", "IG");
				comInstagram.Parameters.AddWithValue("@mostrar_feed", perfil.RedesSociales.mostrarFeedInstagram);
				comInstagram.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comInstagram.ExecuteReader();
				connection.Close();
				connection.Open();

				//LinkedIn
				SqlCommand comLinkedIn = new SqlCommand("insert into redes_sociales (perfil, red_social, tipo_red, mostrar_feed, alta) values" +
					" (@perfil, @red_social, @tipo_red, @mostrar_feed, @alta)", connection);

				comLinkedIn.Parameters.AddWithValue("@perfil", perfilId);
				comLinkedIn.Parameters.AddWithValue("@red_social", perfil.RedesSociales.usuarioLinkedIn);
				comLinkedIn.Parameters.AddWithValue("@tipo_red", "LI");
				comLinkedIn.Parameters.AddWithValue("@mostrar_feed", perfil.RedesSociales.mostrarFeedLinkedIn);
				comLinkedIn.Parameters.AddWithValue("@alta", sqlFormattedDate);

				comLinkedIn.ExecuteReader();
				connection.Close();
				connection.Open();
			}

			if (perfil.Emails.Length > 0)
			{
				for (int i = 0; i < perfil.Emails.Length; ++i)
				{
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comEmail = new SqlCommand("insert into correos_electronicos (perfil, correo, alta) values" +
						" (@perfil, @correo, @alta)", connection);

					comEmail.Parameters.AddWithValue("@perfil", perfilId);
					comEmail.Parameters.AddWithValue("@correo", perfil.Emails[i].Valor);
					comEmail.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comEmail.ExecuteReader();
					connection.Close();
					connection.Open();
				}
			}

			if (perfil.Telefonos.Length > 0)
			{
				for (int i = 0; i < perfil.Telefonos.Length; ++i)
				{
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comTelefono = new SqlCommand("insert into telefonos (perfil, numero_telefono, alta) values" +
						" (@perfil, @numero_telefono, @alta)", connection);

					comTelefono.Parameters.AddWithValue("@perfil", perfilId);
					comTelefono.Parameters.AddWithValue("@numero_telefono", perfil.Telefonos[i].Valor);
					comTelefono.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comTelefono.ExecuteReader();
					connection.Close();
					connection.Open();
				}
			}

			connection.Close();
		}
	}
}
