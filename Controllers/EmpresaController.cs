using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PortalEmpleos.Models;
using System;

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
		public void Post([FromBody] Empresa empresa)
		{
			DateTime myDateTime = DateTime.Now;
			string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			string connectionstring = _configuration.GetConnectionString("DefaultConnectionString");
			SqlConnection connection = new SqlConnection(connectionstring);
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
		}
	}
}
