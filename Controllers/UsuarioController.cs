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
	public class UsuarioController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public UsuarioController(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		[HttpGet]
		public List<Usuario> GetList()
		{
			var usuarios = new List<Usuario>();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre_usuario, pass_usuario, tipo_usuario, alumno, empresa, convert(varchar,alta,103) as alta, aprobado, nombre, apellido from usuarios", connection);
			SqlDataReader dr = com.ExecuteReader();

			while (dr.Read())
			{
				var usuario = new Usuario();
				usuario.Id = dr["id"].ToString();
				usuario.NombreUsuario = dr["nombre_usuario"].ToString();
				usuario.Password = dr["pass_usuario"].ToString();
				usuario.TipoUsuario = dr["tipo_usuario"].ToString();
				usuario.AlumnoId = dr["alumno"].ToString();
				usuario.EmpresaId = dr["empresa"].ToString();
				usuario.Alta = dr["alta"].ToString();
				usuario.Aprobado = dr["aprobado"] == DBNull.Value ? false : Convert.ToBoolean(dr["aprobado"]);
				usuario.Nombre = dr["nombre"].ToString();
				usuario.Apellido = dr["apellido"].ToString();

				usuarios.Add(usuario);
			}

			connection.Close();

			return usuarios;
		}

		[HttpPost]
		[Route("set-aprobado")]
		public void SetAprobado([FromBody] Usuario u)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("update usuarios set aprobado = @aprobado where id = @id", connection);
			com.Parameters.AddWithValue("@aprobado", u.Aprobado);
			com.Parameters.AddWithValue("@id", u.Id);

			com.ExecuteReader();

			connection.Close();
		}

		[HttpGet]
		[Route("get-by-id")]
		public Usuario GetById([FromQuery] string nombreUsuario, [FromQuery] string password)
		{
			var usuario = new Usuario();

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre_usuario, pass_usuario, tipo_usuario, alumno, empresa, convert(varchar,alta,103) as alta, aprobado, nombre, apellido from usuarios " +
				"where nombre_usuario = @nombre_usuario and pass_usuario = @pass_usuario", connection);
			com.Parameters.AddWithValue("@nombre_usuario", nombreUsuario);
			com.Parameters.AddWithValue("@pass_usuario", password);

			SqlDataReader dr = com.ExecuteReader();

			if (dr.HasRows)
			{
				while (dr.Read())
				{
					if (dr["aprobado"] == DBNull.Value || Convert.ToBoolean(dr["aprobado"]) == false)
					{
						throw new Exception("El usuario no está aprobado");
					}
					usuario.Id = dr["id"].ToString();
					usuario.NombreUsuario = dr["nombre_usuario"].ToString();
					usuario.Password = dr["pass_usuario"].ToString();
					usuario.TipoUsuario = dr["tipo_usuario"].ToString();
					usuario.AlumnoId = dr["alumno"].ToString();
					usuario.EmpresaId = dr["empresa"].ToString();
					usuario.Alta = dr["alta"].ToString();
					usuario.Aprobado = dr["aprobado"] == DBNull.Value ? false : Convert.ToBoolean(dr["aprobado"]);
					usuario.Nombre = dr["nombre"].ToString();
					usuario.Apellido = dr["apellido"].ToString();
				}
			}
			else
			{
				throw new Exception("Datos de login incorrectos. Intente nuevamente.");
			}

			connection.Close();

			return usuario;
		}
	}
}
