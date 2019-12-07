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
	public class EmpresaController: ControllerBase
	{
		private readonly IConfiguration _configuration;

		public EmpresaController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public Usuario Post([FromBody] Empresa empresa)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();
			SqlCommand comUsr = new SqlCommand("select id from usuarios where nombre_usuario = @nombre_usuario", connection);
			comUsr.Parameters.AddWithValue("@nombre_usuario", empresa.ContactoNombreUsuario);

			SqlDataReader dr = comUsr.ExecuteReader();
			if (dr.HasRows)
			{
				throw new Exception("Ya existe un usuario con ese nombre!");
			}

			connection.Close();
			connection.Open();

			SqlCommand com = new SqlCommand("" +
				"insert into empresas (nombre, cuit, sitio_web, domicilio, contacto_nombre, contacto_apellido, contacto_telefono, contacto_email, contacto_cargo, contacto_usuario, alta) output INSERTED.ID" +
				" values (@nombre, @cuit, @sitio_web, @domicilio, @contacto_nombre, @contacto_apellido, @contacto_telefono, @contacto_email, @contacto_cargo, @contacto_usuario, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(empresa.Nombre) ? DBNull.Value.ToString() : empresa.Nombre);
			com.Parameters.AddWithValue("@cuit", string.IsNullOrEmpty(empresa.Cuit) ? DBNull.Value.ToString() : empresa.Cuit);
			com.Parameters.AddWithValue("@sitio_web", string.IsNullOrEmpty(empresa.SitioWeb) ? DBNull.Value.ToString() : empresa.SitioWeb);
			com.Parameters.AddWithValue("@domicilio", string.IsNullOrEmpty(empresa.Domicilio) ? DBNull.Value.ToString() : empresa.Domicilio);
			com.Parameters.AddWithValue("@contacto_nombre", string.IsNullOrEmpty(empresa.ContactoNombre) ? DBNull.Value.ToString() : empresa.ContactoNombre);
			com.Parameters.AddWithValue("@contacto_apellido", string.IsNullOrEmpty(empresa.ContactoApellido) ? DBNull.Value.ToString() : empresa.ContactoApellido);
			com.Parameters.AddWithValue("@contacto_telefono", string.IsNullOrEmpty(empresa.ContactoTelefono) ? DBNull.Value.ToString() : empresa.ContactoTelefono);
			com.Parameters.AddWithValue("@contacto_email", string.IsNullOrEmpty(empresa.ContactoEmail) ? DBNull.Value.ToString() : empresa.ContactoEmail);
			com.Parameters.AddWithValue("@contacto_cargo", string.IsNullOrEmpty(empresa.ContactoCargo) ? DBNull.Value.ToString() : empresa.ContactoCargo);
			com.Parameters.AddWithValue("@contacto_usuario", string.IsNullOrEmpty(empresa.ContactoNombreUsuario) ? DBNull.Value.ToString() : empresa.ContactoNombreUsuario);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var empresaId = (int)com.ExecuteScalar();
			connection.Close();
			connection.Open();

			if (empresa.Emails.Length > 0)
			{
				for(int i = 0; i < empresa.Emails.Length; ++i)
				{
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comEmail = new SqlCommand("insert into correos_electronicos_empresa (empresa, correo, alta) values" +
						" (@empresa, @correo, @alta)", connection);

					comEmail.Parameters.AddWithValue("@empresa", empresaId);
					comEmail.Parameters.AddWithValue("@correo", empresa.Emails[i].Valor);
					comEmail.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comEmail.ExecuteReader();
					connection.Close();
					connection.Open();
				}
			}

			if (empresa.Telefonos.Length > 0)
			{
				for (int i = 0; i < empresa.Telefonos.Length; ++i)
				{
					myDateTime = DateTime.Now;
					sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

					SqlCommand comTelefono = new SqlCommand("insert into telefonos_empresa (empresa, numero_telefono, alta) values" +
						" (@empresa, @numero_telefono, @alta)", connection);

					comTelefono.Parameters.AddWithValue("@empresa", empresaId);
					comTelefono.Parameters.AddWithValue("@numero_telefono", empresa.Telefonos[i].Valor);
					comTelefono.Parameters.AddWithValue("@alta", sqlFormattedDate);

					comTelefono.ExecuteReader();
					connection.Close();
					connection.Open();
				}
			}

			connection.Close();
			connection.Open();

			SqlCommand comEquiposPost = new SqlCommand("insert into equipos (nombre, tipo_equipo, empresa, alta) " +
				"values ('Administradores de Etapas de Definición de Convenio', 'e', @empresa, @alta)", connection);

			comEquiposPost.Parameters.AddWithValue("@empresa", empresaId);
			comEquiposPost.Parameters.AddWithValue("@alta", sqlFormattedDate);

			comEquiposPost.ExecuteReader();

			connection.Close();
			connection.Open();

			SqlCommand comEquiposPost2 = new SqlCommand("insert into equipos (nombre, tipo_equipo, empresa, alta) " +
				"values ('Seleccionador de alumnos', 'e', @empresa, @alta)", connection);

			comEquiposPost2.Parameters.AddWithValue("@empresa", empresaId);
			comEquiposPost2.Parameters.AddWithValue("@alta", sqlFormattedDate);

			comEquiposPost2.ExecuteReader();

			connection.Close();
			connection.Open();

			SqlCommand comEquiposPost3 = new SqlCommand("insert into equipos (nombre, tipo_equipo, empresa, alta) " +
				"values ('Gestores de ingreso de alumno', 'e', @empresa, @alta)", connection);

			comEquiposPost3.Parameters.AddWithValue("@empresa", empresaId);
			comEquiposPost3.Parameters.AddWithValue("@alta", sqlFormattedDate);

			comEquiposPost3.ExecuteReader();

			connection.Close();
			connection.Open();

			SqlCommand comUsrPost = new SqlCommand("insert into usuarios (nombre_usuario, pass_usuario, tipo_usuario, empresa, alta, nombre, apellido) output INSERTED.ID " +
				"values (@nombre_usuario, @pass_usuario, @tipo_usuario, @empresa, @alta, @nombre, @apellido)", connection);

			comUsrPost.Parameters.AddWithValue("@nombre_usuario", empresa.ContactoNombreUsuario);
			comUsrPost.Parameters.AddWithValue("@pass_usuario", empresa.Password);
			comUsrPost.Parameters.AddWithValue("@tipo_usuario", "e");
			comUsrPost.Parameters.AddWithValue("@empresa", empresaId);
			comUsrPost.Parameters.AddWithValue("@alta", sqlFormattedDate);
			comUsrPost.Parameters.AddWithValue("@nombre", empresa.ContactoNombre);
			comUsrPost.Parameters.AddWithValue("@apellido", empresa.ContactoApellido);

			var usuarioId = (int)comUsrPost.ExecuteScalar();

			connection.Close();
			connection.Open();

			SqlCommand comMie1 = new SqlCommand("insert into miembros_equipos (usuario, equipo, alta) " +
				"select @usuario, id, getdate() from equipos where empresa=@empresa", connection);

			comMie1.Parameters.AddWithValue("@usuario", usuarioId);
			comMie1.Parameters.AddWithValue("@empresa", empresaId);

			comMie1.ExecuteReader();

			connection.Close();
			connection.Open();
			SqlCommand comu = new SqlCommand("select id, nombre_usuario, pass_usuario, tipo_usuario, alumno, empresa, alta, aprobado, nombre, apellido from usuarios " +
					"where nombre_usuario = @nombre_usuario", connection);
			comu.Parameters.AddWithValue("@nombre_usuario", empresa.ContactoNombreUsuario);

			SqlDataReader dru = comu.ExecuteReader();

			var usuario = new Usuario();
			if (dru.HasRows)
			{
				while (dru.Read())
				{
					usuario.Id = dru["id"].ToString();
					usuario.NombreUsuario = dru["nombre_usuario"].ToString();
					usuario.Password = dru["pass_usuario"].ToString();
					usuario.TipoUsuario = dru["tipo_usuario"].ToString();
					usuario.AlumnoId = dru["alumno"].ToString();
					usuario.EmpresaId = dru["empresa"].ToString();
					usuario.Alta = dru["alta"].ToString();
					usuario.Aprobado = dru["aprobado"] == DBNull.Value ? false : Convert.ToBoolean(dru["aprobado"]);
					usuario.Nombre = dru["nombre"].ToString();
					usuario.Apellido = dru["apellido"].ToString();
				}
			}

			connection.Close();

			return usuario;
		}

		[HttpGet]
		public List<IdValor> GetList()
		{
			var empresas = new List<IdValor>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre from empresas", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var empresa = new IdValor();
				empresa.Id = dr["id"].ToString();
				empresa.Valor = dr["nombre"].ToString();

				empresas.Add(empresa);
			}

			connection.Close();

			return empresas;
		}
	}
}
