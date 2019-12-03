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

			if (perfil.Emails?.Length > 0)
			{
				for (int i = 0; i < perfil.Emails.Length; ++i)
				{
					var email = perfil.Emails[i];

					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comEmail = new SqlCommand("insert into correos_electronicos (perfil, correo, alta) values" +
						" (@perfil, @correo, @alta)", connection);

					comEmail.Parameters.AddWithValue("@perfil", perfilId);
					comEmail.Parameters.AddWithValue("@correo", email.Valor);
					comEmail.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comEmail.ExecuteReader();
					connection.Close();
					connection.Open();

				}
			}

			if (perfil.Telefonos?.Length > 0)
			{
				for (int i = 0; i < perfil.Telefonos.Length; ++i)
				{
					var telefono = perfil.Telefonos[i];

					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comTelefono = new SqlCommand("insert into telefonos (perfil, numero_telefono, alta) values" +
						" (@perfil, @numero_telefono, @alta)", connection);

					comTelefono.Parameters.AddWithValue("@perfil", perfilId);
					comTelefono.Parameters.AddWithValue("@numero_telefono", telefono.Valor);
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

			if (perfil.OtrosConocimientos?.Length > 0)
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
		[Route("perfil/get-by-alumno-id")]
		public Perfil GetPerfil([FromQuery] string alumnoId)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select perfil.id, " +
				"perfil.nombre, " +
				"perfil.apellido, " +
				"perfil.pais_residencia as pais_residencia_id, pais.nombre as pais_nombre, " +
				"perfil.provincia as provincia_id, provincia.nombre as provincia_nombre, " +
				"perfil.zona as zona_id, zona.nombre as zona_nombre, " +
				"perfil.ciudad as ciudad_id, ciudad.nombre as ciudad_nombre, " +
				"perfil.localidad as localidad_id, localidad.nombre as localidad_nombre, " +
				"perfil.fecha_nac, " +
				"perfil.estado_civil, " +
				"perfil.pais_nacionalidad as pais_nacionalidad_id, pais2.nombre as pais_nacionalidad_nombre, " +
				"perfil.tipo_documento, " +
				"perfil.documento, " +
				"perfil.objetivo_laboral, " +
				"perfil.intereses_personales, " +
				"perfil.alumno, " +
				"perfil.carrera as carrera_id, carrera.nombre as carrera_nombre, " +
				"perfil.porcentaje_mat_apr, " +
				"perfil.cantidad_mat_apr, " +
				"perfil.promedio, " +
				"perfil.anio_cursada " +
				"from perfil perfil " +
				"left join paises pais on perfil.pais_residencia = pais.id " +
				"left join paises pais2 on perfil.pais_nacionalidad = pais2.id " +
				"left join provincias provincia on perfil.provincia = provincia.id " +
				"left join zonas zona on perfil.zona = zona.id " +
				"left join ciudades ciudad on perfil.ciudad = ciudad.id " +
				"left join localidades localidad on perfil.localidad = localidad.id " +
				"left join carreras carrera on perfil.carrera = carrera.id " +
				"where perfil.alumno = @alumno", connection);

			com.Parameters.AddWithValue("@alumno", alumnoId);

			SqlDataReader dr = com.ExecuteReader();

			var perfil = new Perfil();
			if (dr.HasRows)
			{
				while (dr.Read())
				{
					perfil.Id = dr["id"].ToString();
					perfil.Nombre = dr["nombre"].ToString();
					perfil.Apellido = dr["apellido"].ToString();
					perfil.PaisResidencia = new IdValor[] { new IdValor { Id = dr["pais_residencia_id"].ToString(), Valor = dr["pais_nombre"].ToString() } };
					perfil.ProvinciaResidencia = new IdValor[] { new IdValor { Id = dr["provincia_id"].ToString(), Valor = dr["provincia_nombre"].ToString() } };
					perfil.Zona = new IdValor[] { new IdValor { Id = dr["zona_id"].ToString(), Valor = dr["zona_nombre"].ToString() } };
					perfil.Ciudad = new IdValor[] { new IdValor { Id = dr["ciudad_id"].ToString(), Valor = dr["ciudad_nombre"].ToString() } };
					perfil.Localidad = new IdValor[] { new IdValor { Id = dr["localidad_id"].ToString(), Valor = dr["localidad_nombre"].ToString() } };
					perfil.FechaNacimientoDT = Convert.ToDateTime(dr["fecha_nac"].ToString());
					perfil.EstadoCivil = dr["estado_civil"].ToString() != null ? new IdValor[] { new IdValor { Id = GetEstadoCivilId(dr["estado_civil"].ToString()), Valor = dr["estado_civil"].ToString() } } : null;
					perfil.PaisNacionalidad = new IdValor[] { new IdValor { Id = dr["pais_nacionalidad_id"].ToString(), Valor = dr["pais_nacionalidad_nombre"].ToString() } };
					perfil.TipoDocumento = dr["tipo_documento"].ToString();
					perfil.Documento = dr["documento"].ToString();
					perfil.ObjetivoLaboral = dr["objetivo_laboral"].ToString();
					perfil.InteresesPersonales = dr["intereses_personales"].ToString();
					perfil.Alumno = dr["alumno"].ToString();
					perfil.Carrera = new IdValor[] { new IdValor { Id = dr["carrera_id"].ToString(), Valor = dr["carrera_nombre"].ToString() } };
					perfil.PorcentajeMateriasAprobadas = (float)Convert.ToDouble(dr["porcentaje_mat_apr"]);
					perfil.CantidadMateriasAprobadas = Convert.ToInt32(dr["cantidad_mat_apr"]);
					perfil.Promedio = (float)Convert.ToDouble(dr["promedio"]);
					perfil.AnioCursada = Convert.ToInt32(dr["anio_cursada"]);
				}
			}
			else
			{
				return null;
			}
			connection.Close();
			connection.Open();

			SqlCommand comRedesSociales = new SqlCommand("select id, red_social, tipo_red, mostrar_feed from redes_sociales where perfil = @perfil", connection);
			comRedesSociales.Parameters.AddWithValue("@perfil", perfil.Id);
			SqlDataReader drRedesSociales = comRedesSociales.ExecuteReader();

			if (drRedesSociales.HasRows)
			{
				perfil.RedesSociales = new RedesSociales();
				while (drRedesSociales.Read())
				{
					switch (drRedesSociales["tipo_red"].ToString()) {
						case "FB":
							perfil.RedesSociales.usuarioFacebook = drRedesSociales["red_social"].ToString();
							perfil.RedesSociales.mostrarFeedFacebook = (bool)drRedesSociales["mostrar_feed"];
							break;
						case "TW":
							perfil.RedesSociales.usuarioTwitter = drRedesSociales["red_social"].ToString();
							perfil.RedesSociales.mostrarFeedTwitter = (bool)drRedesSociales["mostrar_feed"];
							break;
						case "IG":
							perfil.RedesSociales.usuarioInstagram = drRedesSociales["red_social"].ToString();
							perfil.RedesSociales.mostrarFeedInstagram = (bool)drRedesSociales["mostrar_feed"];
							break;
						case "LI":
							perfil.RedesSociales.usuarioLinkedIn = drRedesSociales["red_social"].ToString();
							perfil.RedesSociales.mostrarFeedLinkedIn = (bool)drRedesSociales["mostrar_feed"];
							break;
					}
				}
			}

			connection.Close();
			connection.Open();

			SqlCommand comExperienciasLaborales = new SqlCommand("select el.id, el.empresa, el.puesto as puesto_id, puesto.nombre as puesto_nombre, el.fecha_desde, el.fecha_hasta, el.actualmente_trabajando, cel.conocimiento as conocimiento_id, c.nombre as conocimiento_nombre, el.comentarios " +
				"from experiencias_laborales el " +
				"left join puestos puesto on el.puesto = puesto.id " +
				"inner join conocimientos_x_experiencia_laboral cel on el.id = cel.experiencia_laboral " +
				"left join conocimientos c on cel.conocimiento = c.id " +
				"where perfil = @perfil", connection);

			comExperienciasLaborales.Parameters.AddWithValue("@perfil", perfil.Id);

			SqlDataReader drExperienciasLaborales = comExperienciasLaborales.ExecuteReader();
			if (drExperienciasLaborales.HasRows)
			{
				var experienciasLaborales = new List<ExperienciaLaboral>();
				while(drExperienciasLaborales.Read())
				{
					var elIndex = experienciasLaborales.FindIndex(x => x.Id == drExperienciasLaborales["id"].ToString());
					if (elIndex > -1)
					{
						//Lo convierto a List
						List<Conocimiento> conocimientoListAux = new List<Conocimiento>(experienciasLaborales[elIndex].Conocimientos);
						//Agrego el item
						conocimientoListAux.Add(new Conocimiento { Id = drExperienciasLaborales["conocimiento_id"].ToString(), Nombre = drExperienciasLaborales["conocimiento_nombre"].ToString() });
						//Lo vuelvo a Array
						experienciasLaborales[elIndex].Conocimientos = conocimientoListAux.ToArray();
					}
					else
					{
						var experienciaLaboral = new ExperienciaLaboral();
						experienciaLaboral.Id = drExperienciasLaborales["id"].ToString();
						experienciaLaboral.Empresa = drExperienciasLaborales["empresa"].ToString();
						experienciaLaboral.PuestoLaboral = new Puesto[] { new Puesto { Id = drExperienciasLaborales["puesto_id"].ToString(), Nombre = drExperienciasLaborales["puesto_nombre"].ToString() } };
						experienciaLaboral.FechaDesdeDT = Convert.ToDateTime(drExperienciasLaborales["fecha_desde"].ToString());
						experienciaLaboral.FechaHastaDT = Convert.ToDateTime(drExperienciasLaborales["fecha_hasta"].ToString());
						experienciaLaboral.Conocimientos = new Conocimiento[] { new Conocimiento { Id = drExperienciasLaborales["conocimiento_id"].ToString(), Nombre = drExperienciasLaborales["conocimiento_nombre"].ToString() } };
						experienciaLaboral.ActualmenteTrabajando = (bool)drExperienciasLaborales["actualmente_trabajando"];
						experienciaLaboral.Descripcion = drExperienciasLaborales["comentarios"].ToString();

						experienciasLaborales.Add(experienciaLaboral);
					}
				}

				perfil.ExperienciaLaboral = experienciasLaborales.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand comED = new SqlCommand("select id, institucion, titulo, tipo_estudio, estado, fecha_desde, fecha_hasta, actualmente_estudiando, comentarios " +
				"from experiencias_educativas " +
				"where perfil = @perfil", connection);

			comED.Parameters.AddWithValue("@perfil", perfil.Id);

			SqlDataReader drED = comED.ExecuteReader();
			if (drED.HasRows)
			{
				var experienciasEducativas = new List<ExperienciaEducativa>();
				while (drED.Read())
				{
					var experienciaEducativa = new ExperienciaEducativa();
					experienciaEducativa.Id = drED["id"].ToString();
					experienciaEducativa.Institucion = drED["institucion"].ToString();
					experienciaEducativa.Titulo = drED["titulo"].ToString();
					experienciaEducativa.TipoEstudio = new IdValor[] { new IdValor { Id = GetTipoEstudio(drED["tipo_estudio"].ToString()), Valor = drED["tipo_estudio"].ToString() } };
					experienciaEducativa.Estado = new IdValor[] { new IdValor { Id = GetEstado(drED["estado"].ToString()), Valor = drED["estado"].ToString() } };
					experienciaEducativa.FechaDesdeDT = Convert.ToDateTime(drED["fecha_desde"].ToString());
					experienciaEducativa.FechaHastaDT = Convert.ToDateTime(drED["fecha_hasta"].ToString());
					experienciaEducativa.ActualmenteEstudiando = Convert.ToBoolean(drED["actualmente_estudiando"]);
					experienciaEducativa.Comentarios = drED["comentarios"].ToString();

					experienciasEducativas.Add(experienciaEducativa);
				}

				perfil.ExperienciaEducativa = experienciasEducativas.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand comIdioma = new SqlCommand("select ixp.id, ixp.idioma as idioma_id, i.nombre_idioma, ixp.nivel_oral, ixp.nivel_escrito, ixp.comentarios " +
				"from idiomas_x_perfil ixp inner join idiomas i on ixp.idioma = i.id " +
				"where perfil = @perfil", connection);

			comIdioma.Parameters.AddWithValue("@perfil", perfil.Id);

			SqlDataReader drIdioma = comIdioma.ExecuteReader();
			if (drIdioma.HasRows)
			{
				var idiomas = new List<Idioma>();
				while (drIdioma.Read())
				{
					var idioma = new Idioma();
					idioma.Id = drIdioma["id"].ToString();
					idioma.NombreIdioma = new IdValor[] { new IdValor { Id = drIdioma["idioma_id"].ToString(), Valor = drIdioma["nombre_idioma"].ToString() } };
					idioma.NivelOral = new IdValor[] { new IdValor { Id = GetNivel(drIdioma["nivel_oral"].ToString()), Valor = drIdioma["nivel_oral"].ToString() } };
					idioma.NivelEscrito = new IdValor[] { new IdValor { Id = GetNivel(drIdioma["nivel_escrito"].ToString()), Valor = drIdioma["nivel_escrito"].ToString() } };
					idioma.Comentarios = drIdioma["comentarios"].ToString();

					idiomas.Add(idioma);
				}

				perfil.Idioma = idiomas.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand comCXP = new SqlCommand("select cxp.id, cxp.conocimiento, c.nombre " +
				"from conocimientos_x_perfil cxp inner join conocimientos c on cxp.conocimiento = c.id " +
				"where perfil = @perfil", connection);

			comCXP.Parameters.AddWithValue("@perfil", perfil.Id);

			SqlDataReader drCXP = comCXP.ExecuteReader();
			if (drCXP.HasRows)
			{
				var conocimientos = new List<IdValor>();
				while (drCXP.Read())
				{
					var conocimiento = new IdValor();
					conocimiento.Id = drCXP["conocimiento"].ToString();
					conocimiento.Valor = drCXP["nombre"].ToString();

					conocimientos.Add(conocimiento);
				}

				perfil.OtrosConocimientos = conocimientos.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand comEmail = new SqlCommand("select id, correo from correos_electronicos " +
				"where perfil = @perfil", connection);

			comEmail.Parameters.AddWithValue("@perfil", perfil.Id);

			SqlDataReader drEmail = comEmail.ExecuteReader();
			if (drEmail.HasRows)
			{
				var emails = new List<IdValor>();
				while (drEmail.Read())
				{
					var email = new IdValor();
					email.Id = drEmail["id"].ToString();
					email.Valor = drEmail["correo"].ToString();

					emails.Add(email);
				}

				perfil.Emails = emails.ToArray();
			}

			connection.Close();
			connection.Open();

			SqlCommand comTelefono = new SqlCommand("select id, numero_telefono from telefonos " +
				"where perfil = @perfil", connection);

			comTelefono.Parameters.AddWithValue("@perfil", perfil.Id);

			SqlDataReader drTelefono = comTelefono.ExecuteReader();
			if (drTelefono.HasRows)
			{
				var telefonos = new List<IdValor>();
				while (drTelefono.Read())
				{
					var telefono = new IdValor();
					telefono.Id = drTelefono["id"].ToString();
					telefono.Valor = drTelefono["numero_telefono"].ToString();

					telefonos.Add(telefono);
				}

				perfil.Telefonos = telefonos.ToArray();
			}

			connection.Close();

			return perfil;
		}

		private string GetNivel(string v)
		{
			switch (v)
			{
				case "Básico":
					return "bas";
				case "Intermedio":
					return "int";
				case "Avanzado":
					return "ava";
				default:
					return "bas";
			}
		}

		private string GetEstado(string v)
		{
			switch (v)
			{
				case "Completo":
					return "com";
				case "En Curso":
					return "enc";
				default:
					return "com";
			}
		}

		private string GetTipoEstudio(string v)
		{
			switch (v)
			{
				case "Primario":
					return "pri";
				case "Secundario":
					return "sec";
				case "Terciario":
					return "ter";
				case "Universitario":
					return "uni";
				default:
					return "pri";
			}
		}

		private string GetEstadoCivilId(string v)
		{
			switch (v)
			{
				case "Soltero":
					return "1";
				case "En Pareja":
					return "2";
				case "Casado":
					return "3";
				case "Divorciado":
					return "4";
				case "Viudo":
					return "5";
				default:
					return "";
			}
		}
	}
}
