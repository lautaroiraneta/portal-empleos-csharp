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
				"insert into perfil (nombre, apellido, pais_residencia, provincia, zona, ciudad, localidad, fecha_nac, estado_civil, pais_nacionalidad, tipo_documento, documento, objetivo_laboral, intereses_personales, alumno, carrera, porcentaje_mat_apr, cantidad_mat_apr, promedio, anio_cursada, alta) output INSERTED.ID" +
				" values (@nombre, @apellido, @pais_residencia, @provincia, @zona, @ciudad, @localidad, @fecha_nac, @estado_civil, @pais_nacionalidad, @tipo_documento, @documento, @objetivo_laboral, @intereses_personales, @alumno, @carrera, @porcentaje_mat_apr, @cantidad_mat_apr, @promedio, @anio_cursada, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(perfil.Nombre) ? DBNull.Value.ToString() : perfil.Nombre);
			com.Parameters.AddWithValue("@apellido", string.IsNullOrEmpty(perfil.Apellido) ? DBNull.Value.ToString() : perfil.Apellido);
			com.Parameters.AddWithValue("@pais_residencia", perfil.PaisResidencia?.Length > 0 ? perfil.PaisResidencia[0].Id : "1");
			com.Parameters.AddWithValue("@provincia", perfil.ProvinciaResidencia?.Length > 0 ? perfil.ProvinciaResidencia[0].Id : "1");
			com.Parameters.AddWithValue("@zona", perfil.Zona?.Length > 0 ? perfil.Zona[0].Id : "1");
			com.Parameters.AddWithValue("@ciudad", perfil.Ciudad?.Length > 0 ? perfil.Ciudad[0].Id : "1");
			com.Parameters.AddWithValue("@localidad", perfil.Localidad?.Length > 0 ? perfil.Localidad[0].Id : "1");
			com.Parameters.AddWithValue("@fecha_nac", perfil.FechaNacimientoDT);
			com.Parameters.AddWithValue("@estado_civil", perfil.EstadoCivil?.Length > 0 ? perfil.EstadoCivil[0].Valor : DBNull.Value.ToString());
			com.Parameters.AddWithValue("@pais_nacionalidad", perfil.PaisNacionalidad?.Length > 0 ? perfil.PaisNacionalidad[0].Id : "1");
			com.Parameters.AddWithValue("@tipo_documento", string.IsNullOrEmpty(perfil.TipoDocumento) ? DBNull.Value.ToString() : perfil.TipoDocumento);
			com.Parameters.AddWithValue("@documento", string.IsNullOrEmpty(perfil.Documento) ? DBNull.Value.ToString() : perfil.Documento);
			com.Parameters.AddWithValue("@objetivo_laboral", string.IsNullOrEmpty(perfil.ObjetivoLaboral) ? DBNull.Value.ToString() : perfil.ObjetivoLaboral);
			com.Parameters.AddWithValue("@intereses_personales", string.IsNullOrEmpty(perfil.InteresesPersonales) ? DBNull.Value.ToString() : perfil.InteresesPersonales);
			com.Parameters.AddWithValue("@alumno", string.IsNullOrEmpty(perfil.Alumno) ? DBNull.Value.ToString() : perfil.Alumno);
			com.Parameters.AddWithValue("@carrera", perfil.Carrera?.Length > 0 ? perfil.Carrera[0].Id : DBNull.Value.ToString());
			com.Parameters.AddWithValue("@porcentaje_mat_apr", perfil.PorcentajeMateriasAprobadas);
			com.Parameters.AddWithValue("@cantidad_mat_apr", perfil.CantidadMateriasAprobadas);
			com.Parameters.AddWithValue("@promedio", perfil.Promedio);
			com.Parameters.AddWithValue("@anio_cursada", perfil.AnioCursada);
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
				comFacebook.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(perfil.RedesSociales.usuarioFacebook) ? DBNull.Value.ToString() : perfil.RedesSociales.usuarioFacebook);
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
				comTwitter.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(perfil.RedesSociales.usuarioTwitter) ? DBNull.Value.ToString() : perfil.RedesSociales.usuarioTwitter);
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
				comInstagram.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(perfil.RedesSociales.usuarioInstagram) ? DBNull.Value.ToString() : perfil.RedesSociales.usuarioInstagram);
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
				comLinkedIn.Parameters.AddWithValue("@red_social", string.IsNullOrEmpty(perfil.RedesSociales.usuarioLinkedIn) ? DBNull.Value.ToString() : perfil.RedesSociales.usuarioLinkedIn);
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

			if (perfil.ExperienciaLaboral.Length > 0)
			{
				for (int i = 0; i < perfil.ExperienciaLaboral.Length; ++i)
				{
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comExperienciaLaboral = new SqlCommand("insert into experiencias_laborales (perfil, empresa, puesto, fecha_desde, fecha_hasta, actualmente_trabajando, comentarios, alta) output INSERTED.ID values" +
						" (@perfil, @empresa, @puesto, @fecha_desde, @fecha_hasta, @actualmente_trabajando, @comentarios, @alta)", connection);

					comExperienciaLaboral.Parameters.AddWithValue("@perfil", perfilId);
					comExperienciaLaboral.Parameters.AddWithValue("@empresa", perfil.ExperienciaLaboral[i].Empresa);
					comExperienciaLaboral.Parameters.AddWithValue("@puesto", perfil.ExperienciaLaboral[i].PuestoLaboral[0].Id);
					comExperienciaLaboral.Parameters.AddWithValue("@fecha_desde", perfil.ExperienciaLaboral[i].FechaDesdeDT);
					comExperienciaLaboral.Parameters.AddWithValue("@fecha_hasta", perfil.ExperienciaLaboral[i].FechaHastaDT);
					comExperienciaLaboral.Parameters.AddWithValue("@actualmente_trabajando", perfil.ExperienciaLaboral[i].ActualmenteTrabajando ? 1 : 0);
					comExperienciaLaboral.Parameters.AddWithValue("@comentarios", perfil.ExperienciaLaboral[i].Descripcion);
					comExperienciaLaboral.Parameters.AddWithValue("@alta", sqlFormattedDate);

					var experienciaLaboralId = (int)comExperienciaLaboral.ExecuteScalar();
					connection.Close();
					connection.Open();

					// Conocimientos
					if (perfil.ExperienciaLaboral[i].Conocimientos?.Length > 0)
					{
						for (int j = 0; j < perfil.ExperienciaLaboral[i].Conocimientos.Length; ++j)
						{
							myDateTime = DateTime.Now;
							sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

							SqlCommand comConocimientos = new SqlCommand("insert into conocimientos_x_experiencia_laboral (conocimiento, experiencia_laboral, alta) values" +
								" (@conocimiento, @experiencia_laboral, @alta)", connection);

							comConocimientos.Parameters.AddWithValue("@conocimiento", perfil.ExperienciaLaboral[i].Conocimientos[j].Id);
							comConocimientos.Parameters.AddWithValue("@experiencia_laboral", experienciaLaboralId);
							comConocimientos.Parameters.AddWithValue("@alta", sqlFormattedDate);

							comConocimientos.ExecuteReader();
							connection.Close();
							connection.Open();
						}						
					}
				}
			}

			if (perfil.ExperienciaEducativa.Length > 0)
			{
				for (int i = 0; i < perfil.ExperienciaEducativa.Length; ++i)
				{
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comExperienciaEducativa = new SqlCommand("insert into experiencias_educativas (perfil, institucion, titulo, tipo_estudio, estado, fecha_desde, fecha_hasta, actualmente_estudiando, comentarios, alta) output INSERTED.ID values" +
						" (@perfil, @institucion, @titulo, @tipo_estudio, @estado, @fecha_desde, @fecha_hasta, @actualmente_estudiando, @comentarios, @alta)", connection);

					comExperienciaEducativa.Parameters.AddWithValue("@perfil", perfilId);
					comExperienciaEducativa.Parameters.AddWithValue("@institucion", string.IsNullOrEmpty(perfil.ExperienciaEducativa[i].Institucion) ? DBNull.Value.ToString() : perfil.ExperienciaEducativa[i].Institucion);
					comExperienciaEducativa.Parameters.AddWithValue("@titulo", string.IsNullOrEmpty(perfil.ExperienciaEducativa[i].Titulo) ? DBNull.Value.ToString() : perfil.ExperienciaEducativa[i].Titulo);
					comExperienciaEducativa.Parameters.AddWithValue("@tipo_estudio", perfil.ExperienciaEducativa[i].TipoEstudio.Length > 0 ? perfil.ExperienciaEducativa[i].TipoEstudio[0].Valor : DBNull.Value.ToString());
					comExperienciaEducativa.Parameters.AddWithValue("@estado", perfil.ExperienciaEducativa[i].Estado.Length > 0 ? perfil.ExperienciaEducativa[i].Estado[0].Valor : DBNull.Value.ToString());
					comExperienciaEducativa.Parameters.AddWithValue("@fecha_desde", perfil.ExperienciaEducativa[i].FechaDesdeDT);
					comExperienciaEducativa.Parameters.AddWithValue("@fecha_hasta", perfil.ExperienciaEducativa[i].FechaHastaDT);
					comExperienciaEducativa.Parameters.AddWithValue("@actualmente_estudiando", perfil.ExperienciaEducativa[i].ActualmenteEstudiando ? 1 : 0);
					comExperienciaEducativa.Parameters.AddWithValue("@comentarios", string.IsNullOrEmpty(perfil.ExperienciaEducativa[i].Comentarios) ? DBNull.Value.ToString() : perfil.ExperienciaEducativa[i].Comentarios);
					comExperienciaEducativa.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comExperienciaEducativa.ExecuteReader();
					connection.Close();
					connection.Open();

				}
			}

			if (perfil.Idioma.Length > 0)
			{
				for (int i = 0; i < perfil.Idioma.Length; ++i)
				{
					var idioma = perfil.Idioma[i];
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comExperienciaEducativa = new SqlCommand("insert into idiomas_x_perfil (idioma, perfil, nivel_oral, nivel_escrito, comentarios, alta) output INSERTED.ID values" +
						" (@idioma, @perfil, @nivel_oral, @nivel_escrito, @comentarios, @alta)", connection);

					comExperienciaEducativa.Parameters.AddWithValue("@idioma", idioma.NombreIdioma.Length > 0 ? idioma.NombreIdioma[0].Id : DBNull.Value.ToString());
					comExperienciaEducativa.Parameters.AddWithValue("@perfil", perfilId);
					comExperienciaEducativa.Parameters.AddWithValue("@nivel_oral", idioma.NivelOral.Length > 0 ? idioma.NivelOral[0].Valor : DBNull.Value.ToString());
					comExperienciaEducativa.Parameters.AddWithValue("@nivel_escrito", idioma.NivelEscrito.Length > 0 ? idioma.NivelEscrito[0].Valor : DBNull.Value.ToString());
					comExperienciaEducativa.Parameters.AddWithValue("@comentarios", string.IsNullOrEmpty(idioma.Comentarios) ? DBNull.Value.ToString() : idioma.Comentarios);
					comExperienciaEducativa.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comExperienciaEducativa.ExecuteReader();
					connection.Close();
					connection.Open();

				}
			}

			if (perfil.OtrosConocimientos.Length > 0)
			{
				for (int i = 0; i < perfil.OtrosConocimientos.Length; ++i)
				{
					var otroConocimiento = perfil.OtrosConocimientos[i];
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comOtroConocimiento = new SqlCommand("insert into conocimientos_x_perfil (conocimiento, perfil, alta) values" +
						" (@conocimiento, @perfil, @alta)", connection);

					comOtroConocimiento.Parameters.AddWithValue("@conocimiento", otroConocimiento.Id);
					comOtroConocimiento.Parameters.AddWithValue("@perfil", perfilId);
					comOtroConocimiento.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comOtroConocimiento.ExecuteReader();
					connection.Close();
					connection.Open();

				}
			}

			connection.Close();
		}

		[HttpGet]
		public List<Perfil> GetList()
		{
			var perfiles = new List<Perfil>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select * from perfil", connection);
			SqlDataReader dr = com.ExecuteReader();


			while (dr.Read())
			{
				var perfil = new Perfil();
				perfil.Id = dr["id"].ToString();
				perfil.Nombre = dr["nombre"].ToString();
				perfil.Apellido = dr["apellido"].ToString();

				SqlConnection connectionPais = new SqlConnection(connectionstring);
				connectionPais.Open();
				SqlCommand comPais = new SqlCommand("select id, nombre from paises where id = @id", connectionPais);
				comPais.Parameters.AddWithValue("@id", dr["pais_residencia"].ToString());
				SqlDataReader drPais = comPais.ExecuteReader();
				while (drPais.Read())
				{
					perfil.PaisResidencia = new IdValor[] { new IdValor { Id = drPais["id"].ToString(), Valor = drPais["nombre"].ToString() } };
				}
				connectionPais.Close();

				perfiles.Add(perfil);
			}

			connection.Close();


			return perfiles;
		}
	}
}
