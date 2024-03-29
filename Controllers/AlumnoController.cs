﻿using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PortalEmpleos.Models;

namespace PortalEmpleos.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AlumnoController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<AlumnoController> _logger;

		public AlumnoController(IConfiguration configuration, ILogger<AlumnoController> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPost]
		public Usuario Post([FromBody] Alumno alumno)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();
			SqlCommand comUsr = new SqlCommand("select id from usuarios where nombre_usuario = @nombre_usuario", connection);
			comUsr.Parameters.AddWithValue("@nombre_usuario", alumno.NombreUsuario);

			SqlDataReader dr = comUsr.ExecuteReader();
			if (dr.HasRows)
			{
				throw new Exception("Ya existe un usuario con ese nombre!");
			}

			connection.Close();
			connection.Open();

			SqlCommand com = new SqlCommand("insert into alumnos (nombre, apellido, libretaUniversitaria, email, tipoDocumento, documento, nombreUsuario, alta) output INSERTED.ID " +
											"values (@nombre, @apellido, @libretaUniversitaria, @email, @tipoDocumento, @documento, @nombreUsuario, @alta)", connection);

			com.Parameters.AddWithValue("@nombre", alumno.Nombres);
			com.Parameters.AddWithValue("@apellido", alumno.Apellidos);
			com.Parameters.AddWithValue("@libretaUniversitaria", string.IsNullOrEmpty(alumno.LibretaUniversitaria) ? DBNull.Value.ToString() : alumno.LibretaUniversitaria);
			com.Parameters.AddWithValue("@email", string.IsNullOrEmpty(alumno.Email) ? DBNull.Value.ToString() : alumno.Email);
			com.Parameters.AddWithValue("@tipoDocumento", string.IsNullOrEmpty(alumno.TipoDocumento) ? DBNull.Value.ToString() : alumno.TipoDocumento);
			com.Parameters.AddWithValue("@documento", string.IsNullOrEmpty(alumno.NumeroDocumento) ? DBNull.Value.ToString() : alumno.NumeroDocumento);
			com.Parameters.AddWithValue("@nombreUsuario", alumno.NombreUsuario);
			com.Parameters.AddWithValue("@alta", sqlFormattedDate);

			var id = (int)com.ExecuteScalar();

			connection.Close();
			connection.Open();

			SqlCommand comUsrPost = new SqlCommand("insert into usuarios (nombre_usuario, pass_usuario, tipo_usuario, alumno, alta, nombre, apellido) " +
				"values (@nombre_usuario, @pass_usuario, @tipo_usuario, @alumno, @alta, @nombre, @apellido)", connection);

			comUsrPost.Parameters.AddWithValue("@nombre_usuario", alumno.NombreUsuario);
			comUsrPost.Parameters.AddWithValue("@pass_usuario", alumno.Password);
			comUsrPost.Parameters.AddWithValue("@tipo_usuario", "a");
			comUsrPost.Parameters.AddWithValue("@alumno", id);
			comUsrPost.Parameters.AddWithValue("@alta", sqlFormattedDate);
			comUsrPost.Parameters.AddWithValue("@nombre", alumno.Nombres);
			comUsrPost.Parameters.AddWithValue("@apellido", alumno.Apellidos);

			comUsrPost.ExecuteReader();

			connection.Close();
			connection.Open();
			SqlCommand comu = new SqlCommand("select id, nombre_usuario, pass_usuario, tipo_usuario, alumno, empresa, alta, aprobado, nombre, apellido from usuarios " +
					"where nombre_usuario = @nombre_usuario", connection);
			comu.Parameters.AddWithValue("@nombre_usuario", alumno.NombreUsuario);

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

		[Route("/alumno/get-id-by-usuario")]
		[HttpPost]
		public Alumno GetId([FromBody] IdValor usuario)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre, apellido from alumnos where nombreUsuario = @nombreUsuario", connection);
			com.Parameters.AddWithValue("@nombreUsuario", usuario.Valor);
			SqlDataReader dr = com.ExecuteReader();

			Alumno alumno = new Alumno();
			while (dr.Read())
			{
				alumno.Id = dr["id"].ToString();
				alumno.Nombres = dr["nombre"].ToString();
				alumno.Apellidos = dr["apellido"].ToString();
			}
			connection.Close();

			return alumno;
		}

		[Route("/alumno/get-by-id")]
		[HttpGet]
		public Alumno GetId([FromQuery] string idAlumno)
		{
			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
			connection.Open();

			SqlCommand com = new SqlCommand("select id, nombre, apellido, libretaUniversitaria, email, tipoDocumento, documento, nombreUsuario from alumnos where id = @id", connection);
			com.Parameters.AddWithValue("@id", idAlumno);
			SqlDataReader dr = com.ExecuteReader();

			Alumno alumno = new Alumno();
			if (dr.HasRows)
			{
				while (dr.Read())
				{
					alumno.Id = dr["id"].ToString();
					alumno.Nombres = dr["nombre"].ToString();
					alumno.Apellidos = dr["apellido"].ToString();
					alumno.LibretaUniversitaria = dr["libretaUniversitaria"].ToString();
					alumno.Email = dr["email"].ToString();
					alumno.TipoDocumento = dr["tipoDocumento"].ToString();
					alumno.NumeroDocumento = dr["documento"].ToString();
					alumno.NombreUsuario = dr["nombreUsuario"].ToString();
				}
			}			
			connection.Close();

			return alumno;
		}
	}
}